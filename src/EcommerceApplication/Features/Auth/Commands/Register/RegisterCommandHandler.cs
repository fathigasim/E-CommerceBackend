
using EcommerceApplication.Features.Auth.Notifications.RegisterNotification;
using EcommerceDomain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace EcommerceApplication.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;
        public RegisterCommandHandler(UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new RegisterResponse
                {
                    Succeeded = false,
                    Errors = new List<string> { "User with this email already exists" }
                };
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateCreated = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _mediator.Publish(new UserRegisteredNotification(request.Email, request.UserName));
                // Optionally add default role
                await _userManager.AddToRoleAsync(user, "User");

                return new RegisterResponse
                {
                    Succeeded = true,
                    UserId = user.Id,
                    Email = user.Email
                };
            }

            return new RegisterResponse
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
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


