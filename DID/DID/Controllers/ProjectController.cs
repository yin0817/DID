using DID.Entitys;
using DID.Helps;
using DID.Models;
using Microsoft.AspNetCore.Mvc;

namespace DID.Controllers
{
    /// <summary>
    /// 社区相关接口
    /// </summary>
    [ApiController]
    [Route("api/project")]
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;

        private readonly IProjectService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public ProjectController(ILogger<ProjectController> logger, IProjectService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// 获取绑定项目
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojects")]
        public async Task<Response<List<UserProjectRespon>>> GetProjects()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return InvokeResult.Error<List<UserProjectRespon>>(401);
            return await _service.GetProjects(userId);
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("unbind")]
        public async Task<Response> Unbind( string projectId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return InvokeResult.Error<List<UserProjectRespon>>(401);
            return await _service.Unbind(userId, projectId);
        }
    }
}
