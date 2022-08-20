using DID.Entitys;
using DID.Helps;
using DID.Models;
using System.Drawing;

namespace DID.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserAuthService
    {
        /// <summary>
        /// 上传用户认证信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Response> UploadUserInfo(UserAuthInfo info);
        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="file"></param>
        /// <param name="upload"></param>
        /// <returns></returns>
        Task<Response> UploadImage(IFormFile file, Upload upload);

        /// <summary>
        /// 获取未审核信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Response<List<UserAuthRespon>>> GetUnauditedInfo(int uId);

        /// <summary>
        /// 获取已审核审核信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Response<List<UserAuthRespon>>> GetAuditedInfo(int uId);

        /// <summary>
        /// 获取打回信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Response<List<UserAuthRespon>>> GetBackInfo(int uId);

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="userAuthInfoId"></param>
        /// <param name="uId"></param>
        /// <param name="auditType"></param>
        /// <returns></returns>
        Task<Response> AuditInfo(string userAuthInfoId, int uId, AuditTypeEnum auditType);
    }
    /// <summary>
    /// 审核认证服务
    /// </summary>
    public class UserAuthService : IUserAuthService
    {
        private readonly ILogger<UserAuthService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public UserAuthService(ILogger<UserAuthService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="userAuthInfoId"></param>
        /// <param name="uId"></param>
        /// <param name="auditType"></param>
        /// <returns></returns>
        public async Task<Response> AuditInfo(string userAuthInfoId, int uId, AuditTypeEnum auditType)
        {
            using var db = new NDatabase();
            var authinfo = await db.SingleByIdAsync<UserAuthInfo>(userAuthInfoId);
            var auth = await db.SingleOrDefaultAsync<Auth>("select * from Auth where UserAuthInfoId = @0 and AuditUid = @1;", userAuthInfoId, uId);

            auth.AuditType = auditType;
            auth.AuditDate = DateTime.Now;

            db.BeginTransaction();
            await db.UpdateAsync(auth);

            //修改用户审核状态
            if (auth.AuditStep == AuditStepEnum.抽审 && auth.AuditType == AuditTypeEnum.审核通过)
            {
                await db.ExecuteAsync("update DIDUser set AuthType = 2 where Uid = @0;", authinfo.CreatorId);
            }
            else if (auth.AuditType != AuditTypeEnum.审核通过)
            {
                //修改用户审核状态
                await db.ExecuteAsync("update DIDUser set AuthType = 3,UserAuthInfoId = null where Uid = @0;", authinfo.CreatorId);
                //todo: 审核失败 扣分
            }

            //下一步审核
            if (auth.AuditStep == AuditStepEnum.初审 && auth.AuditType == AuditTypeEnum.审核通过)
            {
                var nextAuth = new Auth()
                {
                    AuthId = Guid.NewGuid().ToString(),
                    UserAuthInfoId = userAuthInfoId,
                    AuditUid = await db.SingleOrDefaultAsync<int>("select RefUid from DIDUser where Uid = @0", uId),//推荐人审核                                                                                //HandHeldImage = "Images/AuthImges/" + info.CreatorId + "/" + info.HandHeldImage,
                    CreateDate = DateTime.Now,
                    AuditType = AuditTypeEnum.未审核,
                    AuditStep = AuditStepEnum.二审
                };
                //人像照处理
                var img = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.PortraitImage));
                img = CommonHelp.WhiteGraphics(img, new Rectangle((int)(img.Width * 0.6), 0, (int)(img.Width * 0.4), img.Height));//遮住右边40%
                nextAuth.PortraitImage = "Images/AuthImges/" + authinfo.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
                img.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nextAuth.PortraitImage));
                //国徽面处理
                var img1 = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.NationalImage));
                img1 = CommonHelp.WhiteGraphics(img1, new Rectangle((int)(img1.Width * 0.6), 0, (int)(img1.Width * 0.4), img1.Height));//遮住右边40%
                nextAuth.NationalImage = "Images/AuthImges/" + authinfo.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
                img.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nextAuth.NationalImage));
                
                await db.InsertAsync(nextAuth);
            }
            else if (auth.AuditStep == AuditStepEnum.二审 && auth.AuditType == AuditTypeEnum.审核通过)
            {
                var nextAuth = new Auth()
                {
                    AuthId = Guid.NewGuid().ToString(),
                    UserAuthInfoId = userAuthInfoId,
                    AuditUid = await db.SingleOrDefaultAsync<int>("select RefUid from DIDUser where Uid = @0", uId),//推荐人审核                                                                                //HandHeldImage = "Images/AuthImges/" + info.CreatorId + "/" + info.HandHeldImage,
                    CreateDate = DateTime.Now,
                    AuditType = AuditTypeEnum.未审核,
                    AuditStep = AuditStepEnum.抽审
                };
                //人像照处理
                var img = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.PortraitImage));
                img = CommonHelp.MaSaiKeGraphics(img, 8);//随机30%马赛克
                nextAuth.PortraitImage = "Images/AuthImges/" + authinfo.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
                img.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nextAuth.PortraitImage));
                //国徽面处理
                var img1 = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authinfo.NationalImage));
                img1 = CommonHelp.MaSaiKeGraphics(img1, 8);//随机30%马赛克
                nextAuth.NationalImage = "Images/AuthImges/" + authinfo.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
                img.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nextAuth.NationalImage));

                await db.InsertAsync(nextAuth);
            }
           
            db.CompleteTransaction();

            return InvokeResult.Success("审核成功!");
        }

        /// <summary>
        /// 获取已审核审核信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public async Task<Response<List<UserAuthRespon>>> GetAuditedInfo(int uId)
        {
            var result = new List<UserAuthRespon>();
            using var db = new NDatabase();
            var items = await db.FetchAsync<Auth>("select * from Auth where AuditUid = @0 and AuditType != 0", uId);
            foreach (var item in items)
            {
                var authinfo = await db.SingleOrDefaultAsync<UserAuthRespon>("select * from UserAuthInfo where UserAuthInfoId = @0",item.UserAuthInfoId);
                var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 order by AuditStep", item.UserAuthInfoId);
                authinfo.Auths = auths;
                result.Add(authinfo);
            }
            return InvokeResult.Success(result);
        }

        /// <summary>
        /// 获取未审核信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public async Task<Response<List<UserAuthRespon>>> GetUnauditedInfo(int uId)
        {
            var result = new List<UserAuthRespon>();
            using var db = new NDatabase();
            var items = await db.FetchAsync<Auth>("select * from Auth where AuditUid = @0 and AuditType = 0", uId);
            foreach (var item in items)
            {
                var authinfo = await db.SingleOrDefaultAsync<UserAuthRespon>("select * from UserAuthInfo where UserAuthInfoId = @0", item.UserAuthInfoId);
                var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 order by AuditStep", item.UserAuthInfoId);
                authinfo.Auths = auths;
                result.Add(authinfo);
            }
            return InvokeResult.Success(result);
        }

        /// <summary>
        /// 获取打回信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public async Task<Response<List<UserAuthRespon>>> GetBackInfo(int uId)
        {
            var result = new List<UserAuthRespon>();
            using var db = new NDatabase();
            var items = await db.FetchAsync<Auth>("select * from Auth where AuditUid = @0", uId);
            foreach (var item in items)
            {
                var authinfo = await db.SingleOrDefaultAsync<UserAuthRespon>("select * from UserAuthInfo where UserAuthInfoId = @0", item.UserAuthInfoId);
                var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 order by AuditStep", item.UserAuthInfoId);
                authinfo.Auths = auths;
                var next = auths.Where(a => a.AuditStep == item.AuditStep + 1).ToList();
                if (next.Count > 0 && (next[0].AuditType != AuditTypeEnum.未审核 && next[0].AuditType != AuditTypeEnum.审核通过))
                    result.Add(authinfo);
            }
            return InvokeResult.Success(result);
        }

        /// <summary>
        /// 获取审核详细信息
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        //public async Task<Response<List<UserAuthInfo>>> GetInfo(List<Auth> lists)
        //{
        //    using var db = new NDatabase();
        //    foreach (var item in lists)
        //    {
        //        var items = await db.FetchAsync<UserAuthInfo>("select * from UserAuthInfo where AuditUid = @0 and AuditType != 0 and AuditType != 1;", userId);
        //    }
        //    return InvokeResult.Success(items);
        //}

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="file"></param>
        /// <param name="upload"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response> UploadImage(IFormFile file, Upload upload)
        {
            try
            {
                var dir = new DirectoryInfo(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Images/AuthImges/" + upload.UId + "/"));
                
                //保存目录不存在就创建这个目录
                if (!dir.Exists)
                {
                    Directory.CreateDirectory(dir.FullName);
                }
                //var filename = upload.UserId + "_" + upload.Type + ".jpg";
                var filename = Guid.NewGuid().ToString() + ".jpg";
                using (var stream = new FileStream(dir.FullName + filename, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    //file.CopyTo(stream);
                }
                //return InvokeResult.Success("Images/AuthImges/" + upload.UId + "/" + filename);
                return InvokeResult.Success(filename);
            }
            catch (Exception e)
            {
                _logger.LogError("UploadImage", e);
                return InvokeResult.Fail("Fail");
            }
        }

        /// <summary>
        /// 上传用户认证信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response> UploadUserInfo(UserAuthInfo info)
        {
            info.UserAuthInfoId = Guid.NewGuid().ToString();
            info.CreateDate = DateTime.Now;
            //info.PortraitImage = "Images/AuthImges/" + info.CreatorId + "/" + info.PortraitImage;
            //info.NationalImage = "Images/AuthImges/" + info.CreatorId + "/" + info.NationalImage;
            //info.HandHeldImage = "Images/AuthImges/" + info.CreatorId + "/" + info.HandHeldImage;

            using var db = new NDatabase();
            var list = await db.SingleOrDefaultAsync<string>("select UserAuthInfoId from DIDUSer where Uid =@0", info.CreatorId);
            if(!string.IsNullOrEmpty(list))
                return InvokeResult.Fail("请勿重复提交!");

            var auth = new Auth
            {
                AuthId = Guid.NewGuid().ToString(),
                UserAuthInfoId = info.UserAuthInfoId,
                AuditUid = await db.SingleOrDefaultAsync<int>("select RefUid from DIDUser where Uid = @0", info.CreatorId),//推荐人审核
                //HandHeldImage = "Images/AuthImges/" + info.CreatorId + "/" + info.HandHeldImage,
                CreateDate = DateTime.Now,
                AuditType = AuditTypeEnum.未审核,
                AuditStep = AuditStepEnum.初审
            };
            //人像照处理
            var img = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, info.PortraitImage));
            img = CommonHelp.WhiteGraphics(img, new Rectangle(0, 0, (int)(img.Width * 0.4), img.Height));//遮住左边40%
            auth.PortraitImage = "Images/AuthImges/" + info.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
            img.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, auth.PortraitImage));
            //国徽面处理
            var img1 = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, info.NationalImage));
            img1 = CommonHelp.WhiteGraphics(img, new Rectangle(0, 0, (int)(img1.Width * 0.4), img1.Height));//遮住左边40%
            auth.NationalImage = "Images/AuthImges/" + info.CreatorId + "/" + Guid.NewGuid().ToString() + ".jpg";
            img.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, auth.NationalImage));

            db.BeginTransaction();
            await db.InsertAsync(info);
            await db.InsertAsync(auth);
            await db.ExecuteAsync("update DIDUser set UserAuthInfoId = @0,AuthType = 1 where Uid = @1", info.UserAuthInfoId, info.CreatorId);//更新用户当前认证编号 审核中
            db.CompleteTransaction();

            //两小时没人审核 自动到Dao审核
            var t = new System.Timers.Timer(10000);//实例化Timer类，设置间隔时间为10000毫秒；
            t.Elapsed += new System.Timers.ElapsedEventHandler((object? source, System.Timers.ElapsedEventArgs e) =>
            {
                t.Stop(); //先关闭定时器
               //todo: Dao审核
               
            });//到达时间的时候执行事件；
            t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            t.Start(); //启动定时器

            return InvokeResult.Success("提交成功!");
        }
    }
}
