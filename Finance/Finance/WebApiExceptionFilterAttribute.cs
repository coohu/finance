using Finance.Account.SDK;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance
{
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger = Logger.GetLogger(typeof(WebApiExceptionFilterAttribute));

        //重写基类的异常处理方法
        public override void OnException(ExceptionContext context)
        {
            //1.异常日志记录（正式项目里面一般是用log4net记录异常日志）
            _logger.Error(context.Exception);

            //2.返回调用方具体的异常信息
            if (context.Exception is FinanceException)
            {
                var fex = new FinanceResponse
                {
                    Result = context.Exception.HResult,
                    ErrMsg = context.Exception.Message
                };
                context.Result = new OkObjectResult(fex);
                context.ExceptionHandled = true;
            }

            base.OnException(context);
        }
    }
}
