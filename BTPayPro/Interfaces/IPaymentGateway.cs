using BTPayPro.Domaine;

namespace BTPayPro.Interfaces
{
    // --- DTOs for Payment Gateway Communication ---

    public record InitiatePaymentRequest(
        string InternalTransactionId,
        double Amount, // In TND
        string ReturnUrl
    );

    public record InitiatePaymentResponse(
        bool IsSuccess,
        string? RedirectUrl,
        string? ExternalOrderId,
        string? ErrorMessage
    );

    public record PaymentStatusResponse(
        string Status, // Mapped to our internal status: "Success", "Failed", "Pending"
        double Commission
    );

    // --- Payment Gateway Client Interface ---

    public interface IPaymentGatewayClient
    {
        Task<InitiatePaymentResponse> InitiatePaymentAsync(InitiatePaymentRequest request);
        Task<PaymentStatusResponse> GetPaymentStatusAsync(string externalOrderId);
    }

    // --- Core Services Interfaces (Simulated) ---

    public interface ITransactionRepository
    {
        Task<Transaction> GetTransactionByIdAsync(string id);
        Task AddTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
    }

    public interface IWalletService
    {
        Task UpdateWalletBalanceAsync(string walletId, double amount);
    }

    public interface IPaymentService
    {
        Task<InitiatePaymentResponse> ProcessPaymentInitiationAsync(string walletId, double amount);
        Task<bool> HandleClictopayCallbackAsync(string externalOrderId, string status, double amount);
    }
}

