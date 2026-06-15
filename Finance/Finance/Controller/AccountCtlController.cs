using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Account.Source;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance.Controller
{
    public class AccountCtlController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(AccountCtlController));
        private AccountCtlService service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            service = AccountCtlService.GetInstance();
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse List()
        {
            System.Threading.Thread.Sleep(20);
            var dt = DBHelper.DefaultInstance.ExecuteDt("select _id, _no, _name from _AccountCtl order by _id");
            var lst = EntityConvertor<SampleItem>.ToList(dt);
            return new SampleItemListResponse { Content = lst };
        }

        [HttpPost]
        public FinanceResponse Manage([FromBody] AccountCtlManageRequest request)
        {
            Logger.RestLogger();
            string rsp = "";
            Logger.HookLogger((LogLevel level, string message) =>
            {
                if (LogLevel.LevInfo != level)
                    return;
                rsp += message;
            });
            int ret = CommondHandler.Process(request.Params);
            Logger.RestLogger();
            return new FinanceResponse { Result = ret, ErrMsg = rsp };
        }
    }
}
