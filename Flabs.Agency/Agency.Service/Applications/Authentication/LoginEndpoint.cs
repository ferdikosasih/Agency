using FastEndpoints;

namespace Agency.Service.Authentication;

public class LoginEndpoint(IUserService userService, ITokenService tokenService) : 
    Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/login");
        Description(builder =>
        {
            builder
                .Produces<LoginResponse>(200)
                .Produces(400)
                .Produces(401)
                .Produces(500);
        });
        Summary(s =>
        {
            s.Summary = "Login Agency System.";
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await userService.ValidateUserAsync(req.UserId, req.Password);
        if (user is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        var token = tokenService.GenerateToken(user.Id,user.Name,user.Role);
        await SendAsync(new()
        {
            AccessToken = token
        }, cancellation: ct);
    }
}