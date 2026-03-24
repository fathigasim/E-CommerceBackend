using AutoMapper;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Products.DTOs;
using EcommerceApplication.Features.Products.Resources;
using EcommerceDomain.Entities;
using EcommerceDomain.Interfaces;
using MediaRTutorialApplication.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Net.Mime;


namespace MediaRTutorialApplication.Features.Products.Commands
{
    //public class CreateProductHandler
    // : IRequestHandler<CreateProductCommand, int>
    //{
    //    private readonly IUnitOfWork _unitofwork;
    //    private readonly IConfiguration configuration;
    //    public CreateProductHandler(IUnitOfWork unitofwork, IConfiguration configuration)
    //    {
    //        _unitofwork = unitofwork;
    //        this.configuration = configuration;
    //    }

    //    public async Task<int> Handle(
    //        CreateProductCommand request,
    //        CancellationToken ct)
    //    {

    //      var  _basePath = configuration["FileStorage:Path"] ?? "uploads";


    //        if (!Directory.Exists(_basePath))
    //            Directory.CreateDirectory(_basePath);
    //        var extension = request.File.ContentType switch
    //        {
    //            "image/jpg" => ".jpg",
    //            "image/jpeg" => ".jpg",
    //            "image/png" => ".png",
    //            "image/gif" => ".gif",
    //            "image/webp" => ".webp",
    //            _ => ".bin"
    //        };

    //        var fileName = $"{Guid.NewGuid()}-{request.File.Name}-{extension}";
    //        var filePath = Path.Combine(_basePath, fileName);
    //        using (var stream=new  FileStream(filePath, FileMode.Create)) { 

    //            request.File.CopyTo(stream);

    //        }
    //        //await File.WriteAllBytesAsync(filePath, request.File.ContentDisposition);
    //        var product = new Product
    //        {
    //            Name = request.Name,
    //            Price = request.Price,
    //            ImageUrl=filePath
    //        };
    //      await  _unitofwork.Products.AddAsync(product);
    //       await _unitofwork.CompleteAsync();


    //        return product.Id;
    //    }
    public class CreateProductCommandHandler
     : IRequestHandler<CreateProductCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly IFileStorageService _fileStorageService;
        private readonly IStringLocalizer<ProductResource> _localizer;
        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, 
            IFileStorageService fileStorageService, IStringLocalizer<ProductResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(
            CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Verify category exists
            var categoryExists = await _unitOfWork.Categories
                .ExistsAsync(request.CategoryId, cancellationToken);

            if (!categoryExists)
                return Result<string>.Failure("Category not found.");
            // Handle image upload
            string? imageUrl = null;
            string? imageFileName = null;

            if (request.Image is not null)
            {
                imageFileName = request.Image.FileName;
                imageUrl = await _fileStorageService.UploadFileAsync(
                    request.Image.Stream,
                    request.Image.FileName,
                    request.Image.ContentType,
                    cancellationToken);
            }
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId,
                ImageFileName = imageFileName,
                ImageUrl= imageUrl,
            };

            await _unitOfWork.Products.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with category
            var created = await _unitOfWork.Products
                .GetProductWithCategoryAsync(product.Id, cancellationToken);
           // _mapper.Map<ProductDto>(created!),
            return Result<string>.Success(_localizer["ProductCreated"]);
        }
    }

}
