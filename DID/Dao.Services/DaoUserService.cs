using Dao.Common;
using Dao.Entity;
using Dao.Models.Base;
using Dao.Models.Request;
using Dao.Models.Response;
using DID.Common;
using DID.Entity;
using DID.Entitys;
using DID.Models.Base;
using DID.Models.Response;
using Microsoft.Extensions.Logging;
using NPoco;
using System.Drawing;

namespace Dao.Services
{
    /// <summary>
    /// 风控服务接口
    /// </summary>
    public interface IDaoUserService
    {
        /// <summary>
        /// 成为仲裁员
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        Task<Response<string>> ToArbitrator(string userId, double score);

        /// <summary>
        /// 成为审核员
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        Task<Response<string>> ToAuditor(string userId, double score);

        /// <summary>
        /// 获取Dao用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<GetDaoInfoRespon>> GetDaoInfo(string userId);

        /// <summary>
        /// 获取审核员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<GetAuditorRespon>> GetAuditor(string userId);

        /// <summary>
        /// 解除审核员身份
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response> RelieveAuditor(string userId);

        /// <summary>
        /// 是否启用Dao审核仲裁权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        Task<Response> SetDaoEnable(string userId, IsEnum isEnable);

        /// <summary>
        /// 获取仲裁员申请
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="userId"></param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        Task<Response<List<GetArbitratorsListRespon>>> GetArbitratorsList(long page, long itemsPerPage, string userId, bool isAudit, string? key);

        /// <summary>
        /// 获取审核员申请
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="userId"></param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        Task<Response<List<GetAuditorListRespon>>> GetAuditorList(long page, long itemsPerPage, string userId, bool isAudit, string? key);

        /// <summary>
        /// 获取审核员申请信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<GetAuditorInfoRespon>> GetAuditorInfo(string id);

        /// <summary>
        /// 获取仲裁员申请信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<GetArbitratorsInfoRespon>> GetArbitratorsInfo(string id);

        /// <summary>
        /// 仲裁员 审核员审核
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<Response> AuditAuditor(string userId, string id, int type, AuditStatusEnum state, string? reason);

        /// <summary>
        /// 获取抽审用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<List<UserAuthRespon>>> GetSpotCheck(string userId);

        /// <summary>
        /// 高级节点身份信息打回
        /// </summary>
        /// <param name="userAuthInfoId"></param>
        /// <param name="userId"></param>
        /// <param name="auditType"></param>
        /// <param name="remark"></param>
        /// <returns></returns>

        Task<Response> AuditInfo(string userAuthInfoId, string userId, AuditTypeEnum auditType, string? remark);

        /// <summary>
        /// 获取抽审打回信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isDao"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <returns></returns>
        Task<Response<List<UserAuthRespon>>> GetBackInfo(string userId, long page, long itemsPerPage, string? key);

        /// <summary>
        /// 获取身份认证申述
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<Response<List<GetAuthAppealRespon>>> GetAuthAppeal(string userId, long page, long itemsPerPage, int type);


        /// <summary>
        /// 获取身份认证申述详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<GetAuthAppealDetailsRespon>> GetAuthAppealDetails(string id);


        /// <summary>
        /// 认证申述审核
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        Task<Response> AuthAppeal(string userId, string id, AuthAppealEnum type, string? reason);

        /// <summary>
        /// 获取是否有高级节点审核信息
        /// </summary>
        /// <returns></returns>
        Task<Response<object>> GetHasAuth(string userId);

    }

    /// <summary>
    /// Dao用户信息服务
    /// </summary>
    public class DaoUserService : IDaoUserService
    {
        private readonly ILogger<DaoUserService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public DaoUserService(ILogger<DaoUserService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 成为仲裁员
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<Response<string>> ToArbitrator(string userId, double score)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);

            if (null == user)
            {
                return InvokeResult.Fail<string>("用户信息未找到!");
            }
            if(user.AuthType != AuthTypeEnum.审核成功)
                return InvokeResult.Fail<string>("用户未认证!");
            var eotc = CurrentUser.GetEUModel(user)?.StakeEotc ?? 0;
            //if (eotc < 5000)
            //    return InvokeResult.Fail("质押EOTC数量不足!");
            if (user.IsArbitrate == IsEnum.是)
                return InvokeResult.Fail<string>("请勿重复设置!");

