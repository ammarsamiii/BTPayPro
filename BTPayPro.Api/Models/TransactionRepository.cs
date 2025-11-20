using BTPayPro.data;
using BTPayPro.Domaine;
using BTPayPro.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BTPayPro.Api.Models
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Example of transaction-specific query methods
        public async Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(string status)
        {
            return await _context.Transactions
                .Where(t => t.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByWalletIdAsync(string walletId)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == walletId)
                .ToListAsync();
        }

        public async Task<Transaction> GetLatestTransactionAsync(string walletId)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.TransactionDate)
                .FirstOrDefaultAsync();
        }

        public Task<Transaction> GetTransactionByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task AddTransactionAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTransactionAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
