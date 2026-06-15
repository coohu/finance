using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Controller
{
    public class BalanceSheetController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(BalanceSheetController));

        [HttpPost]
        public FinanceResponse Calc([FromBody] BalanceSheetCalcRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var dict = BalanceSheetService.GetInstance(GetProperties()).Calc(request.filter, request.template);
            return new BalanceSheetCalcResponse { Content = dict };
        }
    }
}
