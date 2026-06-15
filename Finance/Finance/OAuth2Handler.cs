using Finance.Account.SDK;
using Finance.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Finance
{
    /// <summary>
    /// 鉴权处理
    /// </summary>
    public class FinanceAuthMiddleware
    {
        private static readonly string ResourceServerPrivateKey = "<RSAKeyValue><Modulus>4+59zObnTWbqDk4ULzjVbBg2gMZMC4+i8cUj0nEXD622WZyyzDv1kAy4U0KOs5+iCocXGDq0nKG2ovfHk/AGo36Pm4Ij+aq0qQyMr1wvArSiStBHtGl3WNaid1QkT/87tEIg4dNRAPhUlxF6APwTFnNGp/xOLhAo1rTVnjCS4PE=</Modulus><Exponent>AQAB</Exponent><P>77C1lSG09A5GKAHULA/wewJHoAkhVH2zY7ujEeX/7P29UdQWZUDxjJqSDZbqP+rQQLrzFF01GhcxVdql7gfBTw==</P><Q>83DzpsqgQ3LggKq5i7hdzYUegD8uWNicMA9c5GL+rAXpUce/sh87KO9vbCOwOw4N4sbWXlrbaUCLcVGr+uwpvw==</Q><DP>QacWZa3g4cSTJNwzYIpRJXBfbA90KK9xlozLwthL/H8X/zTnmX5ra0bfYIeIzE8mEcTjVh2dsPLPWaPVNVi8cw==</DP><DQ>dmqgKqbv1D9iE1R4kw1om5tAXfPd0Jv1Ra+DaRj6dqUdfIlkpvloJp5pnbmydNd+S6ybBCTAC++4pLOsq48LMw==</DQ><InverseQ>3vc7yC45UFDkQ4XtgaXJYniCYUJIIyloGGWIWDE0EwJeb7Ux82mgs+M8Yeb+9Tz5rwlhk4Sj6rpROSq03xb+Ew==</InverseQ><D>H9caBbyfxSVCPvtTQIF89tuvCXAqAVdwWLvEVEpuAUev+Ha2V2ds11GfkinzC06acUQLytuwjUzd2YgpfhYCpyKySiza6wMYx/cxwSFZI40SeK3JTIYnS0bmGIuo7ocL/4MMbOY1L0GhUSJgn24bfpBOHRQ1Py2DrE6R3tEGeK0=</D></RSAKeyValue>";
        private static readonly string ResourceServerPublicKey = "<RSAKeyValue><Modulus>4+59zObnTWbqDk4ULzjVbBg2gMZMC4+i8cUj0nEXD622WZyyzDv1kAy4U0KOs5+iCocXGDq0nKG2ovfHk/AGo36Pm4Ij+aq0qQyMr1wvArSiStBHtGl3WNaid1QkT/87tEIg4dNRAPhUlxF6APwTFnNGp/xOLhAo1rTVnjCS4PE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private static readonly List<string> NoAuthMethod = new List<string>
        {
            "/user/login", "/accountctl/list", "/accountctl/manage"
        };
        private static readonly List<string> UploadFileMethod = new List<string>
        {
            "/template/upload"
        };

        private readonly RequestDelegate _next;
        private readonly ILogger _logger = Logger.GetLogger(typeof(FinanceAuthMiddleware));

        public FinanceAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string method = context.Request.Path.Value?.ToLower() ?? "";

            if (UploadFileMethod.Contains(method))
            {
                DecodeQueryPara(context);
                await _next(context);
                return;
            }

            var content = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            try
            {
                ValidSign(context.Request.Query, content);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AUTHENTICATION_ERROR");
                await WriteAuthError(context);
                return;
            }

            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(content);
                var props = new Dictionary<string, object>();

                if (!NoAuthMethod.Contains(method))
                {
                    string token = json["Token"].ToString();
                    string[] user = DecryptToken(token).Split('|');
                    props["UserId"] = user[0];
                    props["UserName"] = user[1];
                    props["Tid"] = user[2];
                }
                else if (method == "/user/login")
                {
                    props["Tid"] = json["Tid"].ToString();
                }

                context.Items["_financeProps"] = props;
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AUTHENTICATION_ERROR");
                await WriteAuthError(context);
            }
        }

        private static void DecodeQueryPara(HttpContext context)
        {
            string token = context.Request.Query["token"];
            string[] user = DecryptToken(token).Split('|');
            context.Items["_financeProps"] = new Dictionary<string, object>
            {
                ["UserId"] = user[0],
                ["UserName"] = user[1],
                ["Tid"] = user[2]
            };
        }

        private static async Task WriteAuthError(HttpContext context)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            var body = JsonConvert.SerializeObject(new FinanceResponse { Result = (int)FinanceResult.AUTHENTICATION_ERROR });
            await context.Response.WriteAsync(body);
        }

        public static string DecryptToken(string token)
        {
            byte[] b64Data = Convert.FromBase64String(token.Replace("k@", "").Replace('_', '+').Replace('-', '/'));
            byte[] data = GetResourceServerRsa().Decrypt(b64Data, RSAEncryptionPadding.OaepSHA1);
            return Encoding.UTF8.GetString(data);
        }

        public static readonly Random Rd = new Random(1);

        public static string CreateToken(long uid, string username, long tid, DateTime liftDate)
        {
            byte[] data = Encoding.UTF8.GetBytes(string.Format("{0}|{1}|{2}|{3}",
                uid, username, tid,
                liftDate.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo),
                Rd.Next(1, 99999)));
            byte[] enData = GetResourceServerRsa().Encrypt(data, RSAEncryptionPadding.OaepSHA1);
            return Convert.ToBase64String(enData, Base64FormattingOptions.None).Replace('+', '_').Replace('/', '-');
        }

        public static RSACryptoServiceProvider GetResourceServerRsa()
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(ResourceServerPrivateKey);
            return rsa;
        }

        private void ValidSign(IQueryCollection query, string content)
        {
            var app = GetApp();
            string sign = query["sign"];

            var sb = new StringBuilder(app.Secret);
            sb.Append(content);
            sb.Append(app.Secret);

            using var md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));

            var result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString("X2"));

            if (sign != result.ToString())
                throw new FinanceException(FinanceResult.AUTHENTICATION_ERROR);
        }

        private AppEntity GetApp()
        {
            return new AppEntity
            {
                AppKey = "FinanceClient",
                Ver = "1.0",
                Secret = "lhKL@ti&KukX5FmB0o5P0TABaXhF8I!T"
            };
        }

        private class AppEntity
        {
            public string AppKey { get; set; }
            public string Ver { get; set; }
            public string Secret { get; set; }
        }
    }
}
