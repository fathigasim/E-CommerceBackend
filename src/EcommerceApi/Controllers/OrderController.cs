
using EcommerceApplication.Features.Orders.Commands;
using EcommerceApplication.Features.Orders.Queries;
using EcommerceApplication.Features.Orders.Queries.AllOrders;
using EcommerceApplication.Features.Orders.Queries.PagedOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace MediaRTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IMediator _mediator;

        public OrderController(ILogger<OrderController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        { 
        return Ok(await _mediator.Send(new GetAllOrdersQuery()));
        }
            [HttpGet("GetOrders")]
        public async Task <IActionResult> GetOrders(string? q,int pageNumber=1,int pageSize=6)
        {
            // Placeholder for getting orders
          var result=await  _mediator.Send(new GetPagedOrdersQuery(q,pageNumber,pageSize));

            return Ok(result);
        }

        [HttpGet("GetOrderById/{id}")] 
        public IActionResult GetOrderById(Guid id)
        {
            // Placeholder for getting a specific order by ID
            return Ok(new { Message = $"Get order with ID: {id}" });
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderCommand createOrderCommand)
        {
            if (createOrderCommand == null)
                return BadRequest(new { Message = "Invalid order data" });

            try
            {
                var result = await _mediator.Send(createOrderCommand); // ✅ awaited

                if (!result.IsSuccess)
                    return BadRequest(new { Message = result.ErrorMessage });

                return Ok(new { Message = "Order created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