            var userArbitrate = await db.SingleOrDefaultAsync<UserArbitrate>("select * from UserArbitrate where IsDelete = 0 and DIDUserId = @0", userId);
            if (userArbitrate != null)
            {
                if (userArbitrate.Status != AuditStatusEnum.审核失败)
                    return InvokeResult.Fail<string>("请勿重复设置!");
            }
            await db.ExecuteAsync("update UserArbitrate set IsDelete = 1 where DIDUserId = @0", userId);

            //user.IsArbitrate = IsEnum.是;

            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            var nums = await db.FetchAsync<string>("select Number from UserArbitrate order by Number Desc");

            var number = "";
            if (nums.Count > 0 && nums[0]?.Substring(0, 14) == date)
                number = date + (Convert.ToInt32(nums[0].Substring(14, nums[0].Length - 14)) + 1);
            else
                number = date + 1;

            var model = new UserArbitrate() { 
                CreateDate = DateTime.Now,
                DIDUserId = userId,
                UserArbitrateId = Guid.NewGuid().ToString(),
                Number = number,
                Status = AuditStatusEnum.审核中,
                TestScore = score
            };

            db.BeginTransaction();
            await db.UpdateAsync(user);
            await db.InsertAsync(model);
            db.CompleteTransaction();

            return InvokeResult.Success<string>(model.UserArbitrateId);
        }

