using L1App1;
using L1App1.Authentications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("DevBearer", new OpenApiSecurityScheme
    {
        Description = "Dev bearer token",
        Type = SecuritySchemeType.ApiKey,
        Name = "DevBearer",
        In = ParameterLocation.Header,
        Scheme = "DevBearer",
    });
    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "DevBearer"
        },
        In = ParameterLocation.Header,
    };
    var requirement = new OpenApiSecurityRequirement
    {
        { scheme, new List<string>() }
    };
    options.AddSecurityRequirement(requirement);
});

builder.Services.AddHttpContextAccessor();

//todo Add secrets from vault

builder.Services.AddAuthentication(options =>
{
    if (builder.Environment.IsDevelopment())
    {
         options.AddScheme<DevBearerAuthenticationSchemeHandler>("DevBearer", "DevBearer");
    }

    //todo Add authentication schemes for production
});

builder.Services.AddAuthorization(options =>
{
    var alwaysDenyPolicy = new AuthorizationPolicyBuilder()
        .RequireAssertion(_ => false)
        .Build();
    options.AddPolicy("AlwaysDeny", alwaysDenyPolicy);
    options.DefaultPolicy = alwaysDenyPolicy;
    options.FallbackPolicy = alwaysDenyPolicy;

    options.AddPolicy("User", policy => policy.RequireRole("User"));
});

/*
 * Build
 */
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!")
   .WithTags("Hello")
   .WithOpenApi()
   .AllowAnonymous();
   ;

app.MapGet("Secured/{id}", Endpoints.SecuredEndpoint)
   .WithTags("Secured")
   .RequireAuthorization("User")
   .WithOpenApi();

app.Run();
