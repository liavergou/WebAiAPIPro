using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Exceptions.keycloak;
using Serilog;
using System.Net;

namespace CoordExtractorApp.Helpers
{
    /// <summary>
    /// Global exception handling middleware.
    /// Catches all unhandled exceptions, logs them with detailed context, and returns a consistent JSON error response.
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly ILogger<ErrorHandlerMiddleware> logger =
            new LoggerFactory().AddSerilog().CreateLogger<ErrorHandlerMiddleware>();

        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the ErrorHandlerMiddleware class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invokes the middleware operation.
        /// Wraps the request execution in a try-catch block to handle exceptions.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                var logContext = new
                {
                    ExceptionType = exception.GetType().Name,
                    EndPoint = context.Request.Path,
                    Method = context.Request.Method,
                    User = context.User.Identity?.Name ?? "Anonymous",
                    UserAgent = context.Request.Headers.UserAgent.ToString(),
                    TraceId = context.TraceIdentifier
                };

                logger.LogError("{ExceptionType} at {Endpoint} {Method} by {User} | Trace={TraceId}",
                    logContext.ExceptionType, logContext.EndPoint, logContext.Method, logContext.User, logContext.TraceId);

                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = exception switch
                {
                    EntityAlreadyExistsException => (int)HttpStatusCode.BadRequest,
                    EntityNotAuthorizedException => (int)HttpStatusCode.Unauthorized,
                    EntityForbiddenException => (int)HttpStatusCode.Forbidden,
                    EntityNotFoundException => (int)HttpStatusCode.NotFound,
                    InvalidArgumentException => (int)HttpStatusCode.BadRequest,
                    DeletionForbiddenException => (int)HttpStatusCode.Forbidden,
                    ServerException => (int)HttpStatusCode.InternalServerError,
                    KeycloakException => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.InternalServerError,
                };

                var result = System.Text.Json.JsonSerializer.Serialize(new { code = response.StatusCode, message = exception?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
