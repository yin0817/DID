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

        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        /// <param name="cache"></param>
        /// <param name="currentUser"></param>
        public UserController(ILogger<UserController> logger, IUserService service, IMemoryCache cache, ICurrentUser currentUser)
        {
            _logger = logger;
            _service = service;
            _cache = cache;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getuserinfo")]
        public async Task<Response<UserInfoRespon>> GetUserInfo(/*int uid*/)
        {
            return await _service.GetUserInfo(_currentUser.UserId);
        }

        /// <summary>
        /// 更新用户信息（邀请人 电报群 国家地区） 1 邀请码错误!
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("setuserinfo")]
        public async Task<Response> SetUserInfo(UserInfoRespon user)
        {
            user.UserId = _currentUser.UserId;
            return await _service.SetUserInfo(user);
        }

        /// <summary>
        /// 登录 1 邮箱格式错误! 2 邮箱未注册! 3 密码错误! 4 钱包地址错误! 5 登录错误!
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<Response<string>> Login(LoginReq login)
        {
           if (!string.IsNullOrEmpty(login.Mail) && !CommonHelp.IsMail(login.Mail))
                return InvokeResult.Fail<string>("1");
            //var code = _cache.Get(login.Mail)?.ToString();
            //if (code != login.Code)
            //    return InvokeResult.Fail<string>("验证码错误!");
            return await _service.Login(login);
        }

        /// <summary>
        /// 注册 1 邮箱格式错误! 2 验证码错误! 3 请勿重复注册! 4 邀请码错误!
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<Response> Register(LoginReq login)
        {
            if (!CommonHelp.IsMail(login.Mail))
                return InvokeResult.Fail<string>("1");//邮箱格式错误!
            var code = _cache.Get(login.Mail)?.ToString();
            if (code != login.Code)
                return InvokeResult.Fail<string>("2");//验证码错误!
            return await _service.Register(login);
        }


        /// <summary>
        /// 获取验证码 1 邮箱格式错误!
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcode")]
        [AllowAnonymous]
        public async Task<Response> GetCode(string mail)
        {
            if (!CommonHelp.IsMail(mail))
                return InvokeResult.Fail<string>("1");//邮箱格式错误!
            return await _service.GetCode(mail);
        }

        /// <summary>
        /// 修改密码 1 验证码错误!
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
                return InvokeResult.Fail<string>("1"); //验证码错误!
            return await _service.ChangePassword(_currentUser.UserId, newPassWord);
        }

        /// <summary>
        /// 获取邀请码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getrefuserid")]
        public async Task<Response<string>> GetRefUserId()
        {
            return InvokeResult.Success<string>(_currentUser.UserId);
        }

        /// <summary>
        /// 用户注销
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("logout")]
        public async Task<Response> Logout()
        {
            return await _service.Logout(_currentUser.UserId);
        }

        /// <summary>
        /// 获取团队信息
        /// </summary>
        /// <param name="IsAuth"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getuserteam")]
        public async Task<Response<TeamInfo>> GetUserTeam(bool IsAuth = false)
        {
            return await _service.GetUserTeam(_currentUser.UserId, IsAuth);
        }
    }
}
