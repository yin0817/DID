﻿using DID.Entitys;
using DID.Helps;
using DID.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NPoco;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DID.Services
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<UserInfoRespon>> GetUserInfo(string userId);

        /// <summary>
        /// 更新用户信息（邀请人 电报群 国家地区）
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Response> SetUserInfo(UserInfoRespon user);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        Task<Response<string>> Login(LoginReq login);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        Task<Response> Register(LoginReq login);

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        Task<Response> GetCode(string mail);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassWord"></param>
        /// <returns></returns>
        Task<Response> ChangePassword(string userId, string newPassWord);

        /// <summary>
        /// 用户注销
        /// </summary>
        /// <returns></returns>
        Task<Response> Logout(string userId);

        /// <summary>
        /// 获取团队信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="IsAuth"></param>
        /// <returns></returns>
        Task<Response<TeamInfo>> GetUserTeam(string userId, bool IsAuth);
    }
    /// <summary>
    /// 审核认证服务
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;

        private readonly IConfiguration _config;

        private readonly IMemoryCache _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="cache"></param>
        public UserService(ILogger<UserService> logger, IConfiguration config, IMemoryCache cache)
        {
            _logger = logger;
            _config = config;
            _cache = cache;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<UserInfoRespon>> GetUserInfo(string userId)
        {
            var userRespon = new UserInfoRespon();
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);

            userRespon.Uid = user.Uid;
            if(!string.IsNullOrEmpty(user.RefUserId))
                userRespon.RefUid = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", user.RefUserId);
            userRespon.CreditScore = user.CreditScore;
            userRespon.Mail = user.Mail;
            userRespon.Country = user.Country;
            userRespon.Area = user.Area;
            userRespon.Telegram = user.Telegram;
            userRespon.AuthType = user.AuthType;
            if (user.AuthType == AuthTypeEnum.审核成功)
            {
                var authInfo = await db.SingleOrDefaultByIdAsync<UserAuthInfo>(user.UserAuthInfoId);
                userRespon.Name = authInfo.Name;
                userRespon.PhoneNum = authInfo.PhoneNum;
            }

            return InvokeResult.Success(userRespon);
        }

        /// <summary>
        /// 更新用户信息（邀请人 电报群 国家地区）
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Response> SetUserInfo(UserInfoRespon user)
        {
            using var db = new NDatabase();
            var sql = new Sql("update DIDUser set ");
            if (!string.IsNullOrEmpty(user.RefUserId))
            {
                var refUserId = await db.SingleOrDefaultAsync<string>("select DIDUserId from DIDUser where DIDUserId = @0", user.RefUserId);
                if (string.IsNullOrEmpty(refUserId) || user.UserId == refUserId)//不能修改为自己
                    return InvokeResult.Fail("1"); //邀请码错误!
                sql.Append("RefUserId = @0, ", user.RefUserId);
            }
            if (!string.IsNullOrEmpty(user.Telegram))
                sql.Append("Telegram = @0, ", user.Telegram);
            if(!string.IsNullOrEmpty(user.Country))
                sql.Append("Country = @0, ", user.Country);
            if (!string.IsNullOrEmpty(user.Area))
                sql.Append("Area = @0, ", user.Country);
            sql.Append("DIDUserId = @0 where DIDUserId = @0 ", user.UserId);

            await db.ExecuteAsync(sql);
            return InvokeResult.Success("更新成功!");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<Response<string>> Login(LoginReq login)
        {
            //1.验证用户账号密码是否正确
            using var db = new NDatabase();
           var user = new DIDUser();
            if (!string.IsNullOrEmpty(login.Mail) && !string.IsNullOrEmpty(login.Password))//邮箱密码登录
            {
                user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where Mail = @0", login.Mail);

                if (null == user)
                    return InvokeResult.Fail<string>("2");//邮箱未注册!

                if (user.PassWord != login.Password)
                    return InvokeResult.Fail<string>("3");//密码错误!
            }

            if (!string.IsNullOrEmpty(login.WalletAddress) && !string.IsNullOrEmpty(login.Otype) && !string.IsNullOrEmpty(login.Sign))//钱包登录
            {
                var wallet = await db.SingleOrDefaultAsync<Wallet>("select * from Wallet where WalletAddress = @0 and Otype = @1 and Sign = @2",
                                                            login.WalletAddress, login.Otype, login.Sign);
                if(null != wallet && string.IsNullOrEmpty(user.DIDUserId))
                    user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", wallet.DIDUserId);

                if (!string.IsNullOrEmpty(user.DIDUserId))
                {
                    if (null == wallet)//绑定钱包到用户
                    {
                        var item = new Wallet
                        {
                            WalletId = Guid.NewGuid().ToString(),
                            Otype = login.Otype,
                            Sign = login.Sign,
                            WalletAddress = login.WalletAddress,
                            DIDUserId = user.DIDUserId,
                            CreateDate = DateTime.Now
                        };
                        await db.InsertAsync(item);
                    }
                    else if (wallet.DIDUserId != user.DIDUserId)
                        return InvokeResult.Fail<string>("4");//钱包地址错误!
                }
            }
            if(string.IsNullOrEmpty(user.DIDUserId))
                return InvokeResult.Fail<string>("5");//登录错误!

            //更新登录时间
            user.LoginDate = DateTime.Now;
            await db.UpdateAsync(user);

            //2.生成JWT
            //Header,选择签名算法
            var signingAlogorithm = SecurityAlgorithms.HmacSha256;
            //Payload,存放用户信息，下面我们放了一个用户id
            var claims = new[]
            {
                new Claim("UserId",user.DIDUserId)
            };
            //Signature
            //取出私钥并以utf8编码字节输出
            var secretByte = Encoding.UTF8.GetBytes(_config["Authentication:SecretKey"]);
            //使用非对称算法对私钥进行加密
            var signingKey = new SymmetricSecurityKey(secretByte);
            //使用HmacSha256来验证加密后的私钥生成数字签名
            var signingCredentials = new SigningCredentials(signingKey, signingAlogorithm);
            //生成Token
            var Token = new JwtSecurityToken(
                    issuer: _config["Authentication:Issuer"],        //发布者
                    audience: _config["Authentication:Audience"],    //接收者
                    claims: claims,                                         //存放的用户信息
                    notBefore: DateTime.Now,                             //发布时间
                    expires: DateTime.Now.AddDays(30),                      //有效期设置为30天
                    signingCredentials                                      //数字签名 
                );
            //生成字符串token
            var TokenStr = new JwtSecurityTokenHandler().WriteToken(Token);

            return InvokeResult.Success<string>(TokenStr);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<Response> Register(LoginReq login)
        {
            using var db = new NDatabase();

            //var Uid = await db.SingleOrDefaultAsync<int>("select IDENT_CURRENT('DIDUser') + 1");//用户自增id
            var userId = await db.SingleOrDefaultAsync<string>("select DIDUserId from DIDUser where Mail = @0", login.Mail);
            var walletId = await db.SingleOrDefaultAsync<string>("select WalletId from Wallet where WalletAddress = @0 and Otype = @1 and Sign = @2",
                                                        login.WalletAddress, login.Otype, login.Sign);
            if (!string.IsNullOrEmpty(userId) || !string.IsNullOrEmpty(walletId))
                return InvokeResult.Fail("3");//

            if (!string.IsNullOrEmpty(login.RefUserId))
            {
                var refUserId = await db.SingleOrDefaultAsync<string>("select DIDUserId from DIDUser where DIDUserId = @0", login.RefUserId);
                if (string.IsNullOrEmpty(refUserId))
                    return InvokeResult.Fail("4"); //邀请码错误!
            }

            db.BeginTransaction();
            userId = Guid.NewGuid().ToString();
            var user = new DIDUser
            {
                DIDUserId = userId,
                PassWord = login.Password,
                AuthType = AuthTypeEnum.未审核,
                CreditScore = 0,
                Mail = login.Mail,
                RefUserId = login.RefUserId ?? login.RefUserId,
                UserNode = 0,//啥也不是
                RegDate = DateTime.Now             
            };
            await db.InsertAsync(user);
            //uId = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where Mail = @0", login.Mail);//获取主键
            if (!string.IsNullOrEmpty(login.WalletAddress) && !string.IsNullOrEmpty(login.Otype) && !string.IsNullOrEmpty(login.Sign))//有钱包时
            {
                var wallet = new Wallet
                {
                    WalletId = Guid.NewGuid().ToString(),
                    Otype = login.Otype,
                    Sign = login.Sign,
                    WalletAddress = login.WalletAddress,
                    DIDUserId = userId,
                    CreateDate = DateTime.Now
                };
                await db.InsertAsync(wallet);
            }
            db.CompleteTransaction();

            return InvokeResult.Success("用户注册成功!");
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public async Task<Response> GetCode(string mail)
        {
            var sb = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < 6; i++)//6位验证码
            {
                sb.Append(random.Next(0, 9));
            }
            var code = sb.ToString();
            _cache.Set(mail, code, new TimeSpan(0, 10, 0));//十分钟过期
            //todo 发送邮件
            //return InvokeResult.Success("验证码发送成功!");
            return InvokeResult.Success(code);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassWord"></param>
        /// <returns></returns>
        public async Task<Response> ChangePassword(string userId, string newPassWord)
        {
            using var db = new NDatabase();
            await db.ExecuteAsync("update DIDUser set PassWord = @0 where DIDUserId = @1", newPassWord, userId);
            return InvokeResult.Success("修改成功!");
        }

        /// <summary>
        /// 用户注销
        /// </summary>
        /// <returns></returns>
        public async Task<Response> Logout(string userId)
        {
            using var db = new NDatabase();
            //todo:判断注销条件
            db.BeginTransaction();
            await db.ExecuteAsync("update DIDUser set IsLogout = @0 where DIDUserId = @1", IsEnum.是, userId);
            var refUserId = await db.SingleOrDefaultAsync<string>("select RefUserId from DIDUser where DIDUserId = @0", userId);
            await db.ExecuteAsync("update DIDUser set RefUserId = @0 where RefUserId = @1", refUserId, userId);//更新邀请人为当前用户的上级
            db.CompleteTransaction();
            return InvokeResult.Success("注销成功!");
        }

        /// <summary>
        /// 获取团队信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="IsAuth"></param>
        /// <returns></returns>
        public async Task<Response<TeamInfo>> GetUserTeam(string userId,bool IsAuth)
        {
            using var db = new NDatabase();
            var model = new TeamInfo();
            model.TeamNumber = await db.FirstOrDefaultAsync<int>(";with temp as \n" +
                        "(select DIDUserId from DIDUser where DIDUserId = @0\n" +
                        "union all \n" +
                        "select a.DIDUserId from DIDUser a inner join temp on a.RefUserId = temp.DIDUserId) \n" +
                        "select Count(*) from temp", userId);

            //默认展示6级
            var list = await db.FetchAsync<DIDUser>(";with temp as \n" +
                                                    "(select *,0 Level from DIDUser where DIDUserId = @0\n" +
                                                    "union all \n" +
                                                    "select a.*,temp.Level+1 Level  from DIDUser a inner join temp on a.RefUserId = temp.DIDUserId WHERE temp.Level < 6) \n" +
                                                    "select * from temp ", userId);

            //todo:dao审核通过可以看所有数据

            //根据标签过滤数据
            if(IsAuth)
                list = list.Where(a => a.AuthType == AuthTypeEnum.审核成功).ToList();

            var users = list.Select(a => new TeamUser()
                            {
                                Grade = a.UserNode.ToString(),
                                UID = a.Uid,
                                RegDate = a.RegDate,
                                Name = db.SingleOrDefault<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", a.UserAuthInfoId)
                            }).ToList();
            
            model.Users = users;

            return InvokeResult.Success(model);
        }
    }
}
