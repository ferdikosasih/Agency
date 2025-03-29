using FastEndpoints.Security;

namespace Agency.Service.Configurations;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services
        , IConfiguration config)
    {
        services.AddAuthenticationJwtBearer(opt =>
        {
            opt.SigningKey = GetSigningKey();
            opt.KeyIsPemEncoded = true;
            opt.SigningStyle = TokenSigningStyle.Asymmetric;
        });
        services.AddAuthorization();
        return services;
    }

    public static IApplicationBuilder UseSecurity(this IApplicationBuilder app)
    {
        app
            .UseAuthentication()
            .UseAuthorization();
        return app;
    }
    private static string GetSigningKey()
    {
        return File.ReadAllText("public-key.pem");
    }
}