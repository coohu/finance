using Finance.Account.SDK;
using Finance.Account.SDK.Response;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Finance
{
    [ApiController]
    [Route("[controller]/[action]/{id?}")]
    public class FinanceController : ControllerBase
    {
        protected IDictionary<string, object> GetProperties()
            => HttpContext.Items["_financeProps"] as IDictionary<string, object>
               ?? new Dictionary<string, object>();

        public FinanceResponse CreateResponse(FinanceResult result)
        {
            return new FinanceResponse { Result = (int)result };
        }

        public IdResponse CreateIdResponse(long id)
        {
            return new IdResponse { Result = (int)FinanceResult.SUCCESS, id = id };
        }

        public FinanceResponse CreateResponse(FinanceException ex)
        {
            return new FinanceResponse { Result = ex.HResult, ErrMsg = ex.Message };
        }
    }
}
