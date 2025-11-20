using BTPayPro.Autmpay.Models;
using BTPayPro.Autmpay.Services;

namespace BTPayPro.Autmpay.Parsers
{
    public class DetailRecordParser : IRecordParser<DetailRecord>
    {
        public DetailRecord Parse(string line)
        {
            return new DetailRecord
            {
                BatchCodeRecord = line.Substring(0, 2),
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
                AcquirerAuthorizationCodeInvoiceNumber = line.Substring(319, 6),
                TransactionDate = line.Substring(325, 6),
                IssuerAuthorizationCode = line.Substring(331, 6),
                TransactionAuthorizedAmount = line.Substring(337, 12),
                TransactionCurrencyCode = line.Substring(349, 3),
                NumberOfDecimalsInCurrencyCode = line.Substring(352, 1),
                TransactionSettlementAmount = line.Substring(353, 12),
                TransactionInterchangeFees = line.Substring(365, 12),
                TaxeOnInterchangeFees = line.Substring(377, 12),
                InterchangeFessHTaxe = line.Substring(389, 12),
                InterchangeFessTTC = line.Substring(401, 12),
                IssuerBankIdentification = line.Substring(413, 5),
                AcquirerBankIdentification = line.Substring(418, 5),
                MerchantCategoryCode = line.Substring(423, 4),
                TransactionProvenanceChannel = line.Substring(427, 1),
                CardSystemCode = line.Substring(428, 1),
                MerchantDevice = line.Substring(429, 1),
                TransactionTypeCode = line.Substring(430, 2),
                MerchantCity = line.Substring(432, 15),
                MerchantName = line.Substring(447, 25),
                TerminalIdentification = line.Substring(472, 10),
                TransactionTime = line.Substring(482, 6),
                Filler = line.Substring(488, 11),
                OperationStatus = line.Substring(499, 2),
                ReasonReject = line.Substring(501, 50),
                IssuerStartBalance = line.Substring(551, 12)
            };
        }
    }
}
