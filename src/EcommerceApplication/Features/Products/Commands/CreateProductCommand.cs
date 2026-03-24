
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Products.DTOs;
using MediaRTutorialApplication.DTOs;
using MediaRTutorialDomain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MediaRTutorialApplication.Features.Products.Commands
{
    public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    
    ImageUploadData? Image,
    Guid CategoryId
       
) : IRequest<Result<string>>;

    // A clean DTO to decouple from IFormFile (which is an API concern)
    public  record ImageUploadData(
        Stream Stream,
        string FileName,
        string ContentType
    );
}
