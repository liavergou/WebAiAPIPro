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
        //αν ο controller δώσει exception θα επιστραφεί στον handler
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
                    ExceptionType = exception.GetType().Name, //όνομα του exception
                    EndPoint = context.Request.Path, //ποιο endpoint κλήθηκε
                    Method = context.Request.Method, //
                    User = context.User.Identity?.Name ?? "Anonymous", //principal
                    UserAgent = context.Request.Headers.UserAgent.ToString(), //o agent είναι ο browser
                    TraceId = context.TraceIdentifier //μοναδικό id που μοναδικοποιεί τα request
                };

                logger.LogError("{ExceptionType} at {Endpoint} {Method} by {User} | Trace={TraceId}",
                    logContext.ExceptionType, logContext.EndPoint, logContext.Method, logContext.User, logContext.TraceId);

                var response = context.Response;
                response.ContentType = "application/json"; //δηλωνουμε ότι στέλνουμε json πισω

                response.StatusCode = exception switch
                {                    
                    EntityAlreadyExistsException => (int)HttpStatusCode.BadRequest, // 400
                    EntityNotAuthorizedException => (int)HttpStatusCode.Unauthorized,    // 401
                    EntityForbiddenException => (int)HttpStatusCode.Forbidden,          // 403
                    EntityNotFoundException => (int)HttpStatusCode.NotFound,        // 404
                    InvalidArgumentException => (int)HttpStatusCode.BadRequest, //400
                    DeletionForbiddenException => (int)HttpStatusCode.Forbidden, //403
                    ServerException => (int)HttpStatusCode.InternalServerError,
                    KeycloakException => (int)HttpStatusCode.Unauthorized, //401
                    _ => (int)HttpStatusCode.InternalServerError,              // 500    
                };
                //new {} είναι ανώνυμο object
                var result = System.Text.Json.JsonSerializer.Serialize(new { code = response.StatusCode, message = exception?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
