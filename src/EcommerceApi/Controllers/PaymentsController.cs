using EcommerceApplication.Features.Payment.Queries.PaymentList;
using MediaRTutorialApplication.Features.Payment.Commands.CreatePaymentIntent;
using MediaRTutorialApplication.Features.Payment.Commands.RefundPayment;
using MediaRTutorialApplication.Features.Payment.Queries.GetPaymentById;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediaRTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Policy ="AdminOrUser")]
    //[Authorize(Policy = "AdminOrUser")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Get(
        int pageNumber=1, int pageSize=4)
        { 
              var result=  await _mediator.Send(new PaymentListQuery(pageNumber,pageSize));
            return Ok(result);
        }

            [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent(
            [FromBody] CreatePaymentIntentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new CreatePaymentIntentCommand(
                userId,
                request.Amount,
                request.Currency,
                request.CustomerEmail);

            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok(result.Data)
                : BadRequest(new { error = result.ErrorMessage });
        }

        [HttpPost("{paymentId:guid}/refund")]
        public async Task<IActionResult> RefundPayment(
            Guid paymentId,
            [FromBody] RefundRequest? request = null)
        {
            var command = new RefundPaymentCommand(paymentId, request?.Amount);
            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok(result.Data)
                : BadRequest(new { error = result.ErrorMessage });
        }

        [HttpGet("{paymentId:guid}")]
        public async Task<IActionResult> GetPayment(Guid paymentId)
        {
            var query = new GetPaymentByIdQuery(paymentId);
            var result = await _mediator.Send(query);

            return result.IsSuccess
                ? Ok(result.Data)
                : NotFound(new { error = result.ErrorMessage });
        }
    }

    // Request DTOs
    public record CreatePaymentIntentRequest(
        decimal Amount,
        string Currency,
        string CustomerEmail);

    public record RefundRequest(decimal? Amount);
}
