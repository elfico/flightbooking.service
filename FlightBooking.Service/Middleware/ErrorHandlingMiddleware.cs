using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System.ComponentModel.DataAnnotations;
using System.Net;
using ILogger = NLog.ILogger;
using LogLevel = NLog.LogLevel;

namespace FlightBooking.Service.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _host;

        public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment host)
        {
            _next = next;
            _host = host;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string loggerName;

            Logger.Log(LogLevel.Info, "Error handing exceptions");
            try
            {
                loggerName = exception.TargetSite!.DeclaringType!.FullName!;
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e);
                loggerName = nameof(GetType);
            }

            var statusCode = (int)HttpStatusCode.InternalServerError;

            var errorMessage = exception.Message;

            if (exception is ValidationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                var logEvent = new LogEventInfo(LogLevel.Error, loggerName, exception.Message)
                {
                    Exception = exception
                };

                logEvent.Properties["Environment"] = _host.EnvironmentName;

                if (_host.IsProduction())
                {
                    errorMessage = "Error handing exceptions";
                }
                Logger.Log(logEvent);
            }

            ProblemDetails response = new ProblemDetails
            {
                Status = statusCode,
                Title = errorMessage,
                Detail = errorMessage
            };

            string result = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(result);
        }
    }
}