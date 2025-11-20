using BTPayPro.Domaine;

namespace BTPayPro.Api.Models
{
    // --- ACCOUNTING DTOs ---
    public record AccountingResponseDto(string AccountId, string AccountName, double Balance);
    public record AccountingRequestDto(string AccountName, double Balance);

    // --- TRANSACTION DTOs ---
    public record TransactionResponseDto(
        string TransactionId,
        string TransactionType,
        double TransactionAmount,
        DateOnly TransactionDate,
        string Status,
        double Comission,
        string WalletId,
        string? ExternalOrderId
    );
    public record TransactionRequestDto(
        string TransactionType,
        double TransactionAmount,
        string WalletId
    // Note: Status, Date, Comission, ExternalOrderId are set by the service
    );

    // Detailed transaction DTO with wallet and user info
    public record TransactionDetailDto(
        string TransactionId,
        string TransactionType,
        double TransactionAmount,
        DateOnly TransactionDate,
        string Status,
        double Comission,
        string WalletId,
        string? ExternalOrderId,
        string? UserEmail,
        string? UserName,
        double WalletBalance
    );

    public record TransactionUpdateDto(
        string? TransactionType,
        double? TransactionAmount,
        string? Status
    );

    // Transfer between users
    public record TransactionTransferRequestDto(
        string SenderWalletId,
        string RecipientWalletId,
        double Amount
    );

    public record TransactionAcceptDto(
        string RecipientWalletId
    );

    // --- USER DTOs ---
    public record UserResponseDto(string UserId, string Username, string Email);
    public record UserRequestDto(string Username, string Email);

    // --- PROFILE DTOs ---
    public record ProfileResponseDto(
        string UserId,
        string Email,
        string? FirstName,
        string? LastName,
        string? PhoneNumber,
        string? ProjectName,
        string UserType,
        string? ProfilePhotoUrl
    );

    public record ProfileSearchDto(
        string UserId,
        string? FirstName,
        string? LastName,
        string? Email,
        string? ProfilePhotoUrl,
        string? WalletId
    );

    public record ProfileUpdateDto(
        string? FirstName,
        string? LastName,
        string? PhoneNumber,
        string? ProjectName
    );

    // --- WALLET DTOs ---
    public record WalletResponseDto(string WalletId, double AccountBalance, double TransactionLimit);
    public record WalletRequestDto(double AccountBalance, double TransactionLimit);

    // Detailed wallet DTO with user and transaction info
    public record WalletDetailDto(
        string WalletId,
        double Balance,
        double TransactionLimit,
        string UserId,
        string UserEmail,
        string? UserName,
        string AccountId,
        int TransactionCount,
        DateTime CreatedDate
    );

    public record WalletUpdateDto(
        double? Balance,
        double? TransactionLimit
    );

    // --- COMPLAINT DTOs ---
    public record ComplaintResponseDto(
        string ComplaintId,
        string Description,
        DateOnly DateComplaint,
        string ComplaintStatus,
        string Channel,
        string? AdminResponse,
        DateTime? ResponseDate,
        string UserId
    );

    public record ComplaintRequestDto(
        string Description,
        string Channel
    );

    public record ComplaintDetailDto(
        string ComplaintId,
        string Description,
        DateOnly DateComplaint,
        string ComplaintStatus,
        string Channel,
        string? AdminResponse,
        DateTime? ResponseDate,
        string UserId,
        string UserEmail,
        string UserName
    );

    public record ComplaintAdminResponseDto(
        string AdminResponse
    );

    // --- MAPPING UTILITY (for simplicity, we'll use static methods) ---
    public static class DtoMapper
    {
        public static TransactionResponseDto ToDto(this Transaction entity) =>
            new(
                entity.TransactionId ?? string.Empty,
                entity.TransactionType ?? string.Empty,
                entity.TransactionAmount,
                entity.TransactionDate,
                entity.Status ?? "Unknown",
                entity.Comission,
                entity.WalletId ?? string.Empty,
                entity.ExternalOrderId
            );

        public static Transaction ToEntity(this TransactionRequestDto dto, string? id = null) =>
            new()
            {
                TransactionId = id ?? Guid.NewGuid().ToString(),
                TransactionType = dto.TransactionType ?? string.Empty,
                TransactionAmount = dto.TransactionAmount,
                WalletId = dto.WalletId ?? string.Empty,
                TransactionDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = "Created",
                Comission = 0
            };

        public static UserResponseDto ToDto(this User entity) =>
            new(
                entity.IdUser ?? string.Empty,
                entity.LastName ?? string.Empty,
                entity.Email ?? string.Empty
            );

        public static User ToEntity(this UserRequestDto dto, string? id = null) =>
            new()
            {
                IdUser = id ?? Guid.NewGuid().ToString(),
                LastName = dto.Username ?? string.Empty,
                Email = dto.Email ?? string.Empty
            };

        public static WalletResponseDto ToDto(this Wallet entity) =>
            new(
                entity.WalletId ?? string.Empty,
                entity.Balance,
                entity.TransactionLimit
            );

        public static Wallet ToEntity(this WalletRequestDto dto, string? id = null) =>
            new()
            {
                WalletId = id ?? Guid.NewGuid().ToString(),
                Balance = dto.AccountBalance,
                TransactionLimit = dto.TransactionLimit,
                UserId = string.Empty,
                AccountId = string.Empty
            };
    }
}
