using EcommerceApplication.Features.Products.Queries;
using MediaRTutorialApplication.Features.Products.Commands;
using MediaRTutorialApplication.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MediaRTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("PagedProducts")]
        public async Task<IActionResult> GetAllProductsPaginatedAsync(string? q,Guid?categoryId, int pageNumber=1, int pageSize=8)
 {

            return Ok(await _mediator.Send(new GetPaginatedProductsQuery(q,categoryId,pageNumber, pageSize)));
        }
        [HttpGet("AllProducts")]
        public async Task<IActionResult> GetAllProductsAsync()
        {

            return Ok(await _mediator.Send(new GetAllProductsQuery()));
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetByIdAsync(Guid Id)
        {

            return Ok(await _mediator.Send(new GetProductByIdQuery(Id)));
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateProductRequest request,
        CancellationToken cancellationToken)
        {
            ImageUploadData? imageData = null;

            if (request.Image is not null && request.Image.Length > 0)
            {
                imageData = new ImageUploadData(
                    request.Image.OpenReadStream(),
                    request.Image.FileName,
                    request.Image.ContentType);
            }
            var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
             imageData,
            request.CategoryId
           );
            var result = await _mediator.Send(command,cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Validation failed",
                    errors = result.Errors  // Your errors list
                });
            return Ok(result);
         
        }
            
        
          
        
        [HttpPut("{Id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromRoute] Guid Id,
    [FromForm] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            //if (id != request)
            //    return BadRequest("ID mismatch");
            ImageUploadData? imageData = null;

            if (request.Image is not null && request.Image.Length > 0)
            {
                imageData = new ImageUploadData(
                    request.Image.OpenReadStream(),
                    request.Image.FileName,
                    request.Image.ContentType);
            }
            var command = new UpdateProductCommand(
                request.Id,
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
             imageData,
            request.CategoryId
           );
            await _mediator.Send(command,cancellationToken);
            return NoContent();

        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UploadProductImageCommand cmd)
        {
            if (id != cmd.ProductId)
                return BadRequest("ID mismatch");
            using var ms = new MemoryStream();
            await cmd.File.CopyToAsync(ms);
            cmd.FileContent = ms.ToArray(); // optional if your handler expects byte[]
            await _mediator.Send(cmd);
            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id,DeleteProductCommand cmd)
        {
            if (id != cmd.Id)
            {
                BadRequest("ID mismatch");
                    }; 
                
                await _mediator.Send(cmd);
            return NoContent();
        }

            //[HttpPost("OrderEvent")]
            //public async Task<IActionResult> CreateEvent(int id)
            //{
            //    await _mediator.Publish(new OrderCreatedEvent(id));
            //    return Ok();
            //}

        }

    // API/Requests/CreateProductRequest.cs
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile? Image { get; set; }    // ← API-level concern
    }

    public class UpdateProductRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile? Image { get; set; }    // ← API-level concern
    }
}
