using BTPayPro.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BTPayPro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        public record InitiatePaymentRequestDto(
            string WalletId,
            double Amount
        );

        /// <summary>
        /// Initiates a payment transaction with Clictopay.
        /// </summary>
        /// <param name="request">The payment details.</param>
        /// <returns>A redirection URL to the Clictopay payment page.</returns>
        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequestDto request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest("Amount must be positive.");
            }

            // NOTE: WalletId should typically be derived from the authenticated user, not passed in the body.
            // Using it from the body for this simulation.
            var response = await _paymentService.ProcessPaymentInitiationAsync(request.WalletId, request.Amount);

            if (response.IsSuccess && !string.IsNullOrEmpty(response.RedirectUrl))
            {
                // Return the URL to the client. The client (e.g., a web browser) must then redirect the user.
                return Ok(new { RedirectUrl = response.RedirectUrl });
            }

            return StatusCode(500, new { ErrorMessage = response.ErrorMessage ?? "Failed to initiate payment." });
        }

        /// <summary>
        /// Clictopay callback endpoint (the returnUrl).
        /// </summary>
        /// <param name="orderId">The internal transaction ID used as orderId in the initial request.</param>
        /// <param name="status">The status returned by Clictopay (optional, but good for initial check).</param>
        /// <returns>A redirect to the final success/failure page of the application.</returns>
        [HttpGet("callback")]
        public async Task<IActionResult> ClictopayCallback([FromQuery] string orderId, [FromQuery] string status)
        {
            // In a real scenario, Clictopay might pass more parameters, but we rely on the orderId
            // to fetch the transaction and then call GetPaymentStatusAsync for security.

            _logger.LogInformation("Received Clictopay callback for internal order ID: {OrderId}", orderId);

            // The amount is not passed in the query, but we can assume it's part of the transaction data.
            // We pass a dummy amount (0) as it's not used in the service logic for this step.
            var success = await _paymentService.HandleClictopayCallbackAsync(orderId, status, 0);

            // In a real application, you would redirect to a user-facing page
            // e.g., "https://btpaypro.app/payment/success?txId=..."
            // For this simulation, we'll return a simple message.
            if (success)
            {
                return Redirect($"https://btpaypro.app/payment/success?txId={orderId}");
            }
            else
            {
                return Redirect($"https://btpaypro.app/payment/failure?txId={orderId}");
            }
        }
    }
}
