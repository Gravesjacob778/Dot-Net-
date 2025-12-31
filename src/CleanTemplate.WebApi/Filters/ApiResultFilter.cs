using CleanTemplate.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanTemplate.WebApi.Filters;

public sealed class ApiResultFilter : IAsyncResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        switch (context.Result)
        {
            case NotFoundResult:
                context.Result = new NotFoundObjectResult(HttpActionResponse.Fail("Resource not found."));
                break;
            case NotFoundObjectResult notFound when notFound.Value is not HttpActionResponse:
                context.Result = new NotFoundObjectResult(HttpActionResponse.Fail("Resource not found."));
                break;
            case BadRequestObjectResult badRequest when badRequest.Value is ValidationProblemDetails validationDetails:
                context.Result = new BadRequestObjectResult(new HttpActionResponse(false, "Validation failed.", validationDetails.Errors));
                break;
            case BadRequestObjectResult badRequest when badRequest.Value is SerializableError serializableError:
                context.Result = new BadRequestObjectResult(new HttpActionResponse(false, "Validation failed.", serializableError));
                break;
        }

        return next();
    }
}
