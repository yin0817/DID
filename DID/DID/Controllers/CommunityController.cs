using DID.Entitys;
using DID.Helps;
using DID.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DID.Controllers
{
    /// <summary>
    /// 社区相关接口
    /// </summary>
    [ApiController]
    [Route("api/community")]
    public class CommunityController : Controller
    {
        private readonly ILogger<CommunityController> _logger;

        private readonly ICommunityService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public CommunityController(ILogger<CommunityController> logger, ICommunityService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// 设置用户社区选择（未填邀请码） 1 请勿重复设置!
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("setcomselect")]
        public async Task<Response> SetComSelect(ComSelectReq req)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return InvokeResult.Error<ComSelect>(401);
            req.UserId = userId;
            return await _service.SetComSelect(req);
        }

        /// <summary>
        /// 获取选择社区位置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcomselect")]
        public async Task<Response<ComSelect>> GetComSelect()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return InvokeResult.Error<ComSelect>(401);
            return await _service.GetComSelect(userId);
        }

        /// <summary>
        /// 获取已有社区位置
        /// </summary>
        /// <returns> </returns>
        [HttpGet]
        [Route("getcomaddr")]
        public async Task<Response<ComAddrRespon>> GetComAddr()
        {
            return await _service.GetComAddr();
        }
    }
}
