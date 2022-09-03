using DID.Entitys;
using DID.Helps;
using DID.Models;
using System.Drawing;

namespace DID.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICreditScoreService
    {
        /// <summary>
        /// 信用分操作(加分、 减分)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<Response> CreditScore(CreditScoreReq req);

        /// <summary>
        /// 获取信用分记录和当前信用分
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<GetCreditScoreRespon>> GetCreditScore(string userId);
    }
    /// <summary>
    /// 审核认证服务
    /// </summary>
    public class CreditScoreService : ICreditScoreService
    {
        private readonly ILogger<CreditScoreService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public CreditScoreService(ILogger<CreditScoreService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///// 信用分操作(加分、 减分)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<Response> CreditScore(CreditScoreReq req)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where Uid = @0 and IsLogout = 0", req.Uid);
            if(null == user)
                return InvokeResult.Fail("2");//用户未找到!

            var item = new CreditScoreHistory
            {
                CreditScoreHistoryId = Guid.NewGuid().ToString(),
                Fraction = req.Fraction,
                Type = req.Type,
                Remarks = req.Remarks,
                CreateDate = DateTime.Now,
                DIDUserId = user.DIDUserId
            };
            
            db.BeginTransaction();
            if (item.Type == TypeEnum.加分)
                await db.ExecuteAsync("update DIDUser set CreditScore = CreditScore + @1  where DIDUserId = @0", user.DIDUserId, item.Fraction);
            else
            {
                if(user.CreditScore < req.Fraction)
                    return InvokeResult.Fail("3");//信用分不足!
                await db.ExecuteAsync("update DIDUser set CreditScore = CreditScore - @1  where DIDUserId = @0", user.DIDUserId, item.Fraction);
            }
            var insert = await db.InsertAsync(item);
            db.CompleteTransaction();
            return InvokeResult.Success("记录插入成功!");
        }

        /// <summary>
        /// 获取信用分记录和当前信用分
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<GetCreditScoreRespon>> GetCreditScore(string userId)
        {
            using var db = new NDatabase();
            var list = await db.FetchAsync<CreditScoreHistory>("select * from CreditScoreHistory where DIDUserId=@0;", userId);
            var fraction = await db.SingleOrDefaultAsync<int>("select CreditScore from DIDUser where DIDUserId = @0", userId);

            return InvokeResult.Success(new GetCreditScoreRespon { CreditScore = fraction, Items = list });
        }
    }
}
