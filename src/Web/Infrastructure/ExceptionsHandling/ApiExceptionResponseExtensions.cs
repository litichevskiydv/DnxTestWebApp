namespace Web.Infrastructure.ExceptionsHandling
{
    using System;
    using Microsoft.AspNet.Builder;

    public static class ApiExceptionResponseExtensions
    {
        public static IApplicationBuilder UseApiExceptionResponse(this IApplicationBuilder builder, bool showExceptionDetails)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            return builder.UseMiddleware<ApiExceptionResponseMiddleware>(showExceptionDetails);
        }
    }
}