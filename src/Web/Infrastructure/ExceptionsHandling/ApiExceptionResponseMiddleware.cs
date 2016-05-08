namespace Web.Infrastructure.ExceptionsHandling
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Http;
    using Microsoft.Extensions.Logging;

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ApiExceptionResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _showExceptionDetails;
        private readonly ILogger _logger;
        private readonly DiagnosticSource _diagnosticSource;

        public ApiExceptionResponseMiddleware(RequestDelegate next, bool showExceptionDetails, ILogger<ApiExceptionResponseMiddleware> logger,
            DiagnosticSource diagnosticSource)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (diagnosticSource == null)
                throw new ArgumentNullException(nameof(diagnosticSource));

            _next = next;
            _showExceptionDetails = showExceptionDetails;
            _logger = logger;
            _diagnosticSource = diagnosticSource;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unhandled exception has occurred while executing the request", ex);

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the error page middleware will not be executed.");
                    throw;
                }

                try
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 500;

                    DisplayException(context, ex);

                    if (_diagnosticSource.IsEnabled("Microsoft.AspNetCore.Diagnostics.UnhandledException"))
                        _diagnosticSource.Write("Microsoft.AspNetCore.Diagnostics.UnhandledException", new {httpContext = context, exception = ex});

                    return;
                }
                catch (Exception ex2)
                {
                    _logger.LogError("An exception was thrown attempting to display the error page.", ex2);
                }
                throw;
            }
        }

        private void DisplayException(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            var output = new StreamWriter(context.Response.Body, Encoding.UTF8, 4096, true);
            if (_showExceptionDetails)
            {
                WriteExceptionData(output, exception, 0);
                output.WriteLine("}");
            }
            else
                output.WriteLine("{\"message\": \"An error has occurred.\"}");
            output.Dispose();
        }

        private static void WriteExceptionData(TextWriter output, Exception exception, int level)
        {
            output.WriteLine("{");
            var leadSpace = string.Join("", Enumerable.Repeat("  ", level + 1));
            output.WriteLine(leadSpace + "\"message\": \"An error has occurred.\",");
            output.WriteLine(leadSpace + $"\"exceptionMessage\": \"{exception.Message}\",");
            output.WriteLine(leadSpace + $"\"exceptionType\": \"{exception.GetType()}\",");

            if (string.IsNullOrEmpty(exception.StackTrace))
                output.Write(leadSpace + "\"stackTrace\": null");
            else
            {
                var preparedStackTrace = exception.StackTrace.Replace("\r", "\\r").Replace("\n", "\\n");
                output.Write(leadSpace + $"\"stackTrace\": \"{preparedStackTrace}\"");
            }

            if (exception.InnerException == null)
                output.WriteLine();
            else
            {
                output.WriteLine(",");
                output.Write(leadSpace + "\"innerException\": ");
                WriteExceptionData(output, exception.InnerException, level + 1);
                output.WriteLine(leadSpace + "}");
            }
        }
    }
}