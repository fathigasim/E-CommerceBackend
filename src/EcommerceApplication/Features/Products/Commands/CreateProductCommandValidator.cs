using EcommerceApplication.Common.Validators;
using EcommerceApplication.Common.Validators.Resources;
using EcommerceApplication.Features.Products.Resources;
using FluentValidation;
using FluentValidation.Resources;
using MediaRTutorialApplication.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRTutorialApplication.Features.Products.Commands
{
    public class CreateProductCommandValidator : LocalizedValidator<CreateProductCommand>//AbstractValidator<CreateProductCommand>
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
                                                          // public CreateProductCommandValidator()
                                                          //public CreateProductCommandValidator(
                                                          // IStringLocalizer<ValidationMessages> localizer) : base(localizer)
        public CreateProductCommandValidator(
        IStringLocalizer<ValidationMessages> localizer
            //,
       // IStringLocalizer<ProductResource> productLocalizer
            ) : base(localizer)

        //  RuleFor(x => x.Name)
        //.NotEmpty()
        //.WithMessage(validationLocalizer["Required"])
        //.WithName(productLocalizer["Name"])
        //.MaximumLength(100)
        //.WithMessage(validationLocalizer["MaxLength"])
        //.WithName(productLocalizer["Name"]);

        //RuleFor(x => x.Price)
        //    .GreaterThan(0)
        //    .WithMessage(validationLocalizer["GreaterThan"])
        //    .WithName(productLocalizer["Price"]);
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Required)//.WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage(MaxLength);//.WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(Required)//("Description is required.")
                .MaximumLength(2000).WithMessage(MaxLength);//("Description must not exceed 2000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage(GreaterThan);//("Price must be greater than zero.");

            RuleFor(x => x.StockQuantity).NotEmpty().WithMessage("Stock quantity is required.")
                .GreaterThanOrEqualTo(1).WithMessage("Stock quantity cannot be negative.");

            //RuleFor(x => x.CategoryId)
            //    .NotEmpty().WithMessage("Category is required.");
            RuleFor(x => x.CategoryId)
      .NotEmpty().WithMessage("Category is required.")
      .NotEqual(Guid.Empty).WithMessage("Please select a valid category.");
            When(x => x.Image != null, () =>
            {
                RuleFor(x => x.Image!)
                    .Must(ValidateImageContent)
                    .WithMessage("Invalid image file. File appears to be corrupted or not a valid image.");
                RuleFor(x => x.Image!.FileName)
                  .Must(HaveValidExtension)
                  .WithMessage($"Invalid file extension. Allowed: {string.Join(", ", AllowedExtensions)}");

                RuleFor(x => x.Image!.ContentType)
                    .Must(contentType => AllowedContentTypes.Contains(contentType.ToLower()))
                    .WithMessage($"Invalid content type. Allowed: {string.Join(", ", AllowedContentTypes)}");

                RuleFor(x => x.Image!.Stream)
                    .Must(stream => stream.Length <= MaxFileSize)
                    .WithMessage($"File size must not exceed {MaxFileSize / 1024 / 1024}MB")
                    .Must(stream => stream.Length > 0)
                    .WithMessage("File cannot be empty");

            });
        }
         
        private bool HaveValidExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }
        private bool ValidateImageContent(ImageUploadData imageData)
        {
            try
            {
                // Read the first few bytes to check magic numbers (file signatures)
                var buffer = new byte[8];
                imageData.Stream.Position = 0;
                imageData.Stream.Read(buffer, 0, buffer.Length);
                imageData.Stream.Position = 0; // Reset position

                // Check for common image file signatures
                // JPEG: FF D8 FF
                if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                    return true;

                // PNG: 89 50 4E 47 0D 0A 1A 0A
                if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                    return true;

                // GIF: 47 49 46 38
                if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x38)
                    return true;

                // WEBP: 52 49 46 46 ... 57 45 42 50
                if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}


//using FluentValidation.Resources;
//using System.Globalization;

//namespace Application.Common.Validators;

