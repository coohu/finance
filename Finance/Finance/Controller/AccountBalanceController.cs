using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Finance.Controller
{
    public class AccountBalanceController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(AccountBalanceController));
        private AccountBalanceService service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            service = AccountBalanceService.GetInstance(GetProperties());
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse Query([FromBody] AccountBalanceRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);

            var beginYear = request.BeginYear;
            var beginPeriod = request.BeginPeriod;
            var endYear = request.EndYear;
            var endPeriod = request.EndPeriod;

            var prevPeriod = CommonUtils.CalcPrevPeriod(new PeridStrunct { Year = beginYear, Period = beginPeriod });
            List<AccountAmountItem> lstBegin = service.QuerySettled(prevPeriod.Year, prevPeriod.Period);
            List<AccountAmountItem> lstCurrent = service.QueryOccurs(beginYear, beginPeriod, endYear, endPeriod);

            return new AccountBalanceResponse { ListBeginBalnace = lstBegin, ListCurrentOccurs = lstCurrent };
        }

        [HttpPost]
        public FinanceResponse Settle()
        {
            service.Settle();
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
