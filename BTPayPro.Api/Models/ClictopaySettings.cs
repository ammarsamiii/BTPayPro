namespace BTPayPro.Api.Models
{
    public class ClictopaySettings
    {
        public const string SectionName = "ClictopaySettings";
        public string BaseUrl { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = "788"; // TND
        public string DefaultLanguage { get; set; } = "en";
        public string CallbackBaseUrl { get; set; } = string.Empty;
    }
}
