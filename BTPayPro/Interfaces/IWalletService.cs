using BTPayPro.Domaine;

namespace BTPayPro.Interfaces
{
    public interface IWalletServices
    {
        Task<IEnumerable<Wallet>> GetAllWalletsAsync();
    Task<Wallet> GetWalletByIdAsync(string id);
    Task AddWalletAsync(Wallet wallet);
    Task UpdateWalletAsync(Wallet wallet);
    Task DeleteWalletAsync(string id);
    Task UpdateWalletBalanceAsync(string walletId, double amount);
    }
}
