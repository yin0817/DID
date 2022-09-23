using Dao.Common;
using Dao.Entity;
using Dao.Models.Base;
using Dao.Models.Request;
using Dao.Models.Response;
using DID.Common;
using DID.Entitys;
using DID.Models.Base;
using Microsoft.Extensions.Logging;
using NPoco;

namespace Dao.Services
{
    /// <summary>
    /// 风控服务接口
    /// </summary>
    public interface IRiskService
    {
        /// <summary>
        /// 设置用户风险等级
        /// </summary>
        /// <param name="req"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        Task<Response> UserRiskLevel(UserRiskLevelReq req);


    }

    /// <summary>
    /// 风控服务
    /// </summary>
    public class RiskService : IRiskService
    {
        private readonly ILogger<RiskService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public RiskService(ILogger<RiskService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 设置用户风险等级
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response> UserRiskLevel(UserRiskLevelReq req)
        {
            using var db = new NDatabase();
            var userId = WalletHelp.GetUserId(req);
            var user = await db.SingleOrDefaultByIdAsync<DIDUser>(userId);
            user.RiskLevel = req.Level;
                
            await db.UpdateAsync(user);

            //生成审核信息（5个人3个通过解除） 可配置
            var list = new List<string>();
            list.Add("e8771b3c-3b05-4830-900d-df2be0a6e9f7");
            list.Add("d389e5db-37d0-40cd-9d8b-0d31a0ef2c12");
            list.Add("61d14a4f-c45f-4b13-a957-5bcaff9b3324");
            list.Add("7e88d292-7454-4e26-821a-b4e6049a7a95");
            list.Add("2a5bf1dd-e15b-40f4-94bb-b68cee2bbaf9");

            list.Select(async x => await db.InsertAsync(
                new UserRisk{
                    UserRiskId = Guid.NewGuid().ToString(),
                    AuditUserId = x,
                    DIDUserId = userId,
                    AuthStatus = RiskStatusEnum.未核对,
                    IsRemoveRisk = Entity.IsEnum.否,
                    Reason = req.Reason,
                    CreateDate = DateTime.Now,
                    IsDelete = Entity.IsEnum.否

                })
            );


            return InvokeResult.Success("设置成功!");
        }

        /// <summary>
        /// 获取风控列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response<List<UserRiskRespon>>> UserRisk(DaoBaseReq req)
        { 
            using var db = new NDatabase();
            var userId = WalletHelp.GetUserId(req);

            var models = await db.FetchAsync<UserRisk>("select * from UserRisk where AuditUserId = @0 and IsRemoveRisk = 0", userId);

            var list = new List<UserRiskRespon>();

            models.ForEach(a => list.Add(new UserRiskRespon()
                {
                    UserRiskId = a.UserRiskId,
                    Name = WalletHelp.GetUidName(a.DIDUserId),
                    Reason = a.Reason,
                    AuthStatus = a.AuthStatus,
                    CreateDate = a.CreateDate
                })
            );

            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 修改用户风险状态
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response> UserRiskStatus(UserRiskStatusReq req)
        {
            using var db = new NDatabase();
            var item = await db.SingleOrDefaultByIdAsync<UserRisk>(req.UserRiskId);
            item.AuthStatus = req.AuthStatus;
            await db.UpdateAsync(item);

            return InvokeResult.Success("修改成功!");
        }

        /// <summary>
        /// 解除风险
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response> RemoveRisk(string UserRiskId)
        {
            using var db = new NDatabase();
            var item = await db.SingleOrDefaultByIdAsync<UserRisk>(UserRiskId);
            item.IsRemoveRisk = Entity.IsEnum.是;
            await db.UpdateAsync(item);

            var list = await db.FetchAsync<UserRisk>("select * from UserRisk where DIDUserId = @0 and IsDelete = 0");
            var num = list.Sum(a => a.IsRemoveRisk == Entity.IsEnum.是 ? 1 : 0);

            //5个人3个通过
            if (num >= 3)
            {
                var user = await db.SingleOrDefaultByIdAsync<DIDUser>(item.DIDUserId);
                user.RiskLevel = RiskLevelEnum.低风险;

                await db.UpdateAsync(user);
                await db.SaveAsync(list.Select(a => { a.IsDelete = Entity.IsEnum.是; return a; }).ToList());
            }
            return InvokeResult.Success("解除成功!");
        }
    }
}