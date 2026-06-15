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
    public class AuxiliaryController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(AuxiliaryController));

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse List([FromBody] AuxiliaryListRequest request)
        {
            var filter = new Auxiliary();
            if (request.Type != 0)
                filter.type = request.Type;
            return new AuxiliaryListResponse { Content = DataManager.GetInstance(GetProperties()).Query(filter) };
        }

        [HttpPost]
        public FinanceResponse Find(long id)
        {
            //Auxiliary auxiliary= DataManager.GetInstance(GetProperties()).Find<Auxiliary>(id);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse Save([FromBody] AuxiliarySaveRequest json)
        {
            var aux = json.Content;
            var props = GetProperties();

            if (aux.id != 0)
            {
                DataManager.GetInstance(props).Update(aux);
            }
            else
            {
                aux.id = SerialNoService.GetInstance(props).Get(SerialNoKey.System);
                DataManager.GetInstance(props).Insert(aux);
                SerialNoService.GetInstance(props).Update(SerialNoKey.System);
            }
            UserService.GetInstance(props).UpdateTimeStampArticle(TimeStampArticleEnum.Auxiliary);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse Delete(long id)
        {
            var props = GetProperties();
            DataManager.GetInstance(props).Delete<Auxiliary>(id);
            UserService.GetInstance(props).UpdateTimeStampArticle(TimeStampArticleEnum.Auxiliary);
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
