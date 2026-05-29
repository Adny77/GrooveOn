using GrooveOn.API.Controllers;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using GrooveOn.WebAPI.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace GrooveOn.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController
        : BaseCRUDController<PaymentResponse, PaymentSearchObject, PaymentUpsertRequest, PaymentUpsertRequest>
    {
        private readonly StripeSettings _stripeSettings;

        public PaymentController(
            IPaymentService paymentService,
            StripeSettings stripeSettings
        ) : base(paymentService)
        {
            _stripeSettings = stripeSettings;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("")]
        public override Task<PagedResult<PaymentResponse>> Get([FromQuery] PaymentSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{id}")]
        public override Task<PaymentResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public override Task<PaymentResponse> Create([FromBody] PaymentUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public override Task<PaymentResponse?> Update(int id, [FromBody] PaymentUpsertRequest request)
        {
            return base.Update(id, request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            throw new UserException("Payments cannot be deleted. Use payment status transitions to manage payment records.");
        }

        [Authorize(Roles = Roles.User)]
        [HttpPost("create-new-intent")]
        public async Task<IActionResult> CreateNewPaymentIntent(
            [FromBody] CreatePaymentIntentRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out var loggedInUserId))
                    return Unauthorized("Invalid token.");

                request.UserId = loggedInUserId;

                var result = await (_service as IPaymentService)!
                    .CreateNewPaymentIntentAsync(request);

                return Ok(result);
            }
            catch (UserException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (StripeException)
            {
                return StatusCode(500, "Stripe service error.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error while creating the payment intent.");
            }
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            try
            {
                using var reader = new StreamReader(HttpContext.Request.Body);
                var json = await reader.ReadToEndAsync();

                var signatureHeader = Request.Headers["Stripe-Signature"];

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signatureHeader,
                    _stripeSettings.WebhookSecret,
                    throwOnApiVersionMismatch: false
                );

                if (stripeEvent.Data.Object is not PaymentIntent paymentIntent)
                    return Ok();

                var paymentService = (_service as IPaymentService)!;

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        await paymentService.HandlePaymentIntentSucceededAsync(
                            paymentIntent.Id,
                            paymentIntent.Metadata
                        );
                        break;

                    case "payment_intent.payment_failed":
                        await paymentService.HandlePaymentIntentFailedAsync(
                            paymentIntent.Id,
                            paymentIntent.Metadata
                        );
                        break;

                    case "payment_intent.canceled":
                        await paymentService.HandlePaymentIntentCanceledAsync(
                            paymentIntent.Id,
                            paymentIntent.Metadata
                        );
                        break;
                }

                return Ok();
            }
            catch (StripeException)
            {
                return BadRequest("Invalid webhook.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error while processing the webhook.");
            }
        }
    }
}
