using BTPayPro.CPMPay.Models;
using BTPayPro.CPMPay.Services;

namespace BTPayPro.CPMPay.Parsers
{
    public class FileDetailRecordParser : IFileRecordParser<FileDetailRecord>
    {
        public FileDetailRecord Parse(string line)
        {
            if (line.Length != 500)
            {
                throw new ArgumentException("Record line must be 500 characters long.");
            }

            return new FileDetailRecord
            {
                BatchCode = line.Substring(0, 2),
                RecordSequenceNumber = line.Substring(2, 6),
                OperationCode = line.Substring(8, 2),
                ProcessingDate = line.Substring(10, 6),
                MerchantIdentification = line.Substring(16, 99),
                ReceiverCardOrWalletNumber = line.Substring(115, 19),
                ReceiverCardOrWalletExpiryDate = line.Substring(134, 4),
                ReceiverCardOrWalletMsisdn = line.Substring(138, 8),
                ReceiverIdentityType = line.Substring(146, 2),
                ReceiverIdentityValue = line.Substring(148, 30),
                TransactionChannelDevice = line.Substring(178, 1),
                MpaySystemTransactionIdentification = line.Substring(179, 30),
                IssuerCardOrWallet = line.Substring(209, 19),
                IssuerCardOrWalletAccountRib = line.Substring(228, 24),
                IssuerCardOrWalletExpiryDate = line.Substring(252, 4),
                IssuerCardOrWalletMsisdn = line.Substring(256, 8),
                IssuerCardOrWalletIdentityType = line.Substring(264, 2),
                IssuerCardOrWalletIdentityValue = line.Substring(266, 30),
                AcquirerReferenceNumber = line.Substring(296, 23),
                AcquirerAuthorizationCode = line.Substring(319, 6),
                TransactionDate = line.Substring(325, 6),
                IssuerAuthorizationCode = line.Substring(331, 6),
                TransactionAuthorizedAmount = line.Substring(337, 12),
                TransactionCurrencyCode = line.Substring(349, 3),
                NumberOfDecimalsInCurrencyCode = line.Substring(352, 1),
                TransactionSettlementAmount = line.Substring(353, 12),
                TransactionInterchangeFees = line.Substring(365, 12),
                TaxeOnInterchangeFees = line.Substring(377, 12),
                InterchangeFeesHTaxe = line.Substring(389, 12),
                InterchangeFeesTTC = line.Substring(401, 12),
                IssuerBankIdentification = line.Substring(413, 5),
                AcquirerBankIdentification = line.Substring(418, 5),
                MerchantCategoryCode = line.Substring(423, 4),
                TransactionProvenanceChannel = line.Substring(427, 1),
                CardSystemCode = line.Substring(428, 1),
                MerchantDevice = line.Substring(429, 1),
                TransactionTypeCode = line.Substring(430, 2),
                MerchantCity = line.Substring(432, 15),
                MerchantName = line.Substring(447, 25),
                SecondPresentmentIndicator = line.Substring(472, 1),
                ChargebackOrSecondPresentmentReasonCode = line.Substring(473, 2),
                Filler = line.Substring(475, 24),
                EndOfRecord = line.Substring(499, 1)
            };
        }
    }
}