//public class ArabicLanguageManager : LanguageManager
//{
//    public ArabicLanguageManager()
//    {
//        // Add Arabic translations for built-in validators
//        AddTranslation("ar", "NotEmptyValidator", "حقل '{PropertyName}' مطلوب.");
//        AddTranslation("ar", "NotNullValidator", "حقل '{PropertyName}' مطلوب.");
//        AddTranslation("ar", "EmailValidator", "'{PropertyName}' ليس بريد إلكتروني صالح.");
//        AddTranslation("ar", "MaximumLengthValidator",
//            "يجب ألا يتجاوز '{PropertyName}' {MaxLength} حرفًا. لقد أدخلت {TotalLength} حرفًا.");
//        AddTranslation("ar", "MinimumLengthValidator",
//            "يجب أن يكون '{PropertyName}' على الأقل {MinLength} حرفًا. لقد أدخلت {TotalLength} حرفًا.");
//        AddTranslation("ar", "LengthValidator",
//            "يجب أن يكون '{PropertyName}' بين {MinLength} و {MaxLength} حرفًا.");
//        AddTranslation("ar", "GreaterThanValidator",
//            "يجب أن يكون '{PropertyName}' أكبر من {ComparisonValue}.");
//        AddTranslation("ar", "GreaterThanOrEqualValidator",
//            "يجب أن يكون '{PropertyName}' أكبر من أو يساوي {ComparisonValue}.");
//        AddTranslation("ar", "LessThanValidator",
//            "يجب أن يكون '{PropertyName}' أقل من {ComparisonValue}.");
//        AddTranslation("ar", "LessThanOrEqualValidator",
//            "يجب أن يكون '{PropertyName}' أقل من أو يساوي {ComparisonValue}.");
//        AddTranslation("ar", "RegularExpressionValidator",
//            "'{PropertyName}' بتنسيق غير صحيح.");
//        AddTranslation("ar", "EqualValidator",
//            "يجب أن يكون '{PropertyName}' مساويًا لـ '{ComparisonValue}'.");
//        AddTranslation("ar", "ExactLengthValidator",
//            "يجب أن يكون '{PropertyName}' بطول {MaxLength} حرفًا. لقد أدخلت {TotalLength} حرفًا.");
//        AddTranslation("ar", "InclusiveBetweenValidator",
//            "يجب أن يكون '{PropertyName}' بين {From} و {To}.");
//        AddTranslation("ar", "ExclusiveBetweenValidator",
//            "يجب أن يكون '{PropertyName}' بين {From} و {To} (حصريًا).");
//        AddTranslation("ar", "CreditCardValidator",
//            "'{PropertyName}' ليس رقم بطاقة ائتمان صالح.");
//        AddTranslation("ar", "NotEqualValidator",
//            "يجب ألا يكون '{PropertyName}' مساويًا لـ '{ComparisonValue}'.");
//        AddTranslation("ar", "PredicateValidator",
//            "الشرط المحدد غير مستوفى لـ '{PropertyName}'.");
//    }
//}

// Infrastructure/Services/AzureBlobStorageService.cs
//public class AzureBlobStorageService : IFileStorageService
//{
//    private readonly BlobContainerClient _containerClient;

//    public AzureBlobStorageService(IConfiguration config)
//    {
//        var connectionString = config["AzureStorage:ConnectionString"];
//        var containerName = config["AzureStorage:ContainerName"] ?? "product-images";
//        _containerClient = new BlobContainerClient(connectionString, containerName);
//        _containerClient.CreateIfNotExists();
//    }

//    public async Task<string> UploadFileAsync(
//        Stream fileStream, string fileName, string contentType,
//        CancellationToken cancellationToken = default)
//    {
//        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
//        var blobClient = _containerClient.GetBlobClient(blobName);

//        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders
//        {
//            ContentType = contentType
//        }, cancellationToken: cancellationToken);

//        return blobClient.Uri.ToString();
//    }

//    public async Task DeleteFileAsync(string fileUrl,
//        CancellationToken cancellationToken = default)
//    {
//        var blobName = Path.GetFileName(new Uri(fileUrl).LocalPath);
//        var blobClient = _containerClient.GetBlobClient(blobName);
//        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
//    }
//}