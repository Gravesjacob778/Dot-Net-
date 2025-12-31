using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanTemplate.WebApi.Filters;

public sealed class ApiActionLoggingFilter(ILogger<ApiActionLoggingFilter> logger) : IAsyncActionFilter
{
    private readonly ILogger<ApiActionLoggingFilter> _logger = logger;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var actionName = context.ActionDescriptor.DisplayName ?? "unknown";
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Action executing {Method} {Path} {Action} {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            actionName,
            httpContext.TraceIdentifier);

        var executedContext = await next();

        stopwatch.Stop();

        if (executedContext.Exception is not null && !executedContext.ExceptionHandled)
        {
            _logger.LogWarning(executedContext.Exception,
                "Action failed {Method} {Path} {Action} {ElapsedMs}ms {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                actionName,
                stopwatch.Elapsed.TotalMilliseconds,
                httpContext.TraceIdentifier);
            return;
        }

        _logger.LogInformation("Action executed {Method} {Path} {Action} {ElapsedMs}ms {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            actionName,
            stopwatch.Elapsed.TotalMilliseconds,
            httpContext.TraceIdentifier);
    }
}
