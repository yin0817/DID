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
            var province_list = await db.FetchAsync<Area>("select * from Area");
            var city_list = await db.FetchAsync<Area>("select * from Area");
            var county_list = await db.FetchAsync<Area>("select * from Area");

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
    }
}
