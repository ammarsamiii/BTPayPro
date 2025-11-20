namespace BTPayPro.Autmpay.Models
{
    public class DetailRecord

    {
        
            public string BatchCodeRecord { get; set; } // Offset 1, Length 2
            public string RecordSequenceNumber { get; set; } // Offset 3, Length 6
            public string OperationCode { get; set; } // Offset 9, Length 2
            public string ProcessingDate { get; set; } // Offset 11, Length 6
            public string MerchantIdentification { get; set; } // Offset 17, Length 99
            public string ReceiverCardOrWalletNumber { get; set; } // Offset 116, Length 19
            public string ReceiverCardOrWalletExpiryDate { get; set; } // Offset 135, Length 4
            public string ReceiverCardOrWalletMsisdn { get; set; } // Offset 139, Length 8
            public string ReceiverIdentityType { get; set; } // Offset 147, Length 2
            public string ReceiverIdentityValue { get; set; } // Offset 149, Length 30
            public string TransactionChannelDevice { get; set; } // Offset 179, Length 1
            public string MpaySystemTransactionIdentification { get; set; } // Offset 180, Length 30
            public string IssuerCardOrWallet { get; set; } // Offset 210, Length 19
            public string IssuerCardOrWalletAccountRib { get; set; } // Offset 229, Length 24
            public string IssuerCardOrWalletExpiryDate { get; set; } // Offset 253, Length 4
            public string IssuerCardOrWalletMsisdn { get; set; } // Offset 257, Length 8
            public string IssuerCardOrWalletIdentityType { get; set; } // Offset 265, Length 2
            public string IssuerCardOrWalletIdentityValue { get; set; } // Offset 267, Length 30
            public string AcquirerReferenceNumber { get; set; } // Offset 297, Length 23
            public string AcquirerAuthorizationCodeInvoiceNumber { get; set; } // Offset 320, Length 6
            public string TransactionDate { get; set; } // Offset 326, Length 6
            public string IssuerAuthorizationCode { get; set; } // Offset 332, Length 6
            public string TransactionAuthorizedAmount { get; set; } // Offset 338, Length 12
            public string TransactionCurrencyCode { get; set; } // Offset 350, Length 3
            public string NumberOfDecimalsInCurrencyCode { get; set; } // Offset 353, Length 1
            public string TransactionSettlementAmount { get; set; } // Offset 354, Length 12
            public string TransactionInterchangeFees { get; set; } // Offset 366, Length 12
            public string TaxeOnInterchangeFees { get; set; } // Offset 378, Length 12
            public string InterchangeFessHTaxe { get; set; } // Offset 390, Length 12
            public string InterchangeFessTTC { get; set; } // Offset 402, Length 12
            public string IssuerBankIdentification { get; set; } // Offset 414, Length 5
            public string AcquirerBankIdentification { get; set; } // Offset 419, Length 5
            public string MerchantCategoryCode { get; set; } // Offset 424, Length 4
            public string TransactionProvenanceChannel { get; set; } // Offset 428, Length 1
            public string CardSystemCode { get; set; } // Offset 429, Length 1
            public string MerchantDevice { get; set; } // Offset 430, Length 1
            public string TransactionTypeCode { get; set; } // Offset 431, Length 2
            public string MerchantCity { get; set; } // Offset 433, Length 15
            public string MerchantName { get; set; } // Offset 448, Length 25
            public string TerminalIdentification { get; set; } // Offset 473, Length 10
            public string TransactionTime { get; set; } // Offset 483, Length 6
            public string Filler { get; set; } // Offset 489, Length 11
            public string OperationStatus { get; set; } // Offset 500, Length 2
            public string ReasonReject { get; set; } // Offset 502, Length 50
            public string IssuerStartBalance { get; set; } // Offset 552, Length 12
        }
    }

