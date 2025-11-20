using BTPayPro.Api.Models;
using BTPayPro.data;
using BTPayPro.Domaine;
using BTPayPro.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTPayPro.Api.Controllers
{
    [ApiController]

    // --- TRANSACTION CONTROLLER ---
    [Route("api/[controller]")]
    public class TransactionController : BaseCrudController<Transaction, TransactionRequestDto, TransactionResponseDto, ITransactionCrudService>
    {
        private readonly AppDbContext _dbContext;

        public TransactionController(ITransactionCrudService service, AppDbContext dbContext) : base(service)
        {
            _dbContext = dbContext;
        }

        // POST: api/Transaction/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequestDto dto)
        {
            try
            {
                // Verify wallet exists
                var wallet = await _dbContext.Wallets.FindAsync(dto.WalletId);
                if (wallet == null)
                {
                    return NotFound(new { message = "Wallet not found" });
                }

                var transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    TransactionType = dto.TransactionType,
                    TransactionAmount = dto.TransactionAmount,
                    WalletId = dto.WalletId,
                    TransactionDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    Status = "Created",
                    Comission = Math.Abs(dto.TransactionAmount) * 0.05, // 5% commission
                    ExternalOrderId = null
                };

                _dbContext.Transactions.Add(transaction);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Transaction created successfully", transaction = DtoMapper.ToDto(transaction) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating transaction", error = ex.Message });
            }
        }

        // GET: api/Transaction/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<TransactionDetailDto>>> GetUserTransactions(string userId)
        {
            try
            {
                var transactions = await _dbContext.Transactions
                    .Include(t => t.Wallet)
                    .ThenInclude(w => w.User)
                    .Where(t => t.Wallet.UserId == userId)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();

                var transactionDetails = transactions.Select(t => new TransactionDetailDto(
                    t.TransactionId,
                    t.TransactionType,
                    t.TransactionAmount,
                    t.TransactionDate,
                    t.Status,
                    t.Comission,
                    t.WalletId,
                    t.ExternalOrderId,
                    t.Wallet?.User?.Email,
                    $"{t.Wallet?.User?.FirstName} {t.Wallet?.User?.LastName}".Trim(),
                    t.Wallet?.Balance ?? 0
                )).ToList();

                return Ok(transactionDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching transactions", error = ex.Message });
            }
        }

        // GET: api/Transaction/details/{transactionId}
        [HttpGet("details/{transactionId}")]
        public async Task<ActionResult<TransactionDetailDto>> GetTransactionDetails(string transactionId)
        {
            try
            {
                var transaction = await _dbContext.Transactions
                    .Include(t => t.Wallet)
                    .ThenInclude(w => w.User)
                    .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

                if (transaction == null)
                {
                    return NotFound(new { message = "Transaction not found" });
                }

                var detail = new TransactionDetailDto(
                    transaction.TransactionId,
                    transaction.TransactionType,
                    transaction.TransactionAmount,
                    transaction.TransactionDate,
                    transaction.Status,
                    transaction.Comission,
                    transaction.WalletId,
                    transaction.ExternalOrderId,
                    transaction.Wallet?.User?.Email,
                    $"{transaction.Wallet?.User?.FirstName} {transaction.Wallet?.User?.LastName}".Trim(),
                    transaction.Wallet?.Balance ?? 0
                );

                return Ok(detail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching transaction", error = ex.Message });
            }
        }

        // PUT: api/Transaction/update/{transactionId}
        [HttpPut("update/{transactionId}")]
        public async Task<IActionResult> UpdateTransaction(string transactionId, [FromBody] TransactionUpdateDto dto)
        {
            var transaction = await _dbContext.Transactions.FindAsync(transactionId);
            if (transaction == null)
            {
                return NotFound(new { message = "Transaction not found" });
            }

            if (!string.IsNullOrEmpty(dto.TransactionType))
            {
                transaction.TransactionType = dto.TransactionType;
            }

            if (dto.TransactionAmount.HasValue)
            {
                transaction.TransactionAmount = dto.TransactionAmount.Value;
            }

            if (!string.IsNullOrEmpty(dto.Status))
            {
                transaction.Status = dto.Status;
            }

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "Transaction updated successfully", transaction = DtoMapper.ToDto(transaction) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating transaction", error = ex.Message });
            }
        }

        // POST: api/Transaction/transfer/request
        [HttpPost("transfer/request")]
        public async Task<IActionResult> RequestTransfer([FromBody] TransactionTransferRequestDto dto)
        {
            try
            {
                if (dto.Amount <= 0)
                {
                    return BadRequest(new { message = "Amount must be greater than 0" });
                }

                var senderWallet = await _dbContext.Wallets.FindAsync(dto.SenderWalletId);
                if (senderWallet == null)
                {
                    return NotFound(new { message = "Sender wallet not found" });
                }

                var recipientWallet = await _dbContext.Wallets.FindAsync(dto.RecipientWalletId);
                if (recipientWallet == null)
                {
                    return NotFound(new { message = "Recipient wallet not found" });
                }

                var transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    TransactionType = "TransferRequest",
                    TransactionAmount = dto.Amount,
                    WalletId = recipientWallet.WalletId,
                    TransactionDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    Status = "Pending",
                    Comission = 0,
                    ExternalOrderId = senderWallet.WalletId // store SENDER wallet id for traceability
                };

                _dbContext.Transactions.Add(transaction);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Transfer request created successfully", transaction = DtoMapper.ToDto(transaction) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating transfer request", error = ex.Message });
            }
        }

        // PUT: api/Transaction/transfer/{transactionId}/accept
        [HttpPut("transfer/{transactionId}/accept")]
        public async Task<IActionResult> AcceptTransfer(string transactionId, [FromBody] TransactionAcceptDto dto)
        {
            try
            {
                var transaction = await _dbContext.Transactions.FindAsync(transactionId);
                if (transaction == null)
                {
                    return NotFound(new { message = "Transaction not found" });
                }

                if (transaction.Status != "Pending" || transaction.TransactionType != "TransferRequest")
                {
                    return BadRequest(new { message = "Transaction is not a pending transfer request" });
                }

                var senderWallet = await _dbContext.Wallets.FindAsync(transaction.ExternalOrderId);
                if (senderWallet == null)
                {
                    return NotFound(new { message = "Sender wallet not found" });
                }

                var recipientWallet = await _dbContext.Wallets.FindAsync(transaction.WalletId);
                if (recipientWallet == null)
                {
                    return NotFound(new { message = "Recipient wallet not found" });
                }

                if (senderWallet.Balance < transaction.TransactionAmount)
                {
                    return BadRequest(new { message = "Insufficient balance" });
                }

                // Perform transfer
                senderWallet.Balance -= transaction.TransactionAmount;
                recipientWallet.Balance += transaction.TransactionAmount;
                transaction.Status = "Accepted";

                _dbContext.Wallets.Update(senderWallet);
                _dbContext.Wallets.Update(recipientWallet);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Transfer accepted successfully", transaction = DtoMapper.ToDto(transaction) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error accepting transfer", error = ex.Message });
            }
        }
    }

    // --- USER CONTROLLER ---
    [Route("api/[controller]")]
    public class UserController : BaseCrudController<User, UserRequestDto, UserResponseDto, IUserCrudService>
    {
        public UserController(IUserCrudService service) : base(service) { }
    }

    // --- WALLET CONTROLLER ---
    [Route("api/[controller]")]
    public class WalletController : BaseCrudController<Wallet, WalletRequestDto, WalletResponseDto, IWalletCrudService>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly AppDbContext _dbContext;

        public WalletController(IWalletCrudService service, IWalletRepository walletRepository, AppDbContext dbContext) : base(service)
        {
            _walletRepository = walletRepository;
            _dbContext = dbContext;
        }

        // GET: api/Wallet/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<WalletResponseDto>> GetByUserId(string userId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GetByUserId START - userId: {userId} ===");

                // Simple, direct query
                var wallet = await _dbContext.Wallets
                    .Where(w => w.UserId == userId)
                    .FirstOrDefaultAsync();

                System.Diagnostics.Debug.WriteLine($"Query executed. Wallet found: {wallet != null}");

                if (wallet == null)
                {
                    System.Diagnostics.Debug.WriteLine("Wallet not found, returning 404");
                    return NotFound(new { message = "Wallet not found for this user" });
                }

                System.Diagnostics.Debug.WriteLine($"Wallet data - WalletId: '{wallet.WalletId}', Balance: {wallet.Balance}");

                var dto = new WalletResponseDto(
                    wallet.WalletId,
                    wallet.Balance,
                    wallet.TransactionLimit
                );

                System.Diagnostics.Debug.WriteLine("=== GetByUserId SUCCESS ===");
                return Ok(dto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== GetByUserId EXCEPTION ===");
                System.Diagnostics.Debug.WriteLine($"Type: {ex.GetType().FullName}");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                }

                return StatusCode(500, new
                {
                    message = "Error fetching wallet",
                    error = ex.Message,
                    type = ex.GetType().Name
                });
            }
        }

        // GET: api/Wallet/details/{userId}
        [HttpGet("details/{userId}")]
        public async Task<ActionResult<WalletDetailDto>> GetWalletDetails(string userId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GetWalletDetails START - userId: {userId} ===");

                // Simple query for wallet
                var wallet = await _dbContext.Wallets
                    .Where(w => w.UserId == userId)
                    .FirstOrDefaultAsync();

                if (wallet == null)
                {
                    System.Diagnostics.Debug.WriteLine("Wallet not found");
                    return NotFound(new { message = "Wallet not found for this user" });
                }

                System.Diagnostics.Debug.WriteLine($"Wallet found - WalletId: {wallet.WalletId}");

                // Get user separately
                var user = await _dbContext.Users
                    .Where(u => u.IdUser == userId)
                    .FirstOrDefaultAsync();

                System.Diagnostics.Debug.WriteLine($"User found: {user != null}");

                // Get transaction count
                var transactionCount = await _dbContext.Transactions
                    .Where(t => t.WalletId == wallet.WalletId)
                    .CountAsync();

                System.Diagnostics.Debug.WriteLine($"Transaction count: {transactionCount}");

                string userEmail = user?.Email ?? "No email";
                string firstName = user?.FirstName ?? "";
                string lastName = user?.LastName ?? "";
                string userName = !string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName)
                    ? $"{firstName} {lastName}".Trim()
                    : "Unknown User";

                var detailDto = new WalletDetailDto(
                    wallet.WalletId,
                    wallet.Balance,
                    wallet.TransactionLimit,
                    wallet.UserId,
                    userEmail,
                    userName,
                    wallet.AccountId,
                    transactionCount,
                    DateTime.UtcNow
                );

                System.Diagnostics.Debug.WriteLine("=== GetWalletDetails SUCCESS ===");
                return Ok(detailDto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== GetWalletDetails EXCEPTION ===");
                System.Diagnostics.Debug.WriteLine($"Type: {ex.GetType().FullName}");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                }

                return StatusCode(500, new
                {
                    message = "Error fetching wallet details",
                    error = ex.Message,
                    type = ex.GetType().Name
                });
            }
        }

        // PUT: api/Wallet/update/{walletId}
        [HttpPut("update/{walletId}")]
        public async Task<IActionResult> UpdateWallet(string walletId, [FromBody] WalletUpdateDto dto)
        {
            var wallet = await _dbContext.Wallets.FindAsync(walletId);
            if (wallet == null)
            {
                return NotFound(new { message = "Wallet not found" });
            }

            if (dto.Balance.HasValue)
            {
                wallet.Balance = dto.Balance.Value;
            }

            if (dto.TransactionLimit.HasValue)
            {
                wallet.TransactionLimit = dto.TransactionLimit.Value;
            }

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "Wallet updated successfully", wallet = DtoMapper.ToDto(wallet) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating wallet", error = ex.Message });
            }
        }
    }

    // --- COMPLAINT CONTROLLER ---
    [Route("api/[controller]")]
    public class ComplaintController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ComplaintController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // POST: api/Complaint
        [HttpPost]
        public async Task<IActionResult> CreateComplaint([FromBody] ComplaintRequestDto dto)
        {
            var userId = Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User ID is required" });
            }

            var complaint = new Complaint
            {
                ComplaintId = Guid.NewGuid().ToString(),
                Description = dto.Description,
                Channel = dto.Channel,
                DateComplaint = DateOnly.FromDateTime(DateTime.UtcNow),
                ComplaintStatus = "Pending",
                UserId = userId,
                AdminResponse = null,
                ResponseDate = null
            };

            try
            {
                _dbContext.Complaints.Add(complaint);
                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "Complaint submitted successfully", complaintId = complaint.ComplaintId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating complaint", error = ex.Message });
            }
        }

        // GET: api/Complaint/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ComplaintDetailDto>>> GetUserComplaints(string userId)
        {
            try
            {
                var complaints = await _dbContext.Complaints
                    .Include(c => c.User)
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.DateComplaint)
                    .ToListAsync();

                var complaintDetails = complaints.Select(c => new ComplaintDetailDto(
                    c.ComplaintId,
                    c.Description,
                    c.DateComplaint,
                    c.ComplaintStatus,
                    c.Channel,
                    c.AdminResponse,
                    c.ResponseDate,
                    c.UserId,
                    c.User?.Email ?? "",
                    $"{c.User?.FirstName} {c.User?.LastName}".Trim()
                )).ToList();

                return Ok(complaintDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching complaints", error = ex.Message });
            }
        }

        // GET: api/Complaint/all (Admin only)
        [HttpGet("all")]
        public async Task<ActionResult<List<ComplaintDetailDto>>> GetAllComplaints()
        {
            try
            {
                var complaints = await _dbContext.Complaints
                    .Include(c => c.User)
                    .OrderByDescending(c => c.DateComplaint)
                    .ToListAsync();

                var complaintDetails = complaints.Select(c => new ComplaintDetailDto(
                    c.ComplaintId,
                    c.Description,
                    c.DateComplaint,
                    c.ComplaintStatus,
                    c.Channel,
                    c.AdminResponse,
                    c.ResponseDate,
                    c.UserId,
                    c.User?.Email ?? "",
                    $"{c.User?.FirstName} {c.User?.LastName}".Trim()
                )).ToList();

                return Ok(complaintDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching complaints", error = ex.Message });
            }
        }

        // GET: api/Complaint/{complaintId}
        [HttpGet("{complaintId}")]
        public async Task<ActionResult<ComplaintDetailDto>> GetComplaint(string complaintId)
        {
            try
            {
                var complaint = await _dbContext.Complaints
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.ComplaintId == complaintId);

                if (complaint == null)
                {
                    return NotFound(new { message = "Complaint not found" });
                }

                var detail = new ComplaintDetailDto(
                    complaint.ComplaintId,
                    complaint.Description,
                    complaint.DateComplaint,
                    complaint.ComplaintStatus,
                    complaint.Channel,
                    complaint.AdminResponse,
                    complaint.ResponseDate,
                    complaint.UserId,
                    complaint.User?.Email ?? "",
                    $"{complaint.User?.FirstName} {complaint.User?.LastName}".Trim()
                );

                return Ok(detail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching complaint", error = ex.Message });
            }
        }

        // PUT: api/Complaint/{complaintId}/respond (Admin only)
        [HttpPut("{complaintId}/respond")]
        public async Task<IActionResult> RespondToComplaint(string complaintId, [FromBody] ComplaintAdminResponseDto dto)
        {
            var complaint = await _dbContext.Complaints.FindAsync(complaintId);
            if (complaint == null)
            {
                return NotFound(new { message = "Complaint not found" });
            }

            complaint.AdminResponse = dto.AdminResponse;
            complaint.ResponseDate = DateTime.UtcNow;
            complaint.ComplaintStatus = "Done";

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "Response sent successfully", complaint });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error responding to complaint", error = ex.Message });
            }
        }
    }
}
