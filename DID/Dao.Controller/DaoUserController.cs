using Dao.Common;
using Dao.Common.ActionFilter;
using Dao.Entity;
using Dao.Models.Base;
using Dao.Models.Request;
using Dao.Models.Response;
using Dao.Services;
using DID.Common;
using DID.Entitys;
using DID.Models.Base;
using DID.Models.Request;
using DID.Models.Response;
using DID.Services;
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

        private readonly IUserService _userservice;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        /// <param name="userservice"></param>
        public DaoUserController(ILogger<DaoUserController> logger, IDaoUserService service, IUserService userservice)
        {
            _logger = logger;
            _service = service;
            _userservice = userservice;
        }

        /// <summary>
        /// 成为仲裁员
        /// </summary>
        /// <param name="req"></param>
        /// <param name="score">考试分数</param>
        /// <returns></returns>
        [HttpPost]
        [Route("toarbitrator")]
        public async Task<Response<string>> ToArbitrator(ToArbitratorReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.ToArbitrator(userId, req.Score);
        }

        /// <summary>
        /// 成为审核员
        /// </summary>
        /// <param name="req"></param>
        /// <param name="score">考试分数</param>
        /// <returns></returns>
        [HttpPost]
        [Route("toauditor")]
        public async Task<Response<string>> ToAuditor(ToArbitratorReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.ToAuditor(userId, req.Score);
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

        /// <summary>
        /// 获取用户质押数量
        /// </summary>
        [HttpPost]
        [Route("getusereotc")]
        public async Task<Response<double>> GetUserEOTC(DaoBaseReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _userservice.GetUserEOTC(userId);
        }

        /// <summary>
        /// 获取认证图片
        /// </summary>
        [HttpPost]
        [Route("getauthimage")]
        public async Task<IActionResult> GetAuthImage(GetAuthImageReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _userservice.GetAuthImage(req.Path, userId);
        }

        /// <summary>
        /// 是否启用Dao审核仲裁权限
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("setdaoenable")]
        public async Task<Response> SetDaoEnable(SetDaoEnableReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.SetDaoEnable(userId, req.IsEnable);
        }

        /// <summary>
        /// 获取已审核审核员申请
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getauditorlist")]
        public async Task<Response<List<GetAuditorListRespon>>> GetAuditorList(DaoBasePageReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetAuditorList(req.Page, req.ItemsPerPage, userId, true, req.Key);
        }

        /// <summary>
        /// 获取未审核审核员申请
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getunauditorlist")]
        public async Task<Response<List<GetAuditorListRespon>>> GetUnAuditorList(DaoBasePageReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetAuditorList(req.Page, req.ItemsPerPage, userId, false, req.Key);
        }

        /// <summary>
        /// 获取已审核仲裁员申请
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getarbitratorslist")]
        public async Task<Response<List<GetArbitratorsListRespon>>> GetArbitratorsList(DaoBasePageReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetArbitratorsList(req.Page, req.ItemsPerPage, userId, true, req.Key);
        }

        /// <summary>
        /// 获取未审核仲裁员申请
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getunarbitratorslist")]
        public async Task<Response<List<GetArbitratorsListRespon>>> GetUnArbitratorsList(DaoBasePageReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetArbitratorsList(req.Page, req.ItemsPerPage, userId, false, req.Key);
        }

        /// <summary>
        /// 获取审核员申请信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getauditorinfo")]
        public async Task<Response<GetAuditorInfoRespon>> GetAuditorInfo(DaoBaseByIdReq req)
        {
            return await _service.GetAuditorInfo(req.Id);
        }


        /// <summary>
        /// 获取仲裁员申请信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getarbitratorsinfo")]
        public async Task<Response<GetArbitratorsInfoRespon>> GetArbitratorsInfo(DaoBaseByIdReq req)
        {
            return await _service.GetArbitratorsInfo(req.Id);
        }

        /// <summary>
        /// 审核员审核
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("auditauditor")]
        public async Task<Response> AuditAuditor(AuditAuditorReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.AuditAuditor(userId, req.Id, 1, req.State, req.Reason);
        }

        /// <summary>
        /// 仲裁员审核
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("auditarbitrators")]
        public async Task<Response> AuditArbitrators(AuditAuditorReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.AuditAuditor(userId, req.Id, 0, req.State, req.Reason);
        }

        /// <summary>
        /// 获取抽审用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getspotcheck")]
        public async  Task<Response<List<UserAuthRespon>>> GetSpotCheck(DaoBaseReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetSpotCheck(userId);
        }

        /// <summary>
        /// 高级节点身份信息打回
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("auditinfo")]
        public async Task<Response> AuditInfo(DaoAuditInfoReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.AuditInfo(req.UserAuthInfoId, userId, req.AuditType, req.Remark);
        }

        /// <summary>
        /// 获取抽审打回信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getbackinfo")]
        public async Task<Response<List<UserAuthRespon>>> GetBackInfo(DaoBasePageReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetBackInfo(userId, req.Page, req.ItemsPerPage, req.Key);
        }

        /// <summary>
        /// 获取身份认证申述列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getauthappeal")]
        public async Task<Response<List<GetAuthAppealRespon>>> GetAuthAppeal(GetAuthAppealReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetAuthAppeal(userId, req.Page, req.ItemsPerPage, req.Type);
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getuserinfo")]
        public async Task<Response<UserInfoRespon>> GetUserInfo(DaoBaseByIdReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _userservice.GetUserInfo(req.Id);
        }

        /// <summary>
        /// 获取身份认证申述详情
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getauthappealdetails")]
        public async Task<Response<GetAuthAppealDetailsRespon>> GetAuthAppealDetails(DaoBaseByIdReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetAuthAppealDetails(req.Id);
        }

        /// <summary>
        /// 认证申述审核
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("authappeal")]
        public async Task<Response> AuthAppeal(AuthReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.AuthAppeal(userId, req.Id, req.Type, req.Reason);
        }

        /// <summary>
        /// 获取是否有高级节点审核信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("gethasauth")]
        public async Task<Response<object>> GetHasAuth(DaoBaseReq req)
        {
            var userId = WalletHelp.GetUserId(req);
            return await _service.GetHasAuth(userId);
        }

    }
}
