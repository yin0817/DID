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
        Task<Response> CreditScore(CreditScoreHistory item);

        /// <summary>
        /// 获取信用分记录和当前信用分
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<GetCreditScoreRespon>> GetCreditScore(int userId);
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
        public async Task<Response> CreditScore(CreditScoreHistory item)
        {
            item.CreditScoreHistoryId = Guid.NewGuid().ToString();
            item.CreateDate = DateTime.Now;
            using var db = new NDatabase();
            db.BeginTransaction();
            if(item.Type == TypeEnum.加分)
                await db.ExecuteAsync("update DIDUser set CreditScore = CreditScore + @1  where Uid = @0", item.Uid, item.Fraction);
            else
                await db.ExecuteAsync("update DIDUser set CreditScore = CreditScore - @1  where Uid = @0", item.Uid, item.Fraction);
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
        public async Task<Response<GetCreditScoreRespon>> GetCreditScore(int userId)
        {
            using var db = new NDatabase();
            var list = await db.FetchAsync<CreditScoreHistory>("select * from CreditScoreHistory where Uid=@0;", userId);
            var fraction = await db.SingleOrDefaultAsync<int>("select CreditScore from DIDUser where Uid = @0", userId);

            return InvokeResult.Success(new GetCreditScoreRespon { CreditScore = fraction, Items = list });
        }
    }
}
