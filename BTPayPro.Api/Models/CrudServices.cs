
using BTPayPro.Domaine;
using BTPayPro.Interfaces;
using BTPayPro.Services;

namespace BTPayPro.Api.Models
{
    public abstract class BaseCrudService<T> where T : class
    {
        protected readonly IRepositories<T> _repository;

        public BaseCrudService(IRepositories<T> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<T?> GetByIdAsync(string id) => await _repository.GetByIdAsync(id);
        public async Task AddAsync(T entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(T entity)
        {
            // In a real scenario, you might fetch the entity first, update properties, and then call Update.
            // For simplicity and matching the ComplaintService pattern:
            _repository.Update(entity);
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Remove(entity);
            }
        }
    }

    // --- Concrete Service Implementations ---



    public class TransactionCrudService : BaseCrudService<Transaction>, ITransactionCrudService
    {
        public TransactionCrudService(IRepositories<Transaction> repository) : base(repository) { }
    }

    public class UserCrudService : BaseCrudService<User>, IUserCrudService
    {
        public UserCrudService(IRepositories<User> repository) : base(repository) { }
    }

    public class WalletCrudService : BaseCrudService<Wallet>, IWalletCrudService
    {
        public WalletCrudService(IRepositories<Wallet> repository) : base(repository) { }
    }
}

