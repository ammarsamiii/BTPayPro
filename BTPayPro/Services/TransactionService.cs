using BTPayPro.Interfaces;

namespace BTPayPro.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepositories<Domaine.Transaction> _transactionRepository;

        public TransactionService(IRepositories<Domaine.Transaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<Domaine.Transaction>> GetAllTransactionsAsync()
        {
            return await _transactionRepository.GetAllAsync();
        }

        public async Task<Domaine.Transaction> GetTransactionByIdAsync(string id)
        {
            return await _transactionRepository.GetByIdAsync(id);
        }

        public async Task AddTransactionAsync(Domaine.Transaction transaction)
        {
            await _transactionRepository.AddAsync(transaction);
        }

        public async Task UpdateTransactionAsync(Domaine.Transaction transaction)
        {
            _transactionRepository.Update(transaction);
        }

        public async Task DeleteTransactionAsync(string id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction != null)
            {
                _transactionRepository.Remove(transaction);
            }
        }
    }
}
