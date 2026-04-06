using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Products.Notifications;
using EcommerceDomain.Interfaces;
using MediaRTutorialApplication.Interfaces;
using MediatR;


namespace MediaRTutorialApplication.Features.Products.Commands
{
    //public class UpdateProductHandler
    //: IRequestHandler<UpdateProductCommand, Unit>
    //{
    //    private readonly IUnitOfWork _unitOfWork;

    //    public UpdateProductHandler(IUnitOfWork unitOfWork)
    //    {
    //        _unitOfWork = unitOfWork;   
    //    }

    //    public async Task<Unit> Handle(
    //        UpdateProductCommand request,
    //        CancellationToken ct)
    //    {
    //        var product = await _unitOfWork.Products.GetByIdAsync(request.Id);



    //        if (product == null)
    //            throw new KeyNotFoundException($"Product with ID {request.Id} was not found.");

    //        product.Name = request.Name;
    //        product.Price = request.Price;
    //         _unitOfWork.Products.Update(product);
    //            await _unitOfWork.CompleteAsync();
    //  return Unit.Value;
    //    }
    //}
    public class UpdateProductCommandHandler
   : IRequestHandler<UpdateProductCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMediator _mediator;
        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _mediator = mediator;
        }

        public async Task<Result<Unit>> Handle(
            UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products
                .GetByIdAsync(request.Id, cancellationToken);

            if (product is null)
                return Result<Unit>.Failure("Product not found.");
            // Handle image upload
            //string? imageUrl = null;
            //string? imageFileName = null;
            if (request.Image is not null)
            {
              var  imageFileName = request.Image.FileName;
              var  imageUrl = await _fileStorageService.UploadFileAsync(
                    request.Image.Stream,
                    request.Image.FileName,
                    request.Image.ContentType,
                    cancellationToken);
                await _fileStorageService.DeleteFileAsync(product.ImageUrl, cancellationToken);
                product.ImageUrl = imageUrl;
                product.ImageFileName = request.Image.FileName;
            }
        
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.CategoryId = request.CategoryId;
           
            
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _mediator.Publish(new ProductUpdatedNotification(product.Id));
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
