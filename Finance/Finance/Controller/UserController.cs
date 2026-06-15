using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.SDK.Utils;
using Finance.Account.Service;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Finance.Controller
{
    public class UserController : FinanceController
    {
        private UserService service;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            service = UserService.GetInstance(GetProperties());
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public FinanceResponse Login([FromBody] UserRequest request)
        {
            long userId = 0;
            var userName = request.UserName;
            var password = CryptInfoHelper.GetDecrypte(request.PassWord);
            password = CryptInfoHelper.MD5Encode(password);

            userId = service.UserVerification(userName, password);
            if (userId == 0)
                throw new FinanceException(FinanceResult.AUTHENTICATION_ERROR);

            var tid = request.Tid;
            var token = FinanceAuthMiddleware.CreateToken(userId, userName, tid, DateTime.Now);
            return new UserResponse { UserId = userId, UserName = userName, Token = token };
        }

        [HttpPost]
        public FinanceResponse Heartbeat([FromBody] HeartBeatRequest request)
        {
            var lastTimeStamp = request.LastTimeStamp;
            var lst = service.GetNeedRefreshTimeStampArticles(lastTimeStamp);
            var taskList = new List<long>();
            if (lst.Contains((int)TimeStampArticleEnum.AccountSubject))
                taskList.Add((long)HeartBeatTask.RefreshAccountSubjectTask);
            if (lst.Contains((int)TimeStampArticleEnum.UserList))
                taskList.Add((long)HeartBeatTask.RefreshUserListTask);
            if (lst.Contains((int)TimeStampArticleEnum.Auxiliary))
                taskList.Add((long)HeartBeatTask.RefreshAuxiliaryTask);
            return new HeartBeatResponse { TimeStamp = CommonUtils.TotalMilliseconds(), TaskList = taskList };
        }

        [HttpPost]
        public IdResponse Save([FromBody] UserSaveRequest request)
        {
            var id = request.Id;
            var userName = request.UserName;
            var password = CryptInfoHelper.GetDecrypte(request.PassWord);
            password = CryptInfoHelper.MD5Encode(password);
            if (id == 0)
                service.AddUser(userName, password);
            else
                service.ModifyUser(id, userName, password);

            return new IdResponse { id = id };
        }

        [HttpPost]
        public FinanceResponse Enable([FromBody] UserEnableRequest request)
        {
            service.Enable(request.Id, request.IsDeleted);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        [HttpPost]
        public UserListResponse List([FromBody] UserListRequest request)
        {
            var lst = service.List();
            return new UserListResponse { Content = lst };
        }

        [HttpPost]
        public FinanceResponse ChangePassword([FromBody] UserChangePasswordRequest request)
        {
            service.ChagePassword(request.Id,
                CryptInfoHelper.MD5Encode(CryptInfoHelper.GetDecrypte(request.OldPwd)),
                CryptInfoHelper.MD5Encode(CryptInfoHelper.GetDecrypte(request.NewPwd)));
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
