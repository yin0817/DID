using DID.Entitys;
using DID.Helps;
using DID.Models;
using DID.Services;
using Microsoft.AspNetCore.Mvc;

namespace DID.Controllers
{
    /// <summary>
    /// 审核认证
    /// </summary>
    [ApiController]
    [Route("api/creditscore")]
    public class CreditScoreController : Controller
    {
        private readonly ILogger<CreditScoreController> _logger;

        private readonly ICreditScoreService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public CreditScoreController(ILogger<CreditScoreController> logger, ICreditScoreService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// 信用分操作(加分、 减分)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("creditscore")]
        public async Task<Response> CreditScore(CreditScoreHistory item)
        {
            if (item.Fraction <= 0)
                return InvokeResult.Fail("参数不合法!");
            return await _service.CreditScore(item);
        }

        /// <summary>
        /// 获取信用分记录和当前信用分
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcreditscore")]
        public async Task<Response<GetCreditScoreRespon>> GetCreditScore(int uId)
        {
            return await _service.GetCreditScore(uId);
        }

    }
}
