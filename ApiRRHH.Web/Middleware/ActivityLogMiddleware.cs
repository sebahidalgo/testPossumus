using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiRRHH.Web.Middleware
{
    public class ActivityLogMiddleware
    {
        private readonly ILogger<ActivityLogMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _context;

        private const string _responseMessageTemplate = "scheme:{Scheme} host:{RequestHost} " +
            "headers:{RequestHeaders} method:{RequestMethod} path:{RequestPath} " +
            "queryString:{QueryString} responded {StatusCode} in {Elapsed:0.0000} ms";


        public ActivityLogMiddleware(RequestDelegate next,
            ILogger<ActivityLogMiddleware> logger,
            IHttpContextAccessor context
            )
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
            _context = context;
        }

        public async Task Invoke(HttpContext context)
        {
            var start = Stopwatch.GetTimestamp();

            //First, get the incoming request
            var activityLog = FormatRequest(context.Request);

            var originalResponseBodyStream = context.Response.Body;
            string responseBodyText;
            Exception errorException = null;

            string requestBodyText = await this.ExtractRequestBody(context);

            LogContext.PushProperty(SerilogPropertyNamesIdentifiers.RequestBody, $"[Request body: {requestBodyText}]");

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    errorException = ex;
                    context.Response.ContentType = context.Response.ContentType == null ? "text/html" : context.Response.ContentType + ";text/html";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalResponseBodyStream);
            }

            var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

            UpdateRequestObjectWithResponse(context.Response, activityLog);
            activityLog.BodyResponse = string.IsNullOrWhiteSpace(responseBodyText) ? null : responseBodyText;
            activityLog.Duration = elapsedMs;

            LogContext.PushProperty(SerilogPropertyNamesIdentifiers.ResponseBody, $"[Response body: {  activityLog.BodyResponse}]");

            if (context.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
                _logger.LogError(errorException, errorException?.Message);
            else
                _logger.LogInformation(_responseMessageTemplate,
                    activityLog.Scheme,
                    activityLog.Host,
                    activityLog.HeadersRequest,
                    activityLog.Method,
                    activityLog.Path,
                    activityLog.QueryString,
                    activityLog.StatusCode,
                    activityLog.Duration
                    );
        }

        private double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
        }

        private void UpdateRequestObjectWithResponse(HttpResponse response, ActivityLog activityLog)
        {
            var headers = new StringBuilder();

            foreach (var head in response.Headers)
                headers.Append(string.Concat(head.Key, "=", head.Value, ";"));

            activityLog.HeadersResponse = headers.Length == 0 ? null : headers.ToString();
            activityLog.StatusCode = response.StatusCode;
            activityLog.ContentType = response.ContentType;
        }

        private ActivityLog FormatRequest(HttpRequest request)
        {
            var userName = _context?.HttpContext?.User?.Identity?.Name;

            var headers = new StringBuilder();

            foreach (var head in request.Headers)
                headers.Append(string.Concat(head.Key, "=", head.Value, ";"));

            var queryString = string.IsNullOrWhiteSpace(request.QueryString.ToString()) ? null : request.QueryString.ToString();

            var userAgent = request.Headers["User-Agent"];

            var traceIdentifier = _context?.HttpContext?.TraceIdentifier;

            if (!string.IsNullOrWhiteSpace(userAgent))
                LogContext.PushProperty(SerilogPropertyNamesIdentifiers.UserAgent, $"agent:{userAgent}");

            var requestBodyStream = new MemoryStream();
            request.Body.CopyToAsync(requestBodyStream).GetAwaiter().GetResult();
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            request.Body = requestBodyStream;

            return new ActivityLog()
            {
                //Borrar StatusCode y Duration Al Deployar En Banco y haber cambiado estructura tablas
                StatusCode = 0,
                Duration = 0,
                ContentType = "",
                Id = Guid.NewGuid(),
                UserAgent = userAgent,
                HeadersRequest = headers.Length == 0 ? null : headers.ToString(),
                BodyRequest = requestBodyText == "" ? null : requestBodyText,
                Scheme = request.Scheme,
                Method = request.Method,
                Host = request.Host.ToString(),
                Path = request.Path,
                Date = DateTime.UtcNow,
                User = userName,
                MachineName = Environment.MachineName,
                RemoteIP = request.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                RemotePort = request.HttpContext?.Connection?.RemotePort,
                QueryString = queryString,
                TraceIdentifier = traceIdentifier,
                ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name
            };
        }

        private async Task<string> ExtractRequestBody(HttpContext context)
        {
            string requestBodyText;
            if (context.Request.ContentType == "application/json")
            {
                var request = context.Request;

                request.EnableBuffering();
                var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                await request.Body.ReadAsync(buffer, 0, buffer.Length);
                //get body string here...
                requestBodyText = Encoding.UTF8.GetString(buffer);

                request.Body.Position = 0;  //rewinding the stream to 0
            }
            else
            {
                requestBodyText = context.Request.Body.ToString();
            }
            return requestBodyText;
        }
    }
}
