
using EcommerceApplication.Common.Settings;
using EcommerceApplication.DTOs;
using EcommerceApplication.Features.Auth.Dtos;
using EcommerceApplication.Interfaces;
using EcommerceDomain.Entities;
using MediatR;




namespace EcommerceApplication.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IIdentityService _identityService;


        public LoginCommandHandler(
      IIdentityService identityService
            //,
            //IAuthenticationService authenticationService,
            //IOptions<JwtSettings> jwtSettings
            )
        {
            _identityService = identityService;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var loginDto=new LoginDto(request.Email, request.Password); 
         var result=   await  _identityService.LoginAsync(loginDto);
            return result;
        }
    }

    //public class LoginCommandHandler
    //: IRequestHandler<LoginCommand, Result<LoginResponse>>
    //{
    //    private readonly IIdentityService _identityService;

    //    public LoginCommandHandler(IIdentityService identityService)
    //    {
    //        _identityService = identityService;
    //    }

    //    public async Task<Result<LoginResponse>> Handle(
    //        LoginCommand request, CancellationToken cancellationToken)
    //    {
    //        return await _identityService.LoginAsync(
    //            new LoginDto(request.Email, request.Password));
    //    }
    //}

}
