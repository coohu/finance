using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Account.Source;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System.Threading;

namespace Finance.Controller
{
    public class VoucherController : FinanceController
    {
        private readonly ILogger logger = Logger.GetLogger(typeof(VoucherController));
        private VoucherService service;
        private LogService logService;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var props = GetProperties();
            service = new VoucherService(props);
            logService = new LogService(props, typeof(VoucherController));
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse List([FromBody] VoucherListRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            return new VoucherListResponse { Content = service.List(request.Filter) };
        }

        [HttpPost]
        public FinanceResponse Find([FromBody] VoucherRequest request)
        {
            var voucher = new Voucher();
            var id = service.Linked(request.id, request.Linked);
            if (id == 0)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            voucher.header = service.FindHeader(id);
            voucher.entries = service.FindEntrys(id);
            voucher.udefenties = service.FindUdefEntrys(id);
            return new VoucherResponse { Content = voucher };
        }

        [HttpPost]
        public FinanceResponse Save([FromBody] VoucherSaveRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var voucher = request.Content;
            var id = voucher.header.id;
            if (id == 0)
            {
                id = service.Add(voucher);
                logService.Write(Operation.Add, voucher.header.word + "-" + voucher.header.no);
            }
            else
            {
                service.Update(voucher);
                logService.Write(Operation.Update, voucher.header.word + "-" + voucher.header.no);
            }
            return CreateIdResponse(id);
        }

        [HttpPost]
        public FinanceResponse Delete(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.Delete(id);
            logService.Write(Operation.Delete, header.word + "-" + header.no);
            return CreateIdResponse(id);
        }

        [HttpPost]
        public FinanceResponse Check(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.Check(id);
            logService.Write(Operation.Check, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse UnCheck(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.UnCheck(id);
            logService.Write(Operation.UnCheck, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse Cancel(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.Cancel(id);
            logService.Write(Operation.Cancel, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse UnCancel(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.UnCancel(id);
            logService.Write(Operation.UnCancel, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse Post(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.Post(id);
            logService.Write(Operation.Post, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse UnPost(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.UnPost(id);
            logService.Write(Operation.UnPost, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public FinanceResponse DoTest()
        {
            var uid = SerialNoService.GetUUID();
            int i = 1;
            while (i < 1000)
            {
                logger.Debug(uid + ":" + i);
                Thread.Sleep(10);
                i++;
            }
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public IActionResult Print([FromBody] VoucherPrintRequest request)
        {
            var tmpInfo = new PrintTemplateInfo();
            tmpInfo.name = "凭证打印模板_v1.xlsx";
            tmpInfo.procName = "sp_voucher_print_v1";
            tmpInfo.id = request.id;
            var printAssemble = new PrintAssemble(tmpInfo, service);
            string filePath = printAssemble.Package();

            var stream = new FileStream(filePath, FileMode.Open);
            //System.IO.File.Delete(filePath);
            return File(stream, "application/vnd.ms-excel", request.FileName);
        }
    }
}
