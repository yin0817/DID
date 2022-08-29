using DID.Entitys;
using DID.Helps;
using DID.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DID.Controllers
{
    /// <summary>
    /// 社区接口
    /// </summary>
    public interface ICommunityService
    { 
        /// <summary>
        /// 设置用户社区选择（未填邀请码）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<Response> SetComSelect(ComSelectReq req);

        /// <summary>
        /// 获取用户社区选择
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<ComSelect>> GetComSelect(string userId);
        /// <summary>
        /// 获取已有社区位置
        /// </summary>
        /// <returns> </returns>
        Task<Response<ComAddrRespon>> GetComAddr();


    }
    /// <summary>
    /// 支付信息服务
    /// </summary>
    public class CommunityService : ICommunityService
{
        private readonly ILogger<CommunityService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public CommunityService(ILogger<CommunityService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取用户社区选择
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<ComSelect>> GetComSelect(string userId)
        {
            using var db = new NDatabase();
            var item = await db.SingleOrDefaultAsync<ComSelect>("select * from ComSelect where DIDUserId = @0", userId);
            return InvokeResult.Success(item);
        }

        /// <summary>
        /// 设置用户社区选择（未填邀请码）
        /// </summary>
        /// <param name="req"></param>
        /// <returns> </returns>
        public async Task<Response> SetComSelect(ComSelectReq req)
        {
            using var db = new NDatabase();
            var id = await db.SingleOrDefaultAsync<string>("select ComSelectId from ComSelect where DIDUserId = @0", req.UserId);
            if(!string.IsNullOrEmpty(id))
                return InvokeResult.Fail("1"); //请勿重复设置!
            var item = new ComSelect() { 
                ComSelectId = Guid.NewGuid().ToString(),
                DIDUserId = req.UserId,
                Country = req.Country,
                Province = req.Province,
                City = req.City,
                Area = req.Area,
                CreateDate = DateTime.Now
            };
            await db.InsertAsync(item);
            return InvokeResult.Success("插入成功!");
        }

        /// <summary>
        /// 获取已有社区位置
        /// </summary>
        /// <returns> </returns>
        public async Task<Response<ComAddrRespon>> GetComAddr()
        {
            using var db = new NDatabase();
            var province_list = await db.FetchAsync<Area>("select Distinct a.Province, b.Name from Community a left join Area b on a.Province = b.Code where AuthType = @0", AuthTypeEnum.审核成功);
            var city_list = await db.FetchAsync<Area>("select Distinct a.City, b.Name from Community a left join Area b on a.Province = b.Code where AuthType = @0", AuthTypeEnum.审核成功);
            var county_list = await db.FetchAsync<Area>("select Distinct a.Area, b.Name from Community a left join Area b on a.Province = b.Code where AuthType = @0", AuthTypeEnum.审核成功);

            var provinces = new Dictionary<int, string>();
            province_list.ForEach(a => provinces.Add(Convert.ToInt32(a.code), a.name));

            var citys = new Dictionary<int, string>();
            city_list.ForEach(a => citys.Add(Convert.ToInt32(a.code), a.name));

            var countys = new Dictionary<int, string>();
            county_list.ForEach(a => countys.Add(Convert.ToInt32(a.code), a.name));

            var item = new ComAddrRespon(){
                province_list = provinces,
                city_list = citys,
                county_list = countys
            };

            return InvokeResult.Success(item);
        }


        /// <summary>
        /// 获取当前位置社区数量
        /// </summary>
        /// <returns> </returns>
        public async Task<Response<string>> GetComNum()
        {
            using var db = new NDatabase();
            var num = await db.SingleOrDefaultAsync<string>("");
            return InvokeResult.Success<string>(num);
        }

        /// <summary>
        /// 获取当前位置社区
        /// </summary>
        /// <returns> </returns>
        public async Task<Response> GetComList()
        {
            using var db = new NDatabase();
            var num = await db.SingleOrDefaultAsync<string>("");
            return InvokeResult.Success<string>(num);
        }

        /// <summary>
        /// 社区申请
        /// </summary>
        /// <returns> </returns>
        public async Task<Response> ApplyCommunity(Community item)
        {
            using var db = new NDatabase();
            item.CommunityId = Guid.NewGuid().ToString();
            var refUserId = await db.FirstOrDefaultAsync<string>("select RefUserId from DIDUser where DIDUserId = @0", item.DIDUserId);//邀请人
            if (string.IsNullOrEmpty(refUserId))
            {
                return InvokeResult.Fail("邀请人未找到!");
            }

            item.RefDIDUserId = refUserId;
            item.RefCommunityId = await db.FirstOrDefaultAsync<string>("select CommunityId from UserCommunity where DIDUserId = @0", refUserId);//邀请人社区
            item.CreateDate = DateTime.Now;
            item.AuthType = AuthTypeEnum.审核中;

            //邀请人审核
            var auth = new ComAuth
            {
                ComAuthId = Guid.NewGuid().ToString(),
                CommunityId = item.CommunityId,
                AuditUserId = await db.SingleOrDefaultAsync<string>("select RefUserId from DIDUser where DIDUserId = @0", refUserId),//推荐人审核
                //HandHeldImage = "Images/AuthImges/" + info.CreatorId + "/" + info.HandHeldImage,
                CreateDate = DateTime.Now,
                AuditType = AuditTypeEnum.未审核,
                AuditStep = AuditStepEnum.初审
            };
            db.BeginTransaction();
            await db.InsertAsync(item);
            await db.InsertAsync(auth);
            db.CompleteTransaction();

            return InvokeResult.Success("申请成功!");
        }

        /// <summary>
        /// 添加社区信息
        /// </summary>
        /// <returns> </returns>
        public async Task<Response> AddCommunityInfo(Community item)
        {
            using var db = new NDatabase();
            await db.UpdateAsync(item);

            return InvokeResult.Success("添加成功!");
        }

        /// <summary>
        /// 查询社区信息
        /// </summary>
        /// <returns> </returns>
        public async Task<Response> GetCommunityInfo(string userId)
        {
            using var db = new NDatabase();
            var item = await db.SingleOrDefaultAsync<Community>("select * from Community a left join UserCommunity b on a.CommunityId = b.CommunityId where b.DIDUserId = @0", userId); 

            return InvokeResult.Success("添加成功!");
        }

        /// <summary>
        /// 社区申请审核
        /// </summary>
        /// <returns> </returns>
        public async Task<Response> AuditCommunity(string communityId, string userId, AuditTypeEnum auditType)
        {
            using var db = new NDatabase();
            var authinfo = await db.SingleByIdAsync<Community>(communityId);
            var auth = await db.SingleOrDefaultAsync<ComAuth>("select * from ComAuth where CommunityId = @0 and AuditUserId = @1;", communityId, userId);

            auth.AuditType = auditType;
            auth.AuditDate = DateTime.Now;

            db.BeginTransaction();
            await db.UpdateAsync(auth);

            //修改用户审核状态
            if (auth.AuditStep == AuditStepEnum.抽审 && auth.AuditType == AuditTypeEnum.审核通过)
            {
                await db.ExecuteAsync("update Community set AuthType = @1 where CommunityId = @0;", communityId, AuthTypeEnum.审核成功);
            }
            else if (auth.AuditType != AuditTypeEnum.审核通过)
            {
                //修改用户审核状态
                await db.ExecuteAsync("update Community set AuthType = @1 where CommunityId = @0;", communityId, AuthTypeEnum.审核失败);
                //todo: 审核失败 扣分
            }

            //下一步审核
            if (auth.AuditStep == AuditStepEnum.初审 && auth.AuditType == AuditTypeEnum.审核通过)
            {
                var nextAuth = new ComAuth()
                {
                    ComAuthId = Guid.NewGuid().ToString(),
                    CommunityId = communityId,
                    AuditUserId = await db.SingleOrDefaultAsync<string>("select RefUserId from DIDUser where DIDUserId = @0", userId),//推荐人审核 
                    CreateDate = DateTime.Now,
                    AuditType = AuditTypeEnum.未审核,
                    AuditStep = AuditStepEnum.二审
                };

                await db.InsertAsync(nextAuth);
            }
            else if (auth.AuditStep == AuditStepEnum.二审 && auth.AuditType == AuditTypeEnum.审核通过)
            {
                var nextAuth = new ComAuth()
                {
                    ComAuthId = Guid.NewGuid().ToString(),
                    CommunityId = communityId,
                    AuditUserId = await db.SingleOrDefaultAsync<string>("select RefUserId from DIDUser where DIDUserId = @0", userId),//推荐人审核 
                    CreateDate = DateTime.Now,
                    AuditType = AuditTypeEnum.未审核,
                    AuditStep = AuditStepEnum.抽审
                };

                await db.InsertAsync(nextAuth);
            }

            db.CompleteTransaction();

            return InvokeResult.Success("审核成功!");
        }
    }
}
