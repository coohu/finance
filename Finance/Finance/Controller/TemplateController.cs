using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Account.Source;
using Finance.Account.Source.DTL;
using Finance.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Finance.Controller
{
    public class TemplateController : FinanceController
    {
        private string Tid = "0";
        private TemplateSevice service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var props = GetProperties();
            Tid = props.ContainsKey("Tid") ? props["Tid"].ToString() : "0";
            service = TemplateSevice.GetInstance(props);
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public ExcelTemplateResponse GetExcelTemplate([FromBody] ExcelTemplateRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            return new ExcelTemplateResponse { Content = service.FindTemplate(request.name) };
        }

        [HttpPost]
        public UdefTemplateResponse GetUdefTemplate([FromBody] UdefTemplateRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            return new UdefTemplateResponse { Content = service.FindUdefTemplate(request.name) };
        }

        [HttpPost]
        public FinanceResponse SaveUdeftemplate([FromBody] UdefTemplateSaveRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            service.SaveUdeftemplate(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse DeleteUdeftemplate([FromBody] UdefTemplateSaveRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            service.DeleteUdeftemplate(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse ListCarriedForwardTempate([FromBody] CarriedForwardTemplateRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            return new CarriedForwardTemplateResponse { Content = service.ListCarriedForwardTemplate(request.id) };
        }

        [HttpPost]
        public FinanceResponse SaveCarriedForwardTemplate([FromBody] CarriedForwardTemplateSaveRequest request)
        {
            if (request == null || request.Content == null || request.Content.Count == 0)
                throw new FinanceException(FinanceResult.NULL);
            service.SaveCarriedForwardTemplate(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public async Task<FinanceResponse> Upload()
        {
            try
            {
                string sPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".Cache");
                string path = Path.Combine(sPath, SerialNoService.GetUUID() + ".xls");
                using (var fs = new FileStream(path, FileMode.Append))
                {
                    await HttpContext.Request.Body.CopyToAsync(fs);
                }

                string name = HttpContext.Request.Query["name"];

                IImportHandler dtl = null;
                switch (name)
                {
                    case "BalanceSheet":
                        dtl = new BalanceSheetDTL();
                        break;
                    case "ProfitSheet":
                        dtl = new ProfitSheetDTL();
                        break;
                }
                if (dtl == null)
                    return CreateResponse(FinanceResult.SYSTEM_ERROR);

                dtl.SetFileName(path);
                var importor = new ExcelImportor(long.Parse(Tid), dtl);
                importor.Import();

                return CreateResponse(FinanceResult.SUCCESS);
            }
            catch
            {
                return CreateResponse(FinanceResult.SYSTEM_ERROR);
            }
        }
    }
}
