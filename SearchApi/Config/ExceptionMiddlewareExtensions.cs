using System;
using System.Net;
using System.Threading.Tasks;
using SearchApi.Models.Error;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SearchApi.Config
{
    //예외처리 Type A:미사용
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            status_code = context.Response.StatusCode,
                            message = "Internal Server Error."
                        }.ToString());
                    }
                });
            });
        }

        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }

    //예외처리 Type B:사용
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (CustomException ex)
            {
                if (ex.errorDetails.error_code < (int)ApiErrorCode.InfoMax)
                {
                    //예상가능 하며 발생할수 있는 케이스
                    _logger.LogInformation($"CustomException : {ex.errorDetails.error_code} Message : {ex.errorDetails.message}");
                }
                else if (ex.errorDetails.error_code < (int)ApiErrorCode.WarnMax)
                {
                    //예상가능 에러이나 아주 많이 발생하면 안되는 에러
                    _logger.LogWarning($"CustomException : {ex.errorDetails.error_code} Message : {ex.errorDetails.message}");
                }
                else
                {
                    //예상가능 에러이나 발생하면 안되는 에러
                    _logger.LogError($"CustomException : {ex.errorDetails.error_code} Message : {ex.errorDetails.message}");
                }
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                //예측하지 못한 에러 : 확인하면 개발코드수정
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, CustomException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.errorDetails.status_code;

            return context.Response.WriteAsync(new ErrorDetails()
            {
                status_code = context.Response.StatusCode,
                error_code = exception.errorDetails.error_code,
                message = exception.errorDetails.message
            }.ToString());
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(new ErrorDetails()
            {
                status_code = context.Response.StatusCode,
                error_code = 0,
                message = $"Internal Server Error : {exception.Message}"
            }.ToString());
        }
    }
}
