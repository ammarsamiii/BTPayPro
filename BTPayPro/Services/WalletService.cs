
using BTPayPro.data;

using BTPayPro.Domaine;
using BTPayPro.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BTPayPro.Services
{
    public class WalletService : IWalletServices
    {
        private readonly IRepositories<Wallet> _walletRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<WalletService> _logger;

        public WalletService(IRepositories<Wallet> walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<IEnumerable<Wallet>> GetAllWalletsAsync()
        {
            return await _walletRepository.GetAllAsync();
        }

        public async Task<Wallet> GetWalletByIdAsync(string id)
        {
            return await _walletRepository.GetByIdAsync(id);
        }

        public async Task AddWalletAsync(Wallet wallet)
        {
            await _walletRepository.AddAsync(wallet);
        }

        public async Task UpdateWalletAsync(Wallet wallet)
        {
            _walletRepository.Update(wallet);
        }

        public async Task DeleteWalletAsync(string id)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet != null)
            {
                _walletRepository.Remove(wallet);
            }
        }
       
        public async Task UpdateWalletBalanceAsync(string walletId, double amount)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.WalletId == walletId);

            if (wallet == null)
            {
                _logger.LogError("Wallet not found for ID: {WalletId}", walletId);
                throw new KeyNotFoundException($"Wallet with ID {walletId} not found.");
            }

            // Perform the balance update
            wallet.Balance += amount;

            // In a real application, you would also check for transaction limits and concurrency issues.

            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Wallet {WalletId} balance updated by {Amount}. New balance: {NewBalance}",
                walletId, amount, wallet.Balance);
        }

    }
}