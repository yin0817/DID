

using App.Entity;
using App.Models.Request;
using DID.Common;
using DID.Models.Base;
using Microsoft.Extensions.Logging;
using Snowflake.Core;

namespace App.Services
{
    /// <summary>
    /// 订单接口
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <returns></returns>
        Task<Response<List<Order>>> GetOrder();
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <returns></returns>
        Task<Response<Order>> GetOrder(string id);
        /// <summary>
        /// 添加订单信息
        /// </summary>
        /// <returns></returns>
        Task<Response> AddOrder(AddOrderReq req,string userId);
        /// <summary>
        /// 更新订单信息
        /// </summary>
        /// <returns></returns>
        Task<Response> UpdateOrder(Order req);
        /// <summary>
        /// 删除订单信息
        /// </summary>
        /// <returns></returns>
        Task<Response> DeleteOrder(string id);
    }

    /// <summary>
    /// 订单服务
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<List<Order>>> GetOrder()
        {
            using var db = new NDatabase();
            var list = await db.FetchAsync<Order>("select * from App_Order where IsDelete = 0");

            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<Order>> GetOrder(string id)
        {
            using var db = new NDatabase();
            var model = await db.SingleOrDefaultByIdAsync<Order>(id);

            return InvokeResult.Success(model);
        }

        /// <summary>
        /// 添加订单信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response> AddOrder(AddOrderReq req, string userId)
        {
            using var db = new NDatabase();
            var model = new Order {
                OrderId = new IdWorker(1, 1).NextId().ToString(),
                Rid = req.Rid,
                Name = req.Name,
                CreateDate = DateTime.Now,
                DIDUserId = userId,
                OrderType = req.OrderType,
                Phone = req.Phone,
                Wechat = req.Wechat,
                Quantity = req.Quantity
            };
            await db.InsertAsync(model);

            return InvokeResult.Success("添加成功!");
        }

        /// <summary>
        /// 更新订单信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response> UpdateOrder(Order req)
        {
            using var db = new NDatabase();
            await db.UpdateAsync(req);

            return InvokeResult.Success("更新成功!");
        }
        /// <summary>
        /// 删除订单信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response> DeleteOrder(string id)
        {
            using var db = new NDatabase();
            var model = await db.SingleOrDefaultByIdAsync<Order>(id);
            model.IsDelete = DID.Entitys.IsEnum.是;
            await db.UpdateAsync(model);

            return InvokeResult.Success("删除成功!");
        }

    }
}