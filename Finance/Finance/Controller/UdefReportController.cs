using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance.Controller
{
    public class UdefReportController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(UdefReportController));
        private UdefReportService service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            service = UdefReportService.GetInstance(GetProperties());
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse Query([FromBody] UdefReportRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var result = service.Query(request.ProcName, request.Filter);
            if (result == null)
                throw new FinanceException(FinanceResult.SYSTEM_ERROR);
            return new UdefReportResponse { Content = result };
        }
    }
}
