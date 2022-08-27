using DID.Entitys;
using DID.Helps;
using Microsoft.AspNetCore.Mvc;

namespace DID.Controllers
{
    /// <summary>
    /// 支付信息接口
    /// </summary>
    [ApiController]
    [Route("api/pay")]
    public class PayController : Controller
    {
        private readonly ILogger<PayController> _logger;

        private readonly IPayService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public PayController(ILogger<PayController> logger, IPayService service)
        {
            _logger = logger;
            _service = service;
        }
        /// <summary>
        /// 添加支付信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="mail"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("addpayment")]
        public async Task<Response> AddPayment(Payment req, string mail, string code)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return InvokeResult.Fail("用户未找到!");
            req.DIDUserId = userId;
            return await _service.AddPayment(req, mail, code);
        }

        /// <summary>
        /// 删除支付信息
        /// </summary>
        /// <param name="payId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletepayment")]
        public async Task<Response> DeletePayment(string payId)
        {
            return await _service.DeletePayment(payId);
        }
        /// <summary>
        /// 修改支付信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatepayment")]
        public async Task<Response> UpdatePayment(Payment req)
        {
            return await _service.UpdatePayment(req);
        }
        /// <summary>
        /// 获取支付信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpayment")]
        public async Task<Response<List<Payment>>> GetPayment()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return InvokeResult.Fail<List<Payment>>("用户未找到!");
            return await _service.GetPayment(userId);
        }
    }
}
