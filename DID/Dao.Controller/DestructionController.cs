using Dao.Common;
using Dao.Common.ActionFilter;
using Dao.Entity;
using Dao.Models.Base;
using Dao.Models.Request;
using Dao.Models.Response;
using Dao.Services;
using DID.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dao.Controllers
{
    /// <summary>
    /// Dao 销毁记录
    /// </summary>
    [ApiController]
    [Route("api/destruction")]
    [AllowAnonymous]
    //[DaoActionFilter]
    public class DestructionController : Controller
    {
        private readonly ILogger<DestructionController> _logger;

        private readonly IDestructionService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public DestructionController(ILogger<DestructionController> logger, IDestructionService service)
        {
            _logger = logger;
            _service = service;
        }
        /// <summary>
        /// 添加销毁记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("adddestruction")]
        public async Task<Response> AddDestruction(Destruction req)
        { 
            return await _service.AddDestruction(req);
        }

        /// <summary>
        /// 查询销毁记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getdestruction")]
        public async Task<Response<List<Destruction>>> GetDestruction(GetDestructionReq req)
        {
            return await _service.GetDestruction(req);
        }

        /// <summary>
        /// 删除销毁记录
        /// </summary>
        /// <param name="destructionId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("destruction")]
        public async Task<Response> DeleteDestruction(string destructionId)
        {
            return await _service.DeleteDestruction(destructionId);
        }
    }
}
