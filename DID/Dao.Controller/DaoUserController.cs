﻿using Dao.Common;
using Dao.Common.ActionFilter;
using Dao.Entity;
using Dao.Models.Base;
using Dao.Models.Request;
using Dao.Models.Response;
using Dao.Services;
using DID.Common;
using DID.Entitys;
using DID.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dao.Controllers
{
    /// <summary>
    /// Dao用户信息
    /// </summary>
    [ApiController]
    [Route("api/daouser")]
    [AllowAnonymous]
    [DaoActionFilter]
    public class DaoUserController : Controller
    {
        private readonly ILogger<DaoUserController> _logger;

        private readonly IDaoUserService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public DaoUserController(ILogger<DaoUserController> logger, IDaoUserService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// 成为仲裁员
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("toarbitrator")]
        public async Task<Response> ToArbitrator(DaoBaseReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.ToArbitrator(userId);
        }

        /// <summary>
        /// 成为审核员
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("toauditor")]
        public async Task<Response> ToAuditor(DaoBaseReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.ToAuditor(userId);
        }

        /// <summary>
        /// 获取Dao用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getdaoinfo")]
        public async Task<Response<GetDaoInfoRespon>> GetDaoInfo(DaoBaseReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetDaoInfo(userId);
        }
        /// <summary>
        /// 获取审核员信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getauditor")]
        public async Task<Response<GetAuditorRespon>> GetAuditor(DaoBaseReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetAuditor(userId);
        }

        /// <summary>
        /// 解除审核员身份
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("relieveauditor")]
        public async Task<Response> RelieveAuditor(DaoBaseReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.RelieveAuditor(userId);
        }

    }
}
