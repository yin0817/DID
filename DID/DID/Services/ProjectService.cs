using DID.Entitys;
using DID.Helps;
using DID.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DID.Controllers
{
    /// <summary>
    /// 项目相关接口
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// 获取绑定项目
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<List<UserProjectRespon>>> GetProjects(string userId);

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<Response> Unbind(string userId, string projectId);

        /// <summary>
        /// 绑定项目
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<Response> bind(string userId, string projectId);
    }
    /// <summary>
    ///  项目相关服务
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly ILogger<ProjectService> _logger;

        private readonly IMemoryCache _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cache"></param>
        public ProjectService(ILogger<ProjectService> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }
        /// <summary>
        /// 获取绑定项目
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<List<UserProjectRespon>>> GetProjects(string userId)
        {
            using var db = new NDatabase();
            var list = await db.FetchAsync<UserProjectRespon>("select c.ProjectId,c.Name from UserProject a left join DIDUser b on a.DIDUserId = b.DIDUserId " +
                "left join Project c on a.ProjectId = c.ProjectId where a.DIDUserId = @0", userId);
            return InvokeResult.Success(list);
        }
        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<Response> Unbind(string userId, string projectId)
        {
            using var db = new NDatabase();
            await db.ExecuteAsync("delete from UserProject where DIDUserId = @0 and ProjectId = @1", userId, projectId);
            return InvokeResult.Success("解绑成功!");
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<Response> bind(string userId, string projectId)
        {
            using var db = new NDatabase();
            await db.ExecuteAsync("insert into UserProject set UserProjectId = @0,DIDUserId = @1,ProjectId = @2", Guid.NewGuid().ToString(), userId, projectId);
            return InvokeResult.Success("绑定成功!");
        }

        /// <summary>
        /// 项目用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<Response> AddUserProject(string userId, string projectId)
        {
            using var db = new NDatabase();
            await db.ExecuteAsync("insert into UserProject set UserProjectId = @0,DIDUserId = @1,ProjectId = @2", Guid.NewGuid().ToString(), userId, projectId);
            return InvokeResult.Success("绑定成功!");
        }
    }
}
