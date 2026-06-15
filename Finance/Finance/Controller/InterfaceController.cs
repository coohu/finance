using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance.Controller
{
    public class InterfaceController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(InterfaceController));
        private InterfaceService service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            service = InterfaceService.GetInstance(GetProperties());
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse ExecTask([FromBody] InterfaceRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var result = service.ExecTask((ExecTaskType)request.TaskType, request.ProcName, request.Filter);
            return new InterfaceResponse { Content = result };
        }

        [HttpPost]
        public FinanceResponse GetResult([FromBody] InterfaceGetResultRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var result = service.GetResult(request.TaskId);
            return new TaskResultResponse { Content = result };
        }
    }
}
