using BTPayPro.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Web;

namespace BTPayPro.Api.Models
{
    public class ClictopayClient : IPaymentGatewayClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClictopaySettings _settings;
        private readonly ILogger<ClictopayClient> _logger;

        public ClictopayClient(HttpClient httpClient, IOptions<ClictopaySettings> settings, ILogger<ClictopayClient> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Clictopay uses millimes (1/1000 TND) for amount
        private int ConvertTNDToMillimes(double amountTND) => (int)(amountTND * 1000);

        public async Task<InitiatePaymentResponse> InitiatePaymentAsync(InitiatePaymentRequest request)
        {
            var amountMillimes = ConvertTNDToMillimes(request.Amount);
            var returnUrl = $"{_settings.CallbackBaseUrl}?orderId={request.InternalTransactionId}";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("userName", _settings.UserName),
                new KeyValuePair<string, string>("password", _settings.Password),
                new KeyValuePair<string, string>("orderNumber", request.InternalTransactionId),
                new KeyValuePair<string, string>("amount", amountMillimes.ToString()),
                new KeyValuePair<string, string>("currency", _settings.DefaultCurrency),
                new KeyValuePair<string, string>("returnUrl", returnUrl),
                new KeyValuePair<string, string>("language", _settings.DefaultLanguage)
            });

            try
            {
                // POST /register.do
                var response = await _httpClient.PostAsync("register.do", formContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Clictopay returns a JSON object with a 'formUrl' for redirection
                    // Example success response: {"errorCode":0,"errorMessage":"Success","formUrl":"https://...","orderId":"..."}
                    var jsonResponse = System.Text.Json.JsonDocument.Parse(responseString);
                    var errorCode = jsonResponse.RootElement.GetProperty("errorCode").GetInt32();

                    if (errorCode == 0)
                    {
                        var redirectUrl = jsonResponse.RootElement.GetProperty("formUrl").GetString();
                        var externalOrderId = jsonResponse.RootElement.GetProperty("orderId").GetString();

                        return new InitiatePaymentResponse(
                            IsSuccess: true,
                            RedirectUrl: redirectUrl,
                            ExternalOrderId: externalOrderId,
                            ErrorMessage: null
                        );
                    }
                    else
                    {
                        var errorMessage = jsonResponse.RootElement.GetProperty("errorMessage").GetString();
                        _logger.LogError("Clictopay API Error: {ErrorCode} - {ErrorMessage}", errorCode, errorMessage);
                        return new InitiatePaymentResponse(
                            IsSuccess: false,
                            RedirectUrl: null,
                            ExternalOrderId: null,
                            ErrorMessage: $"Payment gateway error: {errorMessage}"
                        );
                    }
                }
                else
                {
                    _logger.LogError("Clictopay HTTP Error: {StatusCode} - {Response}", response.StatusCode, responseString);
                    return new InitiatePaymentResponse(
                        IsSuccess: false,
                        RedirectUrl: null,
                        ExternalOrderId: null,
                        ErrorMessage: $"HTTP error: {response.StatusCode}"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Clictopay payment initiation.");
                return new InitiatePaymentResponse(
                    IsSuccess: false,
                    RedirectUrl: null,
                    ExternalOrderId: null,
                    ErrorMessage: $"Internal error: {ex.Message}"
                );
            }
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(string externalOrderId)
        {
            // GET /getOrderStatus.do
            var uriBuilder = new UriBuilder(_settings.BaseUrl + "/getOrderStatus.do");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["userName"] = _settings.UserName;
            query["password"] = _settings.Password;
            query["orderId"] = externalOrderId;
            uriBuilder.Query = query.ToString();

            try
            {
                var response = await _httpClient.GetAsync(uriBuilder.Uri);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Example success response: {"errorCode":0,"errorMessage":"Success","orderStatus":2,"amount":1000,"commission":10}
                    var jsonResponse = System.Text.Json.JsonDocument.Parse(responseString);
                    var errorCode = jsonResponse.RootElement.GetProperty("errorCode").GetInt32();

                    if (errorCode == 0)
                    {
                        var orderStatus = jsonResponse.RootElement.GetProperty("orderStatus").GetInt32();
                        var commissionMillimes = jsonResponse.RootElement.GetProperty("commission").GetInt32();
                        var commissionTND = (double)commissionMillimes / 1000.0;

                        // Clictopay Status Codes: 0 - Registered, 1 - Pre-authorized, 2 - Approved, 3 - Declined, 4 - Reversed, 5 - Refunded
                        string internalStatus = orderStatus switch
                        {
                            2 => "Success",
                            3 => "Failed",
                            _ => "Pending"
                        };

                        return new PaymentStatusResponse(
                            Status: internalStatus,
                            Commission: commissionTND
                        );
                    }
                    else
                    {
                        var errorMessage = jsonResponse.RootElement.GetProperty("errorMessage").GetString();
                        _logger.LogError("Clictopay Status API Error: {ErrorCode} - {ErrorMessage}", errorCode, errorMessage);
                        return new PaymentStatusResponse(
                            Status: "Failed",
                            Commission: 0
                        );
                    }
                }
                else
                {
                    _logger.LogError("Clictopay Status HTTP Error: {StatusCode} - {Response}", response.StatusCode, responseString);
                    return new PaymentStatusResponse(
                        Status: "Failed",
                        Commission: 0
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Clictopay status check.");
                return new PaymentStatusResponse(
                    Status: "Failed",
                    Commission: 0
                );
            }
        }
    }
}
