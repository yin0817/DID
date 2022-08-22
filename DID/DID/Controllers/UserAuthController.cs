using DID.Entitys;
using DID.Helps;
using DID.Models;
using DID.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public UserAuthController(ILogger<UserAuthController> logger, IUserAuthService service)
        {
            _logger = logger;
            _service = service;
        }
        /// <summary>
        /// 认证图片上传
        /// </summary>
        /// <param name="upload"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadimage")]
        public async Task<Response> UploadImage([FromForm] Upload upload)
        {
            var files = Request.Form.Files;
            if (files.Count == 0) return InvokeResult.Fail("请上传文件!");
            if (!CommonHelp.IsPicture(files[0])) return InvokeResult.Fail("文件类型错误!");

            return await _service.UploadImage(files[0],upload);
        }

        /// <summary>
        /// 提交审核信息
        /// </summary>
        /// <param name="info">姓名 手机号 证件号 创建用户编号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("upload")]
        public async Task<Response> UploadUserInfo(UserAuthInfo info)
        {
            if(!CommonHelp.IsPhoneNum(info.PhoneNum))
                return InvokeResult.Fail("手机号错误!");
            if(!CommonHelp.IsIDcard(info.IdCard))
                return InvokeResult.Fail("证件号错误!");
            info.PortraitImage = "Images/AuthImges/" + info.CreatorId + "/" + info.PortraitImage;
            info.NationalImage = "Images/AuthImges/" + info.CreatorId + "/" + info.NationalImage;
            info.HandHeldImage = "Images/AuthImges/" + info.CreatorId + "/" + info.HandHeldImage;
            if (!System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, info.PortraitImage)) || 
                !System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, info.NationalImage)) || 
                !System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, info.HandHeldImage)))
                return InvokeResult.Fail("请上传认证图片!");
            return await _service.UploadUserInfo(info);
        }

        /// <summary>
        /// 获取未审核信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getunauditedinfo")]
        public async Task<Response<List<UserAuthRespon>>> GetUnauditedInfo(int uId)
        {
            return await _service.GetUnauditedInfo(uId);
        }

        /// <summary>
        /// 获取已审核审核信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getauditedinfo")]
        public async Task<Response<List<UserAuthRespon>>> GetAuditedInfo(int uId)
        {
            return await _service.GetAuditedInfo(uId);
        }

        /// <summary>
        /// 获取打回信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbackinfo")]
        public async Task<Response<List<UserAuthRespon>>> GetBackInfo(int uId)
        {
            return await _service.GetBackInfo(uId);
        }


        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="userAuthInfoId">审核记录编号</param>
        /// <param name="uId">用户编号</param>
        /// <param name="auditType">审核类型</param>
        /// <returns></returns>
        [HttpGet]
        [Route("auditinfo")]
        public async Task<Response> AuditInfo(string userAuthInfoId, int uId, AuditTypeEnum auditType)
        {
            return await _service.AuditInfo(userAuthInfoId, uId, auditType);
        }
    }
}
