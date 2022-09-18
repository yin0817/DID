﻿using DID.Common;
using DID.Entitys;
using DID.Models.Base;
using DID.Models.Request;
using DID.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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

        /// <summary>
        /// 获取当前位置社区数量
        /// </summary>
        /// <param name="country"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        Task<Response<int>> GetComNum(string country, string province, string city, string area);

        /// <summary>
        /// 获取当前位置社区信息
        /// </summary>
        /// <param name="country"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        Task<Response<List<ComRespon>>> GetComList(string country, string province, string city, string area, long page, long itemsPerPage);

        /// <summary>
        /// 获取打回信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        Task<Response<List<ComAuthRespon>>> GetBackCom(string userId, long page, long itemsPerPage);

        /// <summary>
        /// 获取未审核信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        Task<Response<List<ComAuthRespon>>> GetUnauditedCom(string userId, long page, long itemsPerPage);

        /// <summary>
        /// 获取已审核审核信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        Task<Response<List<ComAuthRespon>>> GetAuditedCom(string userId, long page, long itemsPerPage);

        /// <summary>
        /// 社区申请审核
        /// </summary>
        /// <returns> </returns>
        Task<Response> AuditCommunity(string communityId, string userId, AuditTypeEnum auditType, string remark);

        /// <summary>
        /// 查询社区信息
        /// </summary>
        /// <returns> </returns>
        Task<Response<Community>> GetCommunityInfo(string userId);

        /// <summary>
        /// 添加社区信息
        /// </summary>
        /// <returns> </returns>
        Task<Response> AddCommunityInfo(Community item);

        /// <summary>
        /// 社区申请
        /// </summary>
        /// <returns> </returns>
        Task<Response> ApplyCommunity(Community item);
        /// <summary>
        /// 社区图片上传 1 请上传文件! 2 文件类型错误!
        /// </summary>
        /// <returns></returns>
        Task<Response> UploadImage(IFormFile file);

    }
    /// <summary>
    /// 社区服务
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
            var country_list = await db.FetchAsync<Area>("select Distinct a.Country code, b.Name from Community a left join Area b on a.Country = b.Code where AuthType = @0", AuthTypeEnum.审核成功);
            var province_list = await db.FetchAsync<Area>("select Distinct a.Province code, b.Name from Community a left join Area b on a.Province = b.Code where AuthType = @0", AuthTypeEnum.审核成功);
            var city_list = await db.FetchAsync<Area>("select Distinct a.City code, b.Name from Community a left join Area b on a.City = b.Code where AuthType = @0", AuthTypeEnum.审核成功);
            var county_list = await db.FetchAsync<Area>("select Distinct a.Area code, b.Name from Community a left join Area b on a.Area = b.Code where AuthType = @0", AuthTypeEnum.审核成功);

            var country = new Dictionary<string, string>();
            country_list.ForEach(a => country.Add(a.code, a.name));

            var provinces = new Dictionary<string, string>();
            province_list.ForEach(a => provinces.Add(a.code, a.name));

            var citys = new Dictionary<string, string>();
            city_list.ForEach(a => citys.Add(a.code, a.name));

            var countys = new Dictionary<string, string>();
            county_list.ForEach(a => countys.Add(a.code, a.name));

            var item = new ComAddrRespon(){
                country_list = country,
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
        public async Task<Response<int>> GetComNum(string country, string province, string city, string area)
        {
            using var db = new NDatabase();
            var num = await db.SingleOrDefaultAsync<int>("select count(1) from Community where Country = @0 and Province = @1 and City = @2 and Area = @3 and AuthType = @4", 
                country, province, city, area, AuthTypeEnum.审核成功);
            return InvokeResult.Success(num);
        }

        /// <summary>
        /// 获取当前位置社区信息
        /// </summary>
        /// <returns> </returns>
        public async Task<Response<List<ComRespon>>> GetComList(string country, string province, string city, string area, long page, long itemsPerPage)
        {
            using var db = new NDatabase();
            //var list = await db.FetchAsync<Community>("select * from Community where Country = @0 and Province = @1 and City = @2 and Area = @3 and AuthType = @4",
            //    country, province, city, area, AuthTypeEnum.审核成功);
            var list = (await db.PageAsync<Community>(page, itemsPerPage, "select * from Community where Country = @0 and Province = @1 and City = @2 and Area = @3 and AuthType = @4",
                country, province, city, area, AuthTypeEnum.审核成功)).Items;

            var result = new List<ComRespon>();
            foreach (var item in list)
            {
                result.Add(new ComRespon() { 
                    Name = item.ComName,
                    Describe = item.ComName,
                    Image = item.Image,
                    Telegram = item.Telegram
                });
            }

            return InvokeResult.Success(result);
        }

        /// <summary>
        /// 社区申请
        /// </summary>
        /// <returns> </returns>
        public async Task<Response> ApplyCommunity(Community item)
        {
            using var db = new NDatabase();
            item.CommunityId = Guid.NewGuid().ToString();

            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", item.DIDUserId);
            if (!string.IsNullOrEmpty(user.ApplyCommunityId))
            {
                var authType = await db.SingleOrDefaultAsync<AuthTypeEnum>("select AuthType from Community where CommunityId = @0", user.ApplyCommunityId);
                if(authType != AuthTypeEnum.未审核 && authType != AuthTypeEnum.审核失败)
                    return InvokeResult.Fail("请勿重复申请!");
            }
            user.ApplyCommunityId = item.CommunityId;
            
            var refUserId = await db.FirstOrDefaultAsync<string>("select RefUserId from DIDUser where DIDUserId = @0", item.DIDUserId);//邀请人
            if (string.IsNullOrEmpty(refUserId))
                return InvokeResult.Fail("邀请人未找到!");

            item.RefDIDUserId = refUserId;
            item.RefCommunityId = await db.FirstOrDefaultAsync<string>("select CommunityId from UserCommunity where DIDUserId = @0", refUserId);//邀请人社区
            item.CreateDate = DateTime.Now;
            item.AuthType = AuthTypeEnum.审核中;

            //邀请人审核
            var auth = new ComAuth
            {
                ComAuthId = Guid.NewGuid().ToString(),
                CommunityId = item.CommunityId,
                AuditUserId = refUserId,//推荐人审核
                CreateDate = DateTime.Now,
                AuditType = AuditTypeEnum.未审核,
                AuditStep = AuditStepEnum.初审
            };

            db.BeginTransaction();
            await db.InsertAsync(item);
            await db.InsertAsync(auth);
            await db.UpdateAsync(user);
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
            var model = await db.SingleOrDefaultByIdAsync<Community>(item.CommunityId);
            model.Image = "Images/ComImges/" + item.Image;
            model.Describe = item.Describe;
            model.Telegram = item.Telegram;
            model.Describe = item.Describe;
            model.QQ = item.QQ;
            await db.UpdateAsync(model);
            return InvokeResult.Success("添加成功!");
        }

        /// <summary>
        /// 查询社区信息
        /// </summary>
        /// <returns> </returns>
        public async Task<Response<Community>> GetCommunityInfo(string userId)
        {
            using var db = new NDatabase();
            var item = await db.SingleOrDefaultAsync<Community>("select * from Community a left join UserCommunity b on a.CommunityId = b.CommunityId where b.DIDUserId = @0", userId); 

            return InvokeResult.Success(item);
        }

        /// <summary>
        /// 社区申请审核
        /// </summary>
        /// <returns> </returns>
        public async Task<Response> AuditCommunity(string communityId, string userId, AuditTypeEnum auditType, string remark)
        {
            using var db = new NDatabase();
            var authinfo = await db.SingleByIdAsync<Community>(communityId);
            var auth = await db.SingleOrDefaultAsync<ComAuth>("select * from ComAuth where CommunityId = @0 and AuditUserId = @1", communityId, userId);

            //不会出现重复的记录 每个用户只审核一次
            if (auth.AuditType != AuditTypeEnum.未审核)
                return InvokeResult.Fail("已审核!");

            auth.AuditType = auditType;
            auth.Remark = remark;
            auth.AuditDate = DateTime.Now;

            db.BeginTransaction();
            await db.UpdateAsync(auth);

            //修改审核状态
            if (auth.AuditStep == AuditStepEnum.抽审 && auth.AuditType == AuditTypeEnum.审核通过)
            {
                await db.ExecuteAsync("update Community set AuthType = @1 where CommunityId = @0", communityId, AuthTypeEnum.审核成功);

                //更改用户社区信息为自己的社区
                await db.ExecuteAsync("update UserCommunity set CommunityId = @0 where DIDUserId = @1", communityId, authinfo.DIDUserId);
            }
            else if (auth.AuditType != AuditTypeEnum.审核通过)
            {
                //修改审核状态
                await db.ExecuteAsync("update Community set AuthType = @1 where CommunityId = @0", communityId, AuthTypeEnum.审核失败);
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

        /// <summary>
        /// 获取已审核审核信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public async Task<Response<List<ComAuthRespon>>> GetAuditedCom(string userId, long page, long itemsPerPage)
        {
            var result = new List<ComAuthRespon>();
            using var db = new NDatabase();
            //var items = await db.FetchAsync<ComAuth>("select * from ComAuth where AuditUserId = @0 and AuditType != 0", userId);
            var items = (await db.PageAsync<ComAuth>(page, itemsPerPage, "select * from ComAuth where AuditUserId = @0 and AuditType != 0", userId)).Items;
            foreach (var item in items)
            {
                var community = await db.SingleOrDefaultAsync<Community>("select * from Community where CommunityId = @0", item.CommunityId);
                var authinfo = new ComAuthRespon()
                {
                    CommunityId = community.CommunityId,
                    DIDUser = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", community.DIDUserId),
                    RefUId = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", community.RefDIDUserId),
                    RefName = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", community.RefDIDUserId),
                    Address = community.Address,
                    ComName = community.ComName,
                    Country  = community.Country,
                    Province = community.Province,
                    City = community.City,
                    Area = community.Area,
                    CreateDate = community.CreateDate,
                    Describe = community.Describe,
                    Discord = community.Discord,
                    HasGroup = community.HasGroup,
                    HasOffice = community.HasOffice,
                    Image = community.Image,
                    Mail = community.Mail,
                    Phone = community.Phone,
                    QQ = community.QQ,
                    RefCommunityName = await db.SingleOrDefaultAsync<string>("select a.ComName from Community a left join UserCommunity b on a.CommunityId = b.CommunityId where b.DIDUserId = @0", community.RefDIDUserId),
                    Telegram = community.Telegram
                };

                var auths = await db.FetchAsync<ComAuth>("select * from ComAuth where CommunityId = @0 order by AuditStep", item.CommunityId);
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
        /// 获取未审核信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public async Task<Response<List<ComAuthRespon>>> GetUnauditedCom(string userId, long page, long itemsPerPage)
        {
            var result = new List<ComAuthRespon>();
            using var db = new NDatabase();
            //var items = await db.FetchAsync<ComAuth>("select * from ComAuth where AuditUserId = @0 and AuditType = 0", userId);
            var items = (await db.PageAsync<ComAuth>(page, itemsPerPage, "select * from ComAuth where AuditUserId = @0 and AuditType = 0", userId)).Items;
            foreach (var item in items)
            {
                var community = await db.SingleOrDefaultAsync<Community>("select * from Community where CommunityId = @0", item.CommunityId);
                var authinfo = new ComAuthRespon()
                {
                    CommunityId = community.CommunityId,
                    DIDUser = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", community.DIDUserId),
                    RefUId = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", community.RefDIDUserId),
                    RefName = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", community.RefDIDUserId),
                    Address = community.Address,
                    ComName = community.ComName,
                    Country = community.Country,
                    Province = community.Province,
                    City = community.City,
                    Area = community.Area,
                    CreateDate = community.CreateDate,
                    Describe = community.Describe,
                    Discord = community.Discord,
                    HasGroup = community.HasGroup,
                    HasOffice = community.HasOffice,
                    Image = community.Image,
                    Mail = community.Mail,
                    Phone = community.Phone,
                    QQ = community.QQ,
                    RefCommunityName = await db.SingleOrDefaultAsync<string>("select a.ComName from Community a left join UserCommunity b on a.CommunityId = b.CommunityId where b.DIDUserId = @0", community.RefDIDUserId),
                    Telegram = community.Telegram
                };

                var auths = await db.FetchAsync<ComAuth>("select * from ComAuth where CommunityId = @0 order by AuditStep", item.CommunityId);
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
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public async Task<Response<List<ComAuthRespon>>> GetBackCom(string userId, long page, long itemsPerPage)
        {
            var result = new List<ComAuthRespon>();
            using var db = new NDatabase();
            //var items = await db.FetchAsync<ComAuth>("select * from ComAuth where AuditUserId = @0", userId);
            var items = (await db.PageAsync<ComAuth>(page, itemsPerPage, "select * from ComAuth where AuditUserId = @0", userId)).Items;
            foreach (var item in items)
            {
                var community = await db.SingleOrDefaultAsync<Community>("select * from Community where CommunityId = @0", item.CommunityId);
                var authinfo = new ComAuthRespon()
                {
                    CommunityId = community.CommunityId,
                    DIDUser = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", community.DIDUserId),
                    RefUId = await db.SingleOrDefaultAsync<int>("select Uid from DIDUser where DIDUserId = @0", community.RefDIDUserId),
                    RefName = await db.SingleOrDefaultAsync<string>("select b.Name from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0", community.RefDIDUserId),
                    Address = community.Address,
                    ComName = community.ComName,
                    Country = community.Country,
                    Province = community.Province,
                    City = community.City,
                    Area = community.Area,
                    CreateDate = community.CreateDate,
                    Describe = community.Describe,
                    Discord = community.Discord,
                    HasGroup = community.HasGroup,
                    HasOffice = community.HasOffice,
                    Image = community.Image,
                    Mail = community.Mail,
                    Phone = community.Phone,
                    QQ = community.QQ,
                    RefCommunityName = await db.SingleOrDefaultAsync<string>("select a.ComName from Community a left join UserCommunity b on a.CommunityId = b.CommunityId where b.DIDUserId = @0", community.RefDIDUserId),
                    Telegram = community.Telegram
                };
              
                var auths = await db.FetchAsync<ComAuth>("select * from ComAuth where CommunityId = @0 order by AuditStep", item.CommunityId);
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
        /// 社区图片上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response> UploadImage(IFormFile file)
        {
            try
            {
                var dir = new DirectoryInfo(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Images/ComImges/" ));

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
    }
}
