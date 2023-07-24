using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

//todo Add secrets from vault

//todo Add authentication

//todo Add authorization

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

app.MapGet("/", () => "Hello World!")
   .WithName("Hello")
   .WithOpenApi();

app.MapGet("/Secured/{id}", (
    [FromRoute]int id,
    [FromServices]IHttpContextAccessor contextAccessor,
    [FromServices]ILogger<Program> logger) => 
{
    try
    {
        var context = contextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null");
        //todo if context.User.Identity NOT have permission to view object with id: {id} then return 403 Forbidden
        var userName = context.User.Identity?.Name;
        return TypedResults.Ok($"Hello {userName} here is id: {id}");
    }
    catch (Exception ex)
    {
        var guid = Guid.NewGuid();
        logger.LogError(ex, "Unexpected error {@guid}", guid);
        return (IResult)TypedResults.Problem(detail: $"Something went unexpectedly wrong. Reference: {guid}", statusCode: 500);
    }
})
   .WithName("SecuredHello")
//    .RequireAuthorization()
   .WithOpenApi();

app.Run();
