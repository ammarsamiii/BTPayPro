using BTPayPro.Interfaces;
using System.Security.Cryptography;

namespace BTPayPro.Api.Models
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentGatewayClient _clictopayClient;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletServices _walletService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentGatewayClient clictopayClient,
            ITransactionRepository transactionRepository,
            IWalletServices walletService,
            ILogger<PaymentService> logger)
        {
            _clictopayClient = clictopayClient;
            _transactionRepository = transactionRepository;
            _walletService = walletService;
            _logger = logger;
        }

        // Helper to generate a unique internal ID
        private string GenerateInternalTransactionId() => $"TX-{DateTime.UtcNow:yyyyMMddHHmmss}-{RandomNumberGenerator.GetInt32(1000, 9999)}";

        public async Task<InitiatePaymentResponse> ProcessPaymentInitiationAsync(string walletId, double amount)
        {
            // 1. Create a new internal transaction record
            var internalId = GenerateInternalTransactionId();
            var newTransaction = new Domaine.Transaction
            {
                TransactionId = internalId,
                WalletId = walletId,
                TransactionAmount = amount,
                TransactionType = "Payment", // Assuming a generic type for now
                TransactionDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = "Pending",
                Comission = 0,
                ExternalOrderId = null // Will be updated after Clictopay call
            };

            await _transactionRepository.AddTransactionAsync(newTransaction);

            // 2. Prepare and call Clictopay API
            var returnUrl = "https://btpaypro.api/api/payment/callback"; // This should be dynamically built or configured
            var request = new InitiatePaymentRequest(internalId, amount, returnUrl);
            var response = await _clictopayClient.InitiatePaymentAsync(request);

            // 3. Update transaction with external ID and save
            if (response.IsSuccess)
            {
                newTransaction.ExternalOrderId = response.ExternalOrderId;
                await _transactionRepository.UpdateTransactionAsync(newTransaction);
            }
            else
            {
                // Mark as failed if initiation failed
                newTransaction.Status = "Failed";
                await _transactionRepository.UpdateTransactionAsync(newTransaction);
            }

            return response;
        }

        public async Task<bool> HandleClictopayCallbackAsync(string internalTransactionId, string status, double amount)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(internalTransactionId);

            if (transaction == null)
            {
                _logger.LogWarning("Callback received for unknown transaction ID: {Id}", internalTransactionId);
                return false;
            }

            if (transaction.Status != "Pending")
            {
                _logger.LogInformation("Transaction {Id} already processed. Status: {Status}", internalTransactionId, transaction.Status);
                return true; // Already handled
            }

            // 1. Confirm final status with Clictopay (Security step)
            if (string.IsNullOrEmpty(transaction.ExternalOrderId))
            {
                _logger.LogError("Transaction {Id} is pending but has no ExternalOrderId. Cannot verify status.", internalTransactionId);
                transaction.Status = "Failed";
                await _transactionRepository.UpdateTransactionAsync(transaction);
                return false;
            }

            var statusResponse = await _clictopayClient.GetPaymentStatusAsync(transaction.ExternalOrderId);

            // 2. Update Transaction based on verified status
            transaction.Status = statusResponse.Status;
            transaction.Comission = statusResponse.Commission;

            if (statusResponse.Status == "Success")
            {
                // 3. Update Wallet (Assuming this is a deposit/top-up)
                // The amount should be the net amount after commission, or the full amount depending on business logic.
                // For simplicity, assuming the full amount is added to the wallet, and commission is tracked separately.
                await _walletService.UpdateWalletBalanceAsync(transaction.WalletId, transaction.TransactionAmount);
                _logger.LogInformation("Transaction {Id} succeeded. Wallet {WalletId} updated.", internalTransactionId, transaction.WalletId);
            }
            else
            {
                _logger.LogInformation("Transaction {Id} failed. Status: {Status}", internalTransactionId, statusResponse.Status);
            }

            await _transactionRepository.UpdateTransactionAsync(transaction);
            return statusResponse.Status == "Success";
        }
    }
}
