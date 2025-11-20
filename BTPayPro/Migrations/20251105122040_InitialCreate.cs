using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTPayPro.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accountings",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Monetique = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    MonetiqueContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accountings", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "DetailRecords",
                columns: table => new
                {
                    BatchCodeRecord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordSequenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessingDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverCardOrWalletNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverCardOrWalletExpiryDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverCardOrWalletMsisdn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverIdentityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverIdentityValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionChannelDevice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MpaySystemTransactionIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWallet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWalletAccountRib = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWalletExpiryDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWalletMsisdn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWalletIdentityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWalletIdentityValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcquirerReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcquirerAuthorizationCodeInvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerAuthorizationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionAuthorizedAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionCurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfDecimalsInCurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionSettlementAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionInterchangeFees = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxeOnInterchangeFees = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterchangeFessHTaxe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterchangeFessTTC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerBankIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcquirerBankIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionProvenanceChannel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardSystemCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantDevice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TerminalIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filler = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReasonReject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerStartBalance = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "FileDetailRecords",
                columns: table => new
                {
                    BatchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordSequenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessingDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverCardOrWalletNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverCardOrWalletExpiryDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverCardOrWalletMsisdn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverIdentityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverIdentityValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionChannelDevice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MpaySystemTransactionIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWallet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWalletAccountRib = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWalletExpiryDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWalletMsisdn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWalletIdentityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerCardOrWalletIdentityValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcquirerReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcquirerAuthorizationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerAuthorizationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionAuthorizedAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionCurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfDecimalsInCurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionSettlementAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionInterchangeFees = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxeOnInterchangeFees = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterchangeFeesHTaxe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterchangeFeesTTC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuerBankIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcquirerBankIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionProvenanceChannel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardSystemCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantDevice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondPresentmentIndicator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChargebackOrSecondPresentmentReasonCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filler = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndOfRecord = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "FileHeaderRecords",
                columns: table => new
                {
                    HeaderCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordSequenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filler1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessingDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InternalRecordCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationBankIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filler2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndOfRecord = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "FileTrailerRecord",
                columns: table => new
                {
                    FileTrailerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordSequenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filler1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessingDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationBankIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GrandTotalSettlementAmountInDebitId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GrandTotalSettlementAmountInCreditId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filler2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndOfRecord = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "HeaderRecords",
                columns: table => new
                {
                    HeaderCodeRecord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordSequenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filler1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessingDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InternalRecordCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationBankIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filler2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndOfRecord = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TrailerRecord",
                columns: table => new
                {
                    FileTrailerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordSequenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filler1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessingDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationBankIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GrandTotalSettlementAmountAuthorizedInDebit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GrandTotalSettlementAmountAuthorizedInCredit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filler2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndOfRecord = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    ComplaintId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateComplaint = table.Column<DateOnly>(type: "date", nullable: false),
                    ComplaintStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.ComplaintId);
                    table.ForeignKey(
                        name: "FK_Complaints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    WalletId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false),
                    TransactionLimit = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.WalletId);
                    table.ForeignKey(
                        name: "FK_Wallets_Accountings_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accountings",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wallets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionAmount = table.Column<double>(type: "float", nullable: false),
                    TransactionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comission = table.Column<double>(type: "float", nullable: false),
                    WalletId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExternalOrderId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "WalletId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_UserId",
                table: "Complaints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId",
                table: "Transactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_AccountId",
                table: "Wallets",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "DetailRecords");

            migrationBuilder.DropTable(
                name: "FileDetailRecords");

            migrationBuilder.DropTable(
                name: "FileHeaderRecords");

            migrationBuilder.DropTable(
                name: "FileTrailerRecord");

            migrationBuilder.DropTable(
                name: "HeaderRecords");

            migrationBuilder.DropTable(
                name: "TrailerRecord");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Accountings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
