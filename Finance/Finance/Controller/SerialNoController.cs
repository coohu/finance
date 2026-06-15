using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.Service;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance.Controller
{
    public class SerialNoController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(SerialNoController));
        private SerialNoService service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            service = SerialNoService.GetInstance(GetProperties());
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse Get([FromBody] SerialNoRequest request)
        {
            long id = service.Get((SerialNoKey)request.SerialKey, request.Ex);
            return CreateIdResponse(id);
        }
    }
}
