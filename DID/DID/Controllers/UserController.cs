﻿using DID.Helps;
using DID.Models;
using DID.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DID.Controllers
{
    /// <summary>
    /// 审核认证
    /// </summary>
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        private readonly IUserService _service;

        private readonly IMemoryCache _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        /// <param name="cache"></param>
        public UserController(ILogger<UserController> logger, IUserService service, IMemoryCache cache)
        {
            _logger = logger;
            _service = service;
            _cache = cache;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getuserinfo")]
        public async Task<Response<UserInfoRespon>> GetUserInfo(/*int uid*/)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return InvokeResult.Fail<UserInfoRespon>("用户未找到!");
            return await _service.GetUserInfo(userId);
        }

        /// <summary>
        /// 更新用户信息（邀请人 电报群 国家地区）
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("setuserinfo")]
        public async Task<Response> SetUserInfo(UserInfoRespon user)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return InvokeResult.Fail<UserInfoRespon>("用户未找到!");
            user.UserId = userId;
            return await _service.SetUserInfo(user);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<Response<string>> Login(LoginReq login)
        {
           if (!CommonHelp.IsMail(login.Mail))
                return InvokeResult.Fail<string>("邮箱格式错误!");
            //var code = _cache.Get(login.Mail)?.ToString();
            //if (code != login.Code)
            //    return InvokeResult.Fail<string>("验证码错误!");
            return await _service.Login(login);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<Response> Register(LoginReq login)
        {
            if (!CommonHelp.IsMail(login.Mail))
                return InvokeResult.Fail<string>("邮箱格式错误!");
            var code = _cache.Get(login.Mail)?.ToString();
            if (code != login.Code)
                return InvokeResult.Fail<string>("验证码错误!");
            return await _service.Register(login);
        }


        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcode")]
        [AllowAnonymous]
        public async Task<Response> GetCode(string mail)
        {
            if (!CommonHelp.IsMail(mail))
                return InvokeResult.Fail<string>("邮箱格式错误!");
            return await _service.GetCode(mail);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="newPassWord"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("changepwd")]
        [AllowAnonymous]
        public async Task<Response> ChangePassword(string mail, string newPassWord, string code)
        {
            var usercode = _cache.Get(mail)?.ToString();
            if (usercode != code)
                return InvokeResult.Fail<string>("验证码错误!");
            return await _service.ChangePassword(mail, newPassWord);
        }
    }
}
