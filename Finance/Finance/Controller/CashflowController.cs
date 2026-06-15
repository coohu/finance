using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Finance.Controller
{
    public class CashflowController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(CashflowController));
        private CashflowSevice service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            service = CashflowSevice.GetInstance(GetProperties());
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public CashflowSheetResponse ListSheet([FromBody] CashflowSheetRequest request)
        {
            Dictionary<string, string> filter = request.filter;
            var lst = service.ListSheet(filter);
            return new CashflowSheetResponse { Content = lst };
        }

        [HttpPost]
        public IActionResult Export([FromBody] CashflowSheetExportRequest request)
        {
            var exportor = new ExcelExportor(new CashflowExportHandler());
            Dictionary<string, string> filter = request.filter;
            var lst = service.ListSheet(filter);
            var dt = EntityConvertor<CashflowSheetItem>.ToDataTable(lst);

            var ms = new MemoryStream();
            exportor.Export(ms, dt, ".xls");

            string sPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");
            if (!Directory.Exists(sPath))
                Directory.CreateDirectory(sPath);

            string fileName = SerialNoService.GetUUID() + ".xls";
            string filePath = Path.Combine(sPath, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            ms.Close();
            ms.Dispose();

            //System.IO.File.Delete(filePath);

            var stream = new FileStream(filePath, FileMode.Open);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        private class CashflowExportHandler : IExportHandler
        {
            public void Encode(ref DataTable data)
            {
                data.Columns.Remove("_Flag");
                data.Columns["_Name"].Caption = "项目";
                data.Columns["_LineNo"].Caption = "行号";
                data.Columns["_Amount"].Caption = "金额";
            }
        }
    }
}
