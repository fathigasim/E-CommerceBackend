
using EcommerceApplication.Common.Settings;
using EcommerceDomain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace MediaRTutorialApplication.Features.Category.Commands
{
    public class CreateCategoryCommandHandler :IRequestHandler<CreateCategoryCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Unit>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {

            var exists = await _unitOfWork.Categories.ItemExistAsync(c => c.Name == request.Name, cancellationToken);
            if (exists)
            {
                return Result<Unit>.Failure("Category already exist ");
            }
                var category = new EcommerceDomain.Entities.Category
            {
                Name = request.Name,
                Description = request.Description
            };
            await _unitOfWork.Categories.AddAsync(category);
          var result=  await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
                return Result<Unit>.Failure("Failed to create category");
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
