using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance.Controller
{
    public class ProfitSheetController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(BalanceSheetController));
        private ProfitSheetService service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            service = ProfitSheetService.GetInstance(GetProperties());
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse Calc([FromBody] ProfitSheetCalcRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var dict = service.Calc(request.filter, request.template);
            return new ProfitSheetCalcResponse { Content = dict };
        }
    }
}
