
using App.Entity;
using App.Models.Request;
using App.Services;
using DID.Common;
using DID.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace App.Controllers
{
    /// <summary>
    /// APP订单接口
    /// </summary>
    [ApiController]
    [Route("api/order")]
    [AllowAnonymous]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        private readonly IOrderService _service;

        private readonly ICurrentUser _currentUser;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public OrderController(ILogger<OrderController> logger, IOrderService service, ICurrentUser currentUser)
        {
            _logger = logger;
            _service = service;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("order")]
        public async Task<Response<List<Order>>> GetOrder()
        {
            return await _service.GetOrder();
        }
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("orderbyid")]
        public async Task<Response<Order>> GetOrder(string id)
        {
            return await _service.GetOrder(id);
        }

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("order")]
        public async Task<Response> AddOrder(AddOrderReq req)
        {
            return await _service.AddOrder(req, _currentUser.UserId);
        }
        /// <summary>
        /// 更新订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("order")]
        public async Task<Response> UpdateOrder(Order req)
        {
            return await _service.UpdateOrder(req);
        }
        /// <summary>
        /// 删除订单
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("order")]
        public async Task<Response> DeleteOrder(string id)
        {
            return await _service.DeleteOrder(id);
        }
    }
}
