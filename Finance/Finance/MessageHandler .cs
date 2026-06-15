using Finance.Utils;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Finance
{
    /// <summary>
    /// 消息处理程序
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger = Logger.GetLogger(typeof(LoggingMiddleware));

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 记录请求内容
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            _logger.Info(string.Format("请求Content:{0}\t{1}", context.Request.Path, requestBody));

            var originalBody = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // 发送HTTP请求到内部处理程序，在异步处理完成后记录响应内容
            await _next(context);

            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            _logger.Info(string.Format("响应\tStatusCode:{0}\tContent:{1}", context.Response.StatusCode, responseText));

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBody);
        }
    }
}
