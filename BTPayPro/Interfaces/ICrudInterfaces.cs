using BTPayPro.Domaine;

namespace BTPayPro.Interfaces
{
    // Generic Repository Interface based on the methods used in your ComplaintService example
   // public interface IRepositories<T> where T : class
   // {
      //  Task<IEnumerable<T>> GetAllAsync();
      //  Task<T?> GetByIdAsync(string id);
     //   Task AddAsync(T entity);
     //   void Update(T entity);
    //    void Remove(T entity);
  //  }

    // Service Interfaces for each entity
    

    // ITransactionService is already partially defined, but we'll complete the CRUD
    public interface ITransactionCrudService
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(string id);
        Task AddAsync(Transaction entity);
        Task UpdateAsync(Transaction entity);
        Task DeleteAsync(string id);
    }

    public interface IUserCrudService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task AddAsync(User entity);
        Task UpdateAsync(User entity);
        Task DeleteAsync(string id);
    }

    // IWalletService is already partially defined, but we'll complete the CRUD
    public interface IWalletCrudService // Renaming to avoid conflict with existing IWalletService
    {
        Task<IEnumerable<Wallet>> GetAllAsync();
        Task<Wallet?> GetByIdAsync(string id);
        Task AddAsync(Wallet entity);
        Task UpdateAsync(Wallet entity);
        Task DeleteAsync(string id);
    }
}
