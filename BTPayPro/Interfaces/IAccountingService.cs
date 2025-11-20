using BTPayPro.Domaine;

namespace BTPayPro.Interfaces
{
    public interface IAccountingService
    {
        Task<IEnumerable<Accounting>> GetAllAccountingsAsync();
        Task<Accounting> GetAccountingByIdAsync(string id);
        Task AddAccountingAsync(Accounting accounting);
        Task UpdateAccountingAsync(Accounting accounting);
        Task DeleteAccountingAsync(string id);
    }
}
