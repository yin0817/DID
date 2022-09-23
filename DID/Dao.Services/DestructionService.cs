using Dao.Common;
using Dao.Entity;
using Dao.Models.Base;
using Dao.Models.Request;
using Dao.Models.Response;
using DID.Common;
using DID.Models.Base;
using Microsoft.Extensions.Logging;
using NPoco;

namespace Dao.Services
{
    /// <summary>
    /// 销毁查询服务接口
    /// </summary>
    public interface IDestructionService
    {
        /// <summary>
        /// 添加销毁记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<Response> AddDestruction(Destruction req);

        /// <summary>
        /// 查询销毁记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<Response<List<Destruction>>> GetDestruction(GetDestructionReq req);
    }

    /// <summary>
    /// 销毁查询服务
    /// </summary>
    public class DestructionService : IDestructionService
    {
        private readonly ILogger<DestructionService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public DestructionService(ILogger<DestructionService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 添加销毁记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response> AddDestruction(Destruction req)
        {
            using var db = new NDatabase();
            req.DestructionId = Guid.NewGuid().ToString();
            req.CreateDate = DateTime.Now;
            await db.InsertAsync(req);
            return InvokeResult.Success("添加成功");
        }


        /// <summary>
        /// 查询销毁记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response<List<Destruction>>> GetDestruction(GetDestructionReq req)
        {
            using var db = new NDatabase();
            var sql = new Sql("select * from Destruction where Memo like '%"+ req.KeyWord + "%'");
            if(null != req.BeginDate && null != req.EndDate)
                sql.Append(" and DestructionDate between @0 and @1", req.BeginDate, req.EndDate);
            sql.Append(" order by DestructionDate Desc");
            var list = (await db.PageAsync<Destruction>(req.Page, req.ItemsPerPage, sql)).Items;

            return InvokeResult.Success(list);
        }
    }
}