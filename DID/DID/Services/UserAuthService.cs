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
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response> UploadImage(IFormFile file, string userId);

        /// <summary>
        /// 获取未审核信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<List<UserAuthRespon>>> GetUnauditedInfo(string userId);

        /// <summary>
        /// 获取已审核审核信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<List<UserAuthRespon>>> GetAuditedInfo(string userId);

        /// <summary>
        /// 获取打回信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<List<UserAuthRespon>>> GetBackInfo(string userId);

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="userAuthInfoId"></param>
        /// <param name="userId"></param>
        /// <param name="auditType"></param>
        /// <returns></returns>
        Task<Response> AuditInfo(string userAuthInfoId, string userId, AuditTypeEnum auditType);
        /// <summary>
        /// 获取用户审核成功信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<AuthSuccessRespon>> GetAuthSuccess(string userId);
        /// <summary>
        /// 获取用户审核失败信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<AuthFailRespon>> GetAuthFail(string userId);
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
        /// <param name="userId"></param>
        /// <param name="auditType"></param>
        /// <returns></returns>
        public async Task<Response> AuditInfo(string userAuthInfoId, string userId, AuditTypeEnum auditType)
        {
            using var db = new NDatabase();
            var authinfo = await db.SingleByIdAsync<UserAuthInfo>(userAuthInfoId);
            var auth = await db.SingleOrDefaultAsync<Auth>("select * from Auth where UserAuthInfoId = @0 and AuditUserId = @1;", userAuthInfoId, userId);

            auth.AuditType = auditType;
            auth.AuditDate = DateTime.Now;

            db.BeginTransaction();
            await db.UpdateAsync(auth);

            //修改用户审核状态
            if (auth.AuditStep == AuditStepEnum.抽审 && auth.AuditType == AuditTypeEnum.审核通过)
            {
                await db.ExecuteAsync("update DIDUser set AuthType = @1 where DIDUserId = @0;", authinfo.CreatorId, AuthTypeEnum.审核成功);
            }
            else if (auth.AuditType != AuditTypeEnum.审核通过)
            {
                //修改用户审核状态
                await db.ExecuteAsync("update DIDUser set AuthType = @1 where DIDUserId = @0;", authinfo.CreatorId, AuthTypeEnum.审核失败);
                //todo: 审核失败 扣分
            }

            //下一步审核
            if (auth.AuditStep == AuditStepEnum.初审 && auth.AuditType == AuditTypeEnum.审核通过)
            {
                var nextAuth = new Auth()
                {
                    AuthId = Guid.NewGuid().ToString(),
                    UserAuthInfoId = userAuthInfoId,
                    AuditUserId = await db.SingleOrDefaultAsync<string>("select RefUserId from DIDUser where DIDUserId = @0", userId),//推荐人审核                                                                                //HandHeldImage = "Images/AuthImges/" + info.CreatorId + "/" + info.HandHeldImage,
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
                    AuditUserId = await db.SingleOrDefaultAsync<string>("select RefUserId from DIDUser where DIDUserId = @0", userId),//推荐人审核                                                                                //HandHeldImage = "Images/AuthImges/" + info.CreatorId + "/" + info.HandHeldImage,
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
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<List<UserAuthRespon>>> GetAuditedInfo(string userId)
        {
            var result = new List<UserAuthRespon>();
            using var db = new NDatabase();
            var items = await db.FetchAsync<Auth>("select * from Auth where AuditUserId = @0 and AuditType != 0", userId);
            foreach (var item in items)
            {
                var authinfo = await db.SingleOrDefaultAsync<UserAuthRespon>("select * from UserAuthInfo where UserAuthInfoId = @0",item.UserAuthInfoId);
                authinfo.PortraitImage = item.PortraitImage;
                authinfo.NationalImage = item.NationalImage;
                authinfo.HandHeldImage = item.HandHeldImage;
                //基本信息处理
                if (item.AuditStep == AuditStepEnum.初审)
                {
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(0, 4).Insert(0, "****");
                    authinfo.IdCard = authinfo.IdCard.Remove(0, 4).Insert(0, "****");
                }
                else if (item.AuditStep == AuditStepEnum.二审)
                {
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(authinfo.PhoneNum.Length - 4, 4).Insert(authinfo.PhoneNum.Length - 4, "****");
                    authinfo.IdCard = authinfo.IdCard.Remove(authinfo.IdCard.Length - 4, 4).Insert(authinfo.IdCard.Length - 4, "****");
                }
                else if (item.AuditStep == AuditStepEnum.初审)
                {
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(3, 4).Insert(3, "****");
                    authinfo.IdCard = authinfo.IdCard.Remove(authinfo.IdCard.Length - 4, 4).Insert(authinfo.IdCard.Length - 4, "****");
                }
                var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 order by AuditStep", item.UserAuthInfoId);
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
                    }) ;
                
                }
                authinfo.Auths = list;
                result.Add(authinfo);
            }
            return InvokeResult.Success(result);
        }

        /// <summary>
        /// 获取未审核信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<List<UserAuthRespon>>> GetUnauditedInfo(string userId)
        {
            var result = new List<UserAuthRespon>();
            using var db = new NDatabase();
            var items = await db.FetchAsync<Auth>("select * from Auth where AuditUserId = @0 and AuditType = 0", userId);
            foreach (var item in items)
            {
                var authinfo = await db.SingleOrDefaultAsync<UserAuthRespon>("select * from UserAuthInfo where UserAuthInfoId = @0", item.UserAuthInfoId);
                authinfo.PortraitImage = item.PortraitImage;
                authinfo.NationalImage = item.NationalImage;
                authinfo.HandHeldImage = item.HandHeldImage;
                //基本信息处理
                if (item.AuditStep == AuditStepEnum.初审)
                {
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(0, 4).Insert(0, "****");
                    authinfo.IdCard = authinfo.IdCard.Remove(0, 4).Insert(0, "****");
                }
                else if (item.AuditStep == AuditStepEnum.二审)
                {
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(authinfo.PhoneNum.Length - 4, 4).Insert(authinfo.PhoneNum.Length - 4, "****");
                    authinfo.IdCard = authinfo.IdCard.Remove(authinfo.IdCard.Length - 4, 4).Insert(authinfo.IdCard.Length - 4, "****");
                }
                else if (item.AuditStep == AuditStepEnum.初审)
                {
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(3, 4).Insert(3, "****");
                    authinfo.IdCard = authinfo.IdCard.Remove(authinfo.IdCard.Length - 4, 4).Insert(authinfo.IdCard.Length - 4, "****");
                }
                var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 order by AuditStep", item.UserAuthInfoId);
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
                authinfo.Auths = list;
                result.Add(authinfo);
            }
            return InvokeResult.Success(result);
        }

        /// <summary>
        /// 获取打回信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<List<UserAuthRespon>>> GetBackInfo(string userId)
        {
            var result = new List<UserAuthRespon>();
            using var db = new NDatabase();
            var items = await db.FetchAsync<Auth>("select * from Auth where AuditUserId = @0", userId);
            foreach (var item in items)
            {
                var authinfo = await db.SingleOrDefaultAsync<UserAuthRespon>("select * from UserAuthInfo where UserAuthInfoId = @0", item.UserAuthInfoId);
                authinfo.PortraitImage = item.PortraitImage;
                authinfo.NationalImage = item.NationalImage;
                authinfo.HandHeldImage = item.HandHeldImage;
                //基本信息处理
                if (item.AuditStep == AuditStepEnum.初审)
                {
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(0, 4).Insert(0, "****");
                    authinfo.IdCard = authinfo.IdCard.Remove(0, 4).Insert(0, "****");
                }
                else if (item.AuditStep == AuditStepEnum.二审)
                {
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(authinfo.PhoneNum.Length - 4, 4).Insert(authinfo.PhoneNum.Length - 4, "****");
                    authinfo.IdCard = authinfo.IdCard.Remove(authinfo.IdCard.Length - 4, 4).Insert(authinfo.IdCard.Length - 4, "****");
                }
                else if (item.AuditStep == AuditStepEnum.初审)
                {
                    authinfo.PhoneNum = authinfo.PhoneNum.Remove(3, 4).Insert(3, "****");
                    authinfo.IdCard = authinfo.IdCard.Remove(authinfo.IdCard.Length - 4, 4).Insert(authinfo.IdCard.Length - 4, "****");
                }
                var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 order by AuditStep", item.UserAuthInfoId);
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
                authinfo.Auths = list;
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
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response> UploadImage(IFormFile file, string userId)
        {
            try
            {
                var dir = new DirectoryInfo(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Images/AuthImges/" + userId + "/"));
                
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
            //var list = await db.SingleOrDefaultAsync<string>("select UserAuthInfoId from DIDUSer where Uid =@0", info.CreatorId);
            //if(!string.IsNullOrEmpty(list))
            //    return InvokeResult.Fail("请勿重复提交!");
            var str = await db.SingleOrDefaultAsync<AuthTypeEnum>("select AuthType from DIDUser where DIDUserId = @0", info.CreatorId);
            if (str != AuthTypeEnum.未审核)
                return InvokeResult.Fail("请勿重复提交!");

            var auth = new Auth
            {
                AuthId = Guid.NewGuid().ToString(),
                UserAuthInfoId = info.UserAuthInfoId,
                AuditUserId = await db.SingleOrDefaultAsync<string>("select RefUserId from DIDUser where DIDUserId = @0", info.CreatorId),//推荐人审核
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
            await db.ExecuteAsync("update DIDUser set UserAuthInfoId = @0,AuthType = @2 where DIDUserId = @1", info.UserAuthInfoId, info.CreatorId, AuthTypeEnum.审核中);//更新用户当前认证编号 审核中
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

        /// <summary>
        /// 获取用户审核成功信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<AuthSuccessRespon>> GetAuthSuccess(string userId)
        {
            using var db = new NDatabase();
            var item = new AuthSuccessRespon();
            var authinfo = await db.SingleOrDefaultAsync<UserAuthInfo>("select b.* from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0 and a.AuthType = 2", userId);
            if (authinfo == null) InvokeResult.Success("认证信息未找到!");
            item.Name = authinfo!.Name;
            item.PhoneNum = authinfo.PhoneNum;
            item.IdCard = authinfo.IdCard;
            item.RefUid = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId" +
                " where a.DIDUserId = (select RefUserId from DIDUser where DIDUserId = @0)", userId);
            item.Mail = await db.SingleOrDefaultAsync<string>("select Mail from DIDUser where DIDUserId =@0", userId);
            var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 order by AuditStep", authinfo.UserAuthInfoId);
            var list = new List<AuthInfo>();
            foreach (var auth in auths)
            {
                list.Add(new AuthInfo()
                {
                    UId = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", auth.AuditUserId),
                    AuditStep = auth.AuditStep,
                    AuthDate = auth.AuditDate,
                    Name = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0 and a.AuthType = 3", auth.AuditUserId),
                    AuditType = auth.AuditType,
                    Remark = auth.Remark
                });
            }
            item.Auths = list;
            return InvokeResult.Success(item);
        }

        /// <summary>
        /// 获取用户审核失败信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<AuthFailRespon>> GetAuthFail(string userId)
        {
            using var db = new NDatabase();
            var item = new AuthFailRespon();
            var authinfo = await db.SingleOrDefaultAsync<UserAuthInfo>("select b.* from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", userId);
            if (authinfo == null) InvokeResult.Success("认证信息未找到!");
            item.Name = authinfo!.Name;
            item.PhoneNum = authinfo.PhoneNum;
            item.IdCard = authinfo.IdCard;
            item.NationalImage = authinfo.NationalImage;
            item.PortraitImage = authinfo.PortraitImage;
            item.HandHeldImage = authinfo.HandHeldImage;
            var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 order by AuditStep Desc", authinfo.UserAuthInfoId);
            if (auths == null) InvokeResult.Success("认证信息未找到!");
            item.Remark = auths[0].Remark;
            item.AuditType = auths[0].AuditType;
            return InvokeResult.Success(item);
        }  
    }
}
