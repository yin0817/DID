using Dao.Entity;
using Dao.Models;
using Dao.Services;
using DID.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dao.Controllers
{
    /// <summary>
    /// Dao 提案相关
    /// </summary>
    [ApiController]
    [Route("api/proposal")]
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
        /// <param name="proposalId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getproposal")]
        public async Task<Response<Proposal>> GetProposal(string proposalId)
        {
            return await _service.GetProposal(proposalId);
        }

        /// <summary>
        /// 获取提案列表
        /// </summary>
        /// <param name="type">0 最新10条 1 更多(所有)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getproposallist")]
        public async Task<Response<List<Proposal>>> GetProposalList(int type)
        {
            return await _service.GetProposalList(type);
        }

        /// <summary>
        /// 取消提案
        /// </summary>
        /// <param name="proposalId"></param>
        /// <returns></returns>
        [HttpGet]
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
        public async Task<Response> ProposalVote(ProposalVoteReq req)
        {
            return await _service.ProposalVote(req);
        }
    }
}