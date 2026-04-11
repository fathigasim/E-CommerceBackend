using AutoMapper;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.DTOs;
using EcommerceDomain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRTutorialApplication.Features.Category.Queries
{
    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, Result<List<CategoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetCategoryQueryHandler(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public  async Task<Result<List<CategoryDto>>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetAllAsync();
             var categoryDtos = _mapper.Map<List<CategoryDto>>(category);
             return Result<List<CategoryDto>>.Success(categoryDtos);
            
        }
    }
}
