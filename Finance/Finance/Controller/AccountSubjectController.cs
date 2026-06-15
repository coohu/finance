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
    public class AccountSubjectController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(AccountSubjectController));
        private AccountSubjectService service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            service = AccountSubjectService.GetInstance(GetProperties());
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse List([FromBody] AccountSubjectListRequest request)
        {
            List<AccountSubject> lst = service.List(request.Status);
            return new AccountSubjectListResponse { Content = lst };
        }

        [HttpPost]
        public AccountSubjectResponse Find(long id)
        {
            var aso = service.Find(id);
            return new AccountSubjectResponse { Content = aso };
        }

        [HttpPost]
        public FinanceResponse Save([FromBody] AccountSubjectSaveRequest request)
        {
            service.Save(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse Delete(long id)
        {
            service.Delete(id);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse SetStatus([FromBody] AccountSubjectSetStatusRequest request)
        {
            service.SetStatus(request.id, request.Status);
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
