using Loan.Api.Model;
using Serilog;

namespace Loan.Api.Middleware
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionLoggingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Log ფაილში შენახვა
            Log.Error("Unhandled exception occurred | Path: {Path} | Method: {Method} | StatusCode: {StatusCode} | Message: {Message}",
                context.Request.Path,
                context.Request.Method,
                context.Response.StatusCode,
                ex.Message);

            context.Response.ContentType = "application/json";

            // იუზერ ფრენდლი ერრორ მოდელის დაბრნუნება
            ErrorModel response = new ErrorModel
            {
                Success = false,
                Error = "Something went wrong!",
                Details = ex.Message
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