        /// <summary>
        /// 获取仲裁员申请
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="userId"></param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        public async Task<Response<List<GetArbitratorsListRespon>>> GetArbitratorsList(long page, long itemsPerPage, string userId, bool isAudit, string? key)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);
            if (null == user)
                return InvokeResult.Fail<List<GetArbitratorsListRespon>>("用户信息未找到!");
            if (user.UserNode != UserNodeEnum.高级节点)
                return InvokeResult.Fail<List<GetArbitratorsListRespon>>("用户等级不够!");
            var sql = new Sql("select a.*,c.Name,b.Uid,b.CreditScore   \n" +
                                            "from UserArbitrate a left join DIDUser b on a.DIDUserId = b.DIDUserId left join UserAuthInfo c on b.UserAuthInfoId = c.UserAuthInfoId\n" +
                                            "where a.Status = 0 and a.IsDelete = 0 and (c.Name like '%" + key + "%' or b.Uid like '%" + key + "%')");
            if (isAudit)
                sql = new Sql("select a.*,c.Name,b.Uid,b.CreditScore \n" +
                                            "from UserArbitrate a left join DIDUser b on a.DIDUserId = b.DIDUserId left join UserAuthInfo c on b.UserAuthInfoId = c.UserAuthInfoId\n" +
                                            "where (c.Name like '%" + key + "%' or b.Uid like '%" + key + "%') and a.AuditUserId = @0", userId);

            sql.Append(" order by a.CreateDate Desc");

            var list = (await db.PageAsync<GetArbitratorsListRespon>(page, itemsPerPage, sql)).Items;

            list.ForEach(a => { 
                var mdoel =  db.SingleOrDefault<DIDUser>("select * from DIDUser where DIDUserId = @0", a.DIDUserId);
                a.StakeEotc = CurrentUser.GetEUModel(mdoel)?.StakeEotc ?? 0;//调接口查eotc总数
            });

            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 获取审核员申请
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="userId"></param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        public async Task<Response<List<GetAuditorListRespon>>> GetAuditorList(long page, long itemsPerPage, string userId, bool isAudit, string? key)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);
            if (null == user)
                return InvokeResult.Fail<List<GetAuditorListRespon>>("用户信息未找到!");
            if (user.UserNode != UserNodeEnum.高级节点)
                return InvokeResult.Fail<List<GetAuditorListRespon>>("用户等级不够!");
            var sql = new Sql("select a.*,c.Name,b.Uid,b.CreditScore \n" +
                                            "from UserExamine a left join DIDUser b on a.DIDUserId = b.DIDUserId left join UserAuthInfo c on b.UserAuthInfoId = c.UserAuthInfoId\n" +
                                            "where a.Status = 0 and a.IsDelete = 0 and (c.Name like '%" + key + "%' or b.Uid like '%" + key + "%')");
            if (isAudit)
                sql = new Sql("select a.*,c.Name,b.Uid,b.CreditScore \n" +
                                            "from UserExamine a left join DIDUser b on a.DIDUserId = b.DIDUserId left join UserAuthInfo c on b.UserAuthInfoId = c.UserAuthInfoId\n" +
                                            "where (c.Name like '%" + key + "%' or b.Uid like '%" + key + "%') and a.AuditUserId = @0", userId);

            sql.Append(" order by a.CreateDate Desc");

            var list = (await db.PageAsync<GetAuditorListRespon>(page, itemsPerPage, sql)).Items;

            list.ForEach(a => {
                var mdoel = db.SingleOrDefault<DIDUser>("select * from DIDUser where DIDUserId = @0", a.DIDUserId);
                a.StakeEotc = CurrentUser.GetEUModel(mdoel)?.StakeEotc ?? 0;//调接口查eotc总数
            });

            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 获取审核员申请信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<GetAuditorInfoRespon>> GetAuditorInfo(string id)
        {
            using var db = new NDatabase();
            var model = await db.SingleOrDefaultAsync<GetAuditorInfoRespon>("select * from UserExamine where UserExamineId = @0", id);
            if(null == model)
                return InvokeResult.Fail<GetAuditorInfoRespon>("信息未找到!");
            model.Uid = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", model.AuditUserId);
            model.Name = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", model.AuditUserId);

            return InvokeResult.Success(model);
        }

        /// <summary>
        /// 获取仲裁员申请信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<GetArbitratorsInfoRespon>> GetArbitratorsInfo(string id)
        {
            using var db = new NDatabase();
            var model = await db.SingleOrDefaultAsync<GetArbitratorsInfoRespon>("select * from UserArbitrate where UserArbitrateId = @0", id);
            if (null == model)
                return InvokeResult.Fail<GetArbitratorsInfoRespon>("信息未找到!");
            model.Uid = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", model.AuditUserId);
            model.Name = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", model.AuditUserId);

            return InvokeResult.Success(model);
        }


        /// <summary>
        /// 成为审核员
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<Response<string>> ToAuditor(string userId, double score)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);

            if (null == user)
            {
                return InvokeResult.Fail<string>("用户信息未找到!");
            }
            if (user.AuthType != AuthTypeEnum.审核成功)
                return InvokeResult.Fail<string>("用户未认证!");
            var eotc = CurrentUser.GetEUModel(user)?.StakeEotc ?? 0;
            //if (eotc < 5000)
            //    return InvokeResult.Fail("质押EOTC数量不足!");
            if (user.IsExamine == IsEnum.是)
                return InvokeResult.Fail<string>("请勿重复设置!");
            //user.IsExamine = IsEnum.是;
            var userExamine = await db.SingleOrDefaultAsync<UserExamine>("select * from UserExamine where IsDelete = 0 and DIDUserId = @0", userId);
            if (userExamine != null)
            { 
                if(userExamine.Status != AuditStatusEnum.审核失败)
                    return InvokeResult.Fail<string>("请勿重复设置!");
            }

            await db.ExecuteAsync("update UserExamine set IsDelete = 1 where DIDUserId = @0", userId);

            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            var nums = await db.FetchAsync<string>("select Number from UserExamine order by Number Desc");

            var number = "";
            if (nums.Count > 0 && nums[0]?.Substring(0, 14) == date)
                number = date + (Convert.ToInt32(nums[0].Substring(14, nums[0].Length - 14)) + 1);
            else
                number = date + 1;

            var model = new UserExamine()
            {
                CreateDate = DateTime.Now,
                DIDUserId = userId,
                UserExamineId = Guid.NewGuid().ToString(),
                Number = number,
                Status = AuditStatusEnum.审核中,
                TestScore = score
            };

            db.BeginTransaction();
            await db.UpdateAsync(user);
            await db.InsertAsync(model);
            db.CompleteTransaction();

            await db.UpdateAsync(user);

            return InvokeResult.Success<string>(model.UserExamineId);
        }

        /// <summary>
        /// 仲裁员 审核员审核
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<Response> AuditAuditor(string userId, string id, int type, AuditStatusEnum state,  string? reason)
        {
            using var db = new NDatabase();
            db.BeginTransaction();
            //仲裁员审核
            if (type == 0)
            {
                var model = await db.SingleByIdAsync<UserArbitrate>(id);
                if (null == model)
                    return InvokeResult.Fail("信息未找到!");
                if (model.Status != AuditStatusEnum.审核中)
                    return InvokeResult.Fail("已审核!");

                model.Status = state;
                model.AuditDate = DateTime.Now;
                model.AuditUserId = userId;
                model.Reason = reason;
                if (state == AuditStatusEnum.审核成功)
                {
                    var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", model.DIDUserId);
                    if (null == user)
                        return InvokeResult.Fail("用户信息未找到!");

                    user.IsArbitrate = IsEnum.是;

                    await db.UpdateAsync(user);
                }
                await db.UpdateAsync(model);
            }
            else if (type == 1)//审核员审核
            {
                var model = await db.SingleByIdAsync<UserExamine>(id);
                if (null == model)
                    return InvokeResult.Fail("信息未找到!");
                if (model.Status != AuditStatusEnum.审核中)
                    return InvokeResult.Fail("已审核!");

                model.Status = state;
                model.AuditDate = DateTime.Now;
                model.Reason = reason;
                model.AuditUserId = userId;
                if (state == AuditStatusEnum.审核成功)
                {
                    var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", model.DIDUserId);
                    if (null == user)
                        return InvokeResult.Fail("用户信息未找到!");

                    user.IsExamine = IsEnum.是;

                    await db.UpdateAsync(user);
                }
                await db.UpdateAsync(model);
            }
            db.CompleteTransaction();
            return InvokeResult.Success("审核成功!");
        }



        /// <summary>
        /// 获取审核员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<GetAuditorRespon>> GetAuditor(string userId)
        {
            using var db = new NDatabase();

            var model = await db.SingleOrDefaultAsync<UserExamine>("select * from UserExamine where DIDUserId = @0 and IsDelete = 0", userId);

            return InvokeResult.Success(new GetAuditorRespon
            {
                ExamineNum = model.ExamineNum,
                CreateDate = model.CreateDate,
                EOTC = model.EOTC,
                Number = model.Number,
                Name = WalletHelp.GetName(userId)
            });
        }


        /// <summary>
        /// 解除审核员身份
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response> RelieveAuditor(string userId)
        {
            using var db = new NDatabase();

            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);
            user.IsExamine = IsEnum.否;

            var model = await db.SingleOrDefaultAsync<UserExamine>("select * from UserExamine where DIDUserId = @0 and IsDelete = 0", userId);
            model.IsDelete = IsEnum.是;

            db.BeginTransaction();
            await db.UpdateAsync(user);
            await db.UpdateAsync(model);
            db.CompleteTransaction();

            return InvokeResult.Success("解除成功!");
        }


        /// <summary>
        /// 获取Dao用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<GetDaoInfoRespon>> GetDaoInfo(string userId)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);
            if (null == user)
            {
                return InvokeResult.Fail<GetDaoInfoRespon>("用户信息未找到!");
            }

            return InvokeResult.Success(new GetDaoInfoRespon() { 
                DaoEOTC = user.DaoEOTC,
                IsExamine = user.IsExamine,
                IsArbitrate = user.IsArbitrate,
                RiskLevel = user.RiskLevel,
                AuthType = user.AuthType,
                Mail = user.Mail,
                Uid = user.Uid,
                UserId = userId,
                IsEnable = user.IsEnable,
                UserNode = user.UserNode,
                UserExamineId = db.SingleOrDefault<string>("select UserExamineId from UserExamine where DIDUserId = @0 and IsDelete = 0", user.DIDUserId),
                UserExamineStatus = db.SingleOrDefault<AuditStatusEnum>("select Status from UserExamine where DIDUserId = @0 and IsDelete = 0", user.DIDUserId),
                UserArbitrateId = db.SingleOrDefault<string>("select UserArbitrateId from UserArbitrate where DIDUserId = @0 and IsDelete = 0", user.DIDUserId),
                UserArbitrateStatus = db.SingleOrDefault<AuditStatusEnum>("select Status from UserArbitrate where DIDUserId = @0 and IsDelete = 0", user.DIDUserId)
            });
        }

        /// <summary>
        /// 是否启用Dao审核仲裁权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public async Task<Response> SetDaoEnable(string userId, IsEnum isEnable)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);
            if (null == user)
            {
                return InvokeResult.Fail("用户信息未找到!");
            }
            user.IsEnable = isEnable;
            await db.UpdateAsync(user);
            return InvokeResult.Success("设置成功!");
        }

        /// <summary>
        /// 获取抽审用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<List<UserAuthRespon>>> GetSpotCheck(string userId)
        {
            var result = new List<UserAuthRespon>();
            using var db = new NDatabase();
            var items = await db.FetchAsync<string>("SELECT top 50 UserAuthInfoId FROM [dbo].[DIDUser] where AuthType = 2 ORDER BY NEWID()");//随机50个
            foreach (var item in items)
            {
                var authinfo = await db.SingleOrDefaultAsync<UserAuthRespon>("select * from UserAuthInfo where UserAuthInfoId = @0", item);

                authinfo.Name = await db.SingleOrDefaultAsync<string>("select isnull(c.Name, '未认证') + '(' + CONVERT(VARCHAR(20), b.Uid) + ')' name from UserAuthInfo c left join DIDUser b on c.UserAuthInfoId = b.UserAuthInfoId where c.UserAuthInfoId = @0", item);

                if (null != authinfo.PortraitImage && authinfo.PortraitImage.StartsWith("Auth/AuthImges"))
                {
                    //人像照处理
                    var img = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.PortraitImage));
                    img = CommonHelp.WhiteGraphics(img, new Rectangle((int)(img.Width * 0.6), 0, (int)(img.Width * 0.4), img.Height));//遮住右边40%
                    authinfo.PortraitImage = "Auth/AuthImges/" + authinfo.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
                    img.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.PortraitImage));
                    //国徽面处理
                    var img1 = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.NationalImage));
                    img1 = CommonHelp.WhiteGraphics(img1, new Rectangle((int)(img1.Width * 0.6), 0, (int)(img1.Width * 0.4), img1.Height));//遮住右边40%
                    authinfo.NationalImage = "Auth/AuthImges/" + authinfo.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
                    img1.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.NationalImage));
                    //手持处理
                    var img2 = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.HandHeldImage));
                    img2 = CommonHelp.WhiteGraphics(img2, new Rectangle((int)(img2.Width * 0.5), 0, (int)(img2.Width * 0.5), img2.Height));//遮住右边40%
                    authinfo.HandHeldImage = "Auth/AuthImges/" + authinfo.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
                    img2.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.HandHeldImage));
                }
                //基本信息处理
                if (authinfo.PhoneNum.Length > 7)
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(3, 4).Insert(3, "****");
                if (authinfo.IdCard.Length > 7)
                    authinfo.IdCard = authinfo.IdCard.Remove(authinfo.IdCard.Length - 4, 4).Insert(authinfo.IdCard.Length - 4, "****");

                var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 and IsDelete = 0 order by CreateDate", item);
                var list = new List<AuthInfo>();
                foreach (var auth in auths)
                {
                    list.Add(new AuthInfo()
                    {
                        UId = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", auth.AuditUserId),
                        AuditStep = auth.AuditStep,
                        AuthDate = auth.AuditDate,
                        Name = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", auth.AuditUserId),
                        AuditType = auth.AuditType,
                        Remark = auth.Remark,
                        IsDao = auth.IsDao
                    });

                }
                authinfo.Auths = list;
                result.Add(authinfo);
            }
            return InvokeResult.Success(result);
        }
        /// <summary>
        /// 高级节点身份信息打回
        /// </summary>
        /// <param name="userAuthInfoId"></param>
        /// <param name="userId"></param>
        /// <param name="auditType"></param>
        /// <param name="remark"></param>
        /// <returns></returns>

        public async Task<Response> AuditInfo(string userAuthInfoId, string userId, AuditTypeEnum auditType, string? remark)
        {
            using var db = new NDatabase();
            var nextAuth = new Auth()
            {
                AuthId = Guid.NewGuid().ToString(),
                UserAuthInfoId = userAuthInfoId,
                AuditUserId = userId,                                                                              
                CreateDate = DateTime.Now,
                AuditType = auditType,
                AuditStep = AuditStepEnum.抽审,
                Remark = remark
            };

            await db.InsertAsync(nextAuth);

            return InvokeResult.Success("打回成功!");
        }

        /// <summary>
        /// 获取抽审打回信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isDao"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <returns></returns>
        public async Task<Response<List<UserAuthRespon>>> GetBackInfo(string userId, long page, long itemsPerPage, string? key)
        {
            var result = new List<UserAuthRespon>();
            using var db = new NDatabase();
            //var items = await db.FetchAsync<Auth>("select * from Auth where AuditUserId = @0", userId);
            var items = (await db.PageAsync<Auth>(page, itemsPerPage, "select a.* from Auth a left join UserAuthInfo b on a.UserAuthInfoId = b.UserAuthInfoId where a.AuditUserId = @0 and a.IsDelete = 0 and a.AuditStep = 2 " +
                " and (b.Name like '%" + key + "%' or b.PhoneNum like '%" + key + "%' or b.IdCard like '%" + key + "%') order by b.CreateDate Desc ", userId)).Items;
            foreach (var item in items)
            {
                var authinfo = await db.SingleOrDefaultAsync<UserAuthRespon>("select * from UserAuthInfo where UserAuthInfoId = @0", item.UserAuthInfoId);

                authinfo.Name = await db.SingleOrDefaultAsync<string>("select isnull(c.Name, '未认证') + '(' + CONVERT(VARCHAR(20), b.Uid) + ')' name from UserAuthInfo c left join DIDUser b on c.UserAuthInfoId = b.UserAuthInfoId where c.UserAuthInfoId = @0", item.UserAuthInfoId);
                if (authinfo.PortraitImage.StartsWith("Auth/AuthImges"))
                {
                    //人像照处理
                    var img = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.PortraitImage));
                    img = CommonHelp.WhiteGraphics(img, new Rectangle((int)(img.Width * 0.6), 0, (int)(img.Width * 0.4), img.Height));//遮住右边40%
                    authinfo.PortraitImage = "Auth/AuthImges/" + authinfo.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
                    img.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.PortraitImage));
                    //国徽面处理
                    var img1 = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.NationalImage));
                    img1 = CommonHelp.WhiteGraphics(img1, new Rectangle((int)(img1.Width * 0.6), 0, (int)(img1.Width * 0.4), img1.Height));//遮住右边40%
                    authinfo.NationalImage = "Auth/AuthImges/" + authinfo.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
                    img1.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.NationalImage));
                    //手持处理
                    var img2 = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.HandHeldImage));
                    img2 = CommonHelp.WhiteGraphics(img2, new Rectangle((int)(img2.Width * 0.5), 0, (int)(img2.Width * 0.5), img2.Height));//遮住右边40%
                    authinfo.HandHeldImage = "Auth/AuthImges/" + authinfo.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
                    img2.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.HandHeldImage));
                }
                //基本信息处理

                if (authinfo.PhoneNum.Length > 7)
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(3, 4).Insert(3, "****");
                if (authinfo.IdCard.Length > 7)
                    authinfo.IdCard = authinfo.IdCard.Remove(authinfo.IdCard.Length - 4, 4).Insert(authinfo.IdCard.Length - 4, "****");

                var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 and IsDelete = 0 and AuditStep <= @1 order by CreateDate", item.UserAuthInfoId, item.AuditStep);
                var list = new List<AuthInfo>();
                foreach (var auth in auths)
                {
                    list.Add(new AuthInfo()
                    {
                        UId = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", auth.AuditUserId),
                        AuditStep = auth.AuditStep,
                        AuthDate = auth.AuditDate,
                        Name = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", auth.AuditUserId),
                        AuditType = auth.AuditType,
                        Remark = auth.Remark,
                        IsDao = auth.IsDao
                    });
                }
                authinfo.Auths = list;
                var next = auths.Where(a => a.AuditStep == item.AuditStep + 1).ToList();
                if (next.Count > 0 && (next[0].AuditType != AuditTypeEnum.未审核 && next[0].AuditType != AuditTypeEnum.审核通过))
                    result.Add(authinfo);
            }
            return InvokeResult.Success(result);
        }

        /// <summary>
        /// 获取身份认证申述
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<Response<List<GetAuthAppealRespon>>> GetAuthAppeal(string userId, long page, long itemsPerPage, int type)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);
            if (null == user)
                return InvokeResult.Fail<List<GetAuthAppealRespon>>("用户信息未找到!");
            if (user.UserNode != UserNodeEnum.高级节点)
                return InvokeResult.Fail<List<GetAuthAppealRespon>>("用户等级不够!");
            var list = new List<GetAuthAppealRespon>();

            if(type == 0)
                list = (await db.PageAsync<GetAuthAppealRespon>(page, itemsPerPage, "select * from AuthAppeal where AuditType = 0")).Items;//查找未审核申述
            else if(type == 1)
                list = (await db.PageAsync<GetAuthAppealRespon>(page, itemsPerPage, "select * from AuthAppeal where AuditType = 0 and AuditUserId = @0", userId)).Items;//查找已审核申述

            list.ForEach(a => { 
                a.Name = db.SingleOrDefault<string>("select Name from UserAuthInfo where UserAuthInfo = @0", a.UserAuthInfoId);
                a.Uid = db.SingleOrDefault<int>("select a.Uid from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.UserAuthInfoId = @0", a.UserAuthInfoId);
            });

            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 获取身份认证申述详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<GetAuthAppealDetailsRespon>> GetAuthAppealDetails(string id)
        {
            using var db = new NDatabase();

            var model = await db.SingleOrDefaultAsync<GetAuthAppealDetailsRespon>("select a.*,b.Name,c.Uid from AuthAppeal a left join UserAuthInfoId b on a.UserAuthInfoId = b.UserAuthInfoId " +
                "left join DIDUser c on c.UserAuthInfoId = b.UserAuthInfoId where a.AuthAppealId = @0", id);
            if (null == model)
                return InvokeResult.Fail<GetAuthAppealDetailsRespon>("信息未找到!");

            var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 and IsDelete = 0 order by AuditStep", model.UserAuthInfoId);
            var list = new List<AuthInfo>();
            foreach (var auth in auths)
            {
                list.Add(new AuthInfo()
                {
                    UId = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", auth.AuditUserId),
                    AuditStep = auth.AuditStep,
                    AuthDate = auth.AuditDate,
                    Name = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", auth.AuditUserId),
                    AuditType = auth.AuditType,
                    Remark = auth.Remark
                });
            }
            model.Auths = list;

            model.AuditName = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", model.AuditUserId);
            model.AuditUid = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", model.AuditUserId);

            return InvokeResult.Success(model);
        }

        /// <summary>
        /// 认证申述审核
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task<Response> AuthAppeal(string userId, string id, AuthAppealEnum type, string? reason)
        {
            using var db = new NDatabase();

            var model = await db.SingleOrDefaultByIdAsync<AuthAppeal>(id);
            if (null == model)
                return InvokeResult.Fail("信息未找到!");
            model.AuditUserId = userId;
            model.AuditDate = DateTime.Now;
            model.AuditType = type;
            model.Reason = reason;

            await db.UpdateAsync(model);

            return InvokeResult.Success("审核成功!");
        }

        /// <summary>
        /// 获取是否有高级节点审核信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<object>> GetHasAuth(string userId)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);
            if (null == user)
                return InvokeResult.Fail<object>("用户信息未找到!");
            if (user.UserNode != UserNodeEnum.高级节点)
                return InvokeResult.Fail<object>("用户等级不够!");

            //审核员申请
            var list = await db.SingleOrDefaultAsync<int>("select count(UserExamineId) from UserExamine where Status = 0 and IsDelete = 0");
            //仲裁员申请
            var list1 = await db.SingleOrDefaultAsync<int>("select count(UserArbitrateId) from UserArbitrate where Status = 0 and IsDelete = 0");
            //认证申述
            var list2 = await db.SingleOrDefaultAsync<int>("select count(AuthAppealId) from AuthAppeal where AuditType = 0");


            return InvokeResult.Success<object>(new { UserExamine = list, UserArbitrate = list1, AuthAppeal = list2});
        }
    }
}