﻿using Dao.Common;
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
    public interface IDaoUserService
    {
        /// <summary>
        /// 成为仲裁员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response> ToArbitrator(string userId);

        /// <summary>
        /// 成为审核员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response> ToAuditor(string userId);

        /// <summary>
        /// 获取Dao用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<GetDaoInfoRespon>> GetDaoInfo(string userId);
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
        /// <returns></returns>
        public async Task<Response> ToArbitrator(string userId)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);

            if (null == user)
            {
                return InvokeResult.Fail("用户信息未找到!");
            }
            if (user.IsArbitrate == IsEnum.是)
                return InvokeResult.Fail("请勿重复设置!");
            user.IsArbitrate = IsEnum.是;
                
           


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
                Number = number
            };

            db.BeginTransaction();
            await db.UpdateAsync(user);
            await db.InsertAsync(model);
            db.CompleteTransaction();

            return InvokeResult.Success("设置成功!");
        }

        /// <summary>
        /// 成为审核员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response> ToAuditor(string userId)
        {
            using var db = new NDatabase();
            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);

            if (null == user)
            {
                return InvokeResult.Fail("用户信息未找到!");
            }
            if (user.IsExamine == IsEnum.是)
                return InvokeResult.Fail("请勿重复设置!");
            user.IsExamine = IsEnum.是;

            await db.UpdateAsync(user);

            return InvokeResult.Success("设置成功!");
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
                AuthType = user.AuthType
            });
        }
    }
}