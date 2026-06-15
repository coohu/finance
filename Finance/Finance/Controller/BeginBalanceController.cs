using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance.Controller
{
    public class BeginBalanceController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(BeginBalanceController));
        private BeginBalanceSevice service;
        private LogService logService;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var props = GetProperties();
            service = BeginBalanceSevice.GetInstance(props);
            logService = LogService.GetInstance(props, typeof(BeginBalanceController));
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse Save([FromBody] BeginBalanceSaveRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            service.Save(request.Content);
            logService.Write(Operation.Update, "");
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse List()
        {
            var lst = service.List();
            return new BeginBalanceListResponse { Content = lst };
        }

        [HttpPost]
        public FinanceResponse Finish()
        {
            service.Finish();
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
