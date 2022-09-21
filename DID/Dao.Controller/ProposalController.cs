﻿using Dao.Common.ActionFilter;
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
    /// Dao 提案相关
    /// </summary>
    [ApiController]
    [Route("api/proposal")]
    [AllowAnonymous]
    [DaoActionFilter]
    public class ProposalController : Controller
    {
        private readonly ILogger<ProposalController> _logger;

        private readonly IProposalService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public ProposalController(ILogger<ProposalController> logger, IProposalService service)
        {
            _logger = logger;
            _service = service;
        }
        /// <summary>
        /// 提交提案
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("addproposal")]
        public async Task<Response> AddProposal(ProposalReq req)
        {
            return await _service.AddProposal(req);
        }

        /// <summary>
        /// 获取提案详情
        /// </summary>
        /// <param name="proposalId">提案编号</param>
        /// <param name="wallet">钱包参数</param>
        /// <returns></returns>
        [HttpPost]
        [Route("getproposal")]
        public async Task<Response<GetProposalRespon>> GetProposal(string proposalId, DaoBaseReq wallet)
        {
            return await _service.GetProposal(proposalId, wallet);
        }

        /// <summary>
        /// 获取我的提案列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getmyproposallist")]
        public async Task<Response<List<ProposalListRespon>>> GetMyProposalList(DaoBaseReq req)
        { 
            return await _service.GetMyProposalList(req);
        }

        /// <summary>
        /// 获取提案列表
        /// </summary>
        /// <param name="type">0 最新10条 1 更多(所有)</param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <returns></returns>
        [HttpPost]
        [Route("getproposallist")]
        public async Task<Response<List<ProposalListRespon>>> GetProposalList(int type, long? page, long? itemsPerPage)
        {
            return await _service.GetProposalList(type, page, itemsPerPage);
        }

        /// <summary>
        /// 取消提案
        /// </summary>
        /// <param name="proposalId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancelproposal")]
        public async Task<Response> CancelProposal(string proposalId)
        {
            return await _service.CancelProposal(proposalId);
        }

        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("proposalvote")]
        public async Task<Response<int>> ProposalVote(ProposalVoteReq req)
        {
            return await _service.ProposalVote(req);
        }
    }
}