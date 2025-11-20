using BTPayPro.Domaine;
using BTPayPro.Interfaces;

namespace BTPayPro.Services
{
    public class AccountingService : IAccountingService
    {
        private readonly IRepositories<Accounting> _accountingRepository;

        public AccountingService(IRepositories<Accounting> accountingRepository)
        {
            _accountingRepository = accountingRepository;
        }

        public async Task<IEnumerable<Accounting>> GetAllAccountingsAsync()
        {
            return await _accountingRepository.GetAllAsync();
        }

        public async Task<Accounting> GetAccountingByIdAsync(string id)
        {
            return await _accountingRepository.GetByIdAsync(id);
        }

        public async Task AddAccountingAsync(Accounting accounting)
        {
            await _accountingRepository.AddAsync(accounting);
        }

        public async Task UpdateAccountingAsync(Accounting accounting)
        {
            _accountingRepository.Update(accounting);
        }

        public async Task DeleteAccountingAsync(string id)
        {
            var accounting = await _accountingRepository.GetByIdAsync(id);
            if (accounting != null)
            {
                _accountingRepository.Remove(accounting);
            }
        }
    }
}
