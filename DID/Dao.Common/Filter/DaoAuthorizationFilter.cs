using Dao.Models.Base;
using DID.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;

namespace Dao.Common.ActionFilter
{
    /// <summary>
    /// Dao接口钱包验证
    /// </summary>
    public class DaoActionFilter : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async virtual Task OnActionExecuting(ActionExecutingContext context)
        {
            var paramss = context.ActionArguments;
            if (paramss != null && paramss.Count > 0)
            {
                foreach (var para in paramss)
                {
                    var formCollection = para.Value as DaoBaseReq;
                    if (formCollection != null) //是否是post请求
                    {
                        var walletAddress = formCollection.WalletAddress;
                        var otype = formCollection.Otype;
                        var sign = formCollection.Sign;

                        //Dao验证钱包地址
                        using var db = new NDatabase();
                        var walletId = await db.SingleOrDefaultAsync<string>("select WalletId from Wallet where WalletAddress = @0 and Otype = @1 and Sign = @2 and IsLogout = 0 and IsDelete = 0",
                                                                            walletAddress, otype, sign);
                        if (string.IsNullOrEmpty(walletId))
                        {
                            context.Result = new ContentResult()
                            {
                                Content = JsonConvert.SerializeObject(new { Code = 401, Message = "认证失败!" }),
                                ContentType = "application/json; charset=utf-8",
                                StatusCode = (int)HttpStatusCode.Unauthorized
                            };
                        }
                    }
                    else
                    {
                        context.Result = new ContentResult()
                        {
                            Content = JsonConvert.SerializeObject(new { Code = 401, Message = "认证失败!" }),
                            ContentType = "application/json; charset=utf-8",
                            StatusCode = (int)HttpStatusCode.Unauthorized
                        };
                    }
                }
            }
            else
            {
                context.Result = new ContentResult()
                {
                    Content = JsonConvert.SerializeObject(new { Code = 401, Message = "认证失败!" }),
                    ContentType = "application/json; charset=utf-8",
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            }

            await Task.CompletedTask;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async virtual Task OnActionExecuted(ActionExecutedContext context)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await OnActionExecuting(context);
            if (context.Result == null)
            {
                var nextContext = await next();
                await OnActionExecuted(nextContext);
            }
        }
    }
}
