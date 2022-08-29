using DID.Entitys;
using DID.Helps;
using DID.Models;
using Microsoft.AspNetCore.Mvc;

namespace DID.Controllers
{
    /// <summary>
    /// 钱包相关接口
    /// </summary>
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : Controller
    {
        private readonly ILogger<WalletController> _logger;

        private readonly IWalletService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public WalletController(ILogger<WalletController> logger, IWalletService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// 绑定公链地址
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("setwallet")]
        public async Task<Response> SetWallet(Wallet req)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return InvokeResult.Error(401);
            req.DIDUserId = userId;
            return await _service.SetWallet(req);
        }

        /// <summary>
        /// 获取用户公链地址
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getwallets")]
        public async Task<Response<List<Wallet>>> GetWallets()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return InvokeResult.Error<List<Wallet>>(401);
            return await _service.GetWallets(userId);
        }
    }
}
