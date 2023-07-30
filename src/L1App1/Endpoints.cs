using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace L1App1;

public class Endpoints
{
    public static async Task<Results<Ok<string>, ProblemHttpResult>> SecuredEndpoint(
        [FromRoute]int id,
        [FromServices]IHttpContextAccessor contextAccessor,
        [FromServices]ILogger<Endpoints> logger)
    {
        try
        {
            await Task.CompletedTask;
            var context = contextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null");
            //todo if context.User.Identity NOT have permission to view object with id: {id} then return 403 Forbidden
            var userName = context.User.Identity?.Name;
            return TypedResults.Ok($"Hello {userName} here is id: {id}");
        }
        catch (Exception ex)
        {
            var guid = Guid.NewGuid();
            logger.LogError(ex, "Unexpected error {guid}", guid);
            return TypedResults.Problem(detail: $"Something went unexpectedly wrong. Reference: {guid}", statusCode: 500);
        }
    }
}
