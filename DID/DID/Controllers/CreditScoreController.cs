﻿using DID.Entitys;
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

        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        /// <param name="currentUser"></param>
        public CreditScoreController(ILogger<CreditScoreController> logger, ICreditScoreService service, ICurrentUser currentUser)
        {
            _logger = logger;
            _service = service;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 信用分操作(加分、 减分) 1 参数不合法! 2 用户未找到! 3 信用分不足!
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("creditscore")]
        public async Task<Response> CreditScore(CreditScoreReq req)
        {
            if (req.Fraction <= 0)
                return InvokeResult.Fail("1"); //参数不合法!
            return await _service.CreditScore(req);
        }

        /// <summary>
        /// 获取信用分记录和当前信用分
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcreditscore")]
        public async Task<Response<GetCreditScoreRespon>> GetCreditScore(/*string userId*/)
        {
            return await _service.GetCreditScore(_currentUser.UserId);
        }

    }
}
