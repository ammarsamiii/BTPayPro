using BTPayPro.data;
using BTPayPro.Domaine;
using BTPayPro.Interfaces;

namespace BTPayPro.Api.Models
{
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        private readonly AppDbContext _context;
        public WalletRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
