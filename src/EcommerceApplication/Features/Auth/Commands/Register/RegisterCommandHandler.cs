
using EcommerceApplication.Common.Settings;
using EcommerceApplication.DTOs;
using EcommerceApplication.Features.Auth.Notifications.RegisterNotification;
using EcommerceApplication.Interfaces;
using EcommerceDomain.Entities;
using MediaRTutorialApplication.Interfaces;
using MediatR;


namespace EcommerceApplication.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
    {
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        public RegisterCommandHandler(IIdentityService identityService, IMediator mediator)
        {
            _identityService = identityService;
            _mediator = mediator;
        }

        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
               var registerDto = new RegisterDto(request.FirstName,request.LastName,request.Email,request.Password,request.ConfirmPassword);
             var result= await _identityService.RegisterAsync(registerDto);
           await _mediator.Publish(new UserRegisteredNotification(request.Email,request.FirstName),cancellationToken);
            return result;
        }
        
    }

    //public class RegisterCommandHandler
    //: IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    //{
    //    private readonly IIdentityService _identityService;

    //    public RegisterCommandHandler(IIdentityService identityService)
    //    {
    //        _identityService = identityService;
    //    }

    //    public async Task<Result<AuthResponseDto>> Handle(
    //        RegisterCommand request, CancellationToken cancellationToken)
    //    {
    //        var registerDto = new RegisterDto(
    //            request.FirstName,
    //            request.LastName,
    //            request.Email,
    //            request.Password,
    //            request.ConfirmPassword
    //        );

    //        return await _identityService.RegisterAsync(registerDto);
    //    }
}


