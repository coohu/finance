using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance.Controller
{
    public class SystemProfileController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(SystemProfileController));
        private SystemProfileService service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            service = SystemProfileService.GetInstance(GetProperties());
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse Find([FromBody] SystemProfileRequest request)
        {
            var rsp = service.Find((SystemProfileCategory)request.Category, (SystemProfileKey)request.Key);
            return new SystemProfileResponse { Content = rsp };
        }

        [HttpPost]
        public FinanceResponse List()
        {
            var rsp = service.List();
            return new SystemProfileListResponse { Content = rsp };
        }

        [HttpPost]
        public FinanceResponse Udpate([FromBody] SystemProfileUpdateRequest request)
        {
            service.Update((SystemProfileCategory)request.Category, (SystemProfileKey)request.Key, request.Value);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse MenuTables([FromBody] MenuTablesRequest request)
        {
            var rsp = service.MenuTables();
            return new MenuTablesResponse { Content = rsp };
        }

        [HttpPost]
        public FinanceResponse AllMenuTables([FromBody] AllMenuTablesRequest request)
        {
            var rsp = service.AllMenuTables();
            return new MenuTablesResponse { Content = rsp };
        }

        [HttpPost]
        public FinanceResponse SaveMenu([FromBody] MenuTableSaveRequest request)
        {
            service.SaveMenu(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse DeleteMenu([FromBody] MenuTableDeleteRequest request)
        {
            service.DeleteMenu(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public AccessRightResponse AccessRight([FromBody] AccessRightRequest request)
        {
            var rsp = service.GetAccessRightList(request.id);
            return new AccessRightResponse { Content = rsp };
        }

        [HttpPost]
        public FinanceResponse SaveAccessRight([FromBody] AccessRightSaveRequest request)
        {
            service.SaveAccessRight(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
