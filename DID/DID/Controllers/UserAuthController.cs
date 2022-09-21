using DID.Common;
using DID.Entitys;
using DID.Models.Base;
using DID.Models.Response;
using DID.Services;
using Microsoft.AspNetCore.Mvc;

namespace DID.Controllers
{
    /// <summary>
    /// 审核认证
    /// </summary>
    [ApiController]
    [Route("api/userauth")]
    public class UserAuthController : Controller
    {
        private readonly ILogger<UserAuthController> _logger;

        private readonly IUserAuthService _service;

        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        /// <param name="currentUser"></param>
        public UserAuthController(ILogger<UserAuthController> logger, IUserAuthService service, ICurrentUser currentUser)
        {
            _logger = logger;
            _service = service;
            _currentUser = currentUser;
        }
        /// <summary>
        /// 认证图片上传 1 请上传文件! 2 文件类型错误!
        /// </summary>
        /// <param name="upload"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadimage")]
        public async Task<Response> UploadImage()
        {
            var files = Request.Form.Files;
            if (files.Count == 0) return InvokeResult.Fail("1");//请上传文件!
            if (!CommonHelp.IsPicture(files[0])) return InvokeResult.Fail("2");//文件类型错误!
            return await _service.UploadImage(files[0], _currentUser.UserId);
        }

        /// <summary>
        /// 提交审核信息 1 手机号错误! 2 证件号错误! 3 请上传认证图片! 4 请勿重复提交!
        /// </summary>
        /// <param name="info">姓名 手机号 证件号 创建用户编号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("upload")]
        public async Task<Response> UploadUserInfo(UserAuthInfo info)
        {
            //if (!CommonHelp.IsPhoneNum(info.PhoneNum))
            //    return InvokeResult.Fail("1");//手机号错误!
            //if (!CommonHelp.IsCard(info.IdCard))
            //    return InvokeResult.Fail("2");//证件号错误!

            info.CreatorId = _currentUser.UserId;
            info.PortraitImage = "Images/AuthImges/" + info.CreatorId + "/" + info.PortraitImage;
            info.NationalImage = "Images/AuthImges/" + info.CreatorId + "/" + info.NationalImage;
            info.HandHeldImage = "Images/AuthImges/" + info.CreatorId + "/" + info.HandHeldImage;
            if (!System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, info.PortraitImage)) || 
                !System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, info.NationalImage)) || 
                !System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, info.HandHeldImage)))
                return InvokeResult.Fail("3");//请上传认证图片!
            return await _service.UploadUserInfo(info);
        }

        /// <summary>
        /// 获取未审核信息
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getunauditedinfo")]
        public async Task<Response<List<UserAuthRespon>>> GetUnauditedInfo(long page, long itemsPerPage)
        {
            return await _service.GetUnauditedInfo(_currentUser.UserId, page, itemsPerPage);
        }

        /// <summary>
        /// 获取已审核审核信息
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getauditedinfo")]
        public async Task<Response<List<UserAuthRespon>>> GetAuditedInfo(long page, long itemsPerPage)
        {
            return await _service.GetAuditedInfo(_currentUser.UserId, page, itemsPerPage);
        }

        /// <summary>
        /// 获取打回信息
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbackinfo")]
        public async Task<Response<List<UserAuthRespon>>> GetBackInfo(long page, long itemsPerPage)
        {
            return await _service.GetBackInfo(_currentUser.UserId, page, itemsPerPage);
        }


        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="userAuthInfoId">审核记录编号</param>
        /// <param name="auditType">审核类型</param>
        /// <returns></returns>
        [HttpGet]
        [Route("auditinfo")]
        public async Task<Response> AuditInfo(string userAuthInfoId, AuditTypeEnum auditType, string remark)
        {
            return await _service.AuditInfo(userAuthInfoId, _currentUser.UserId, auditType, remark);
        }
        /// <summary>
        /// 获取用户审核失败信息 1 认证信息未找到!
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getauthfail")]
        public async Task<Response<AuthFailRespon>> GetAuthFail()
        {
            return await _service.GetAuthFail(_currentUser.UserId);
        }

        /// <summary>
        /// 获取用户审核成功信息 1 认证信息未找到!
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getauthsuccess")]
        public async Task<Response<AuthSuccessRespon>> GetAuthSuccess()
        {
            return await _service.GetAuthSuccess(_currentUser.UserId);
        }
    }
}
