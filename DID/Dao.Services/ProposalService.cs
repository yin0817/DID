using Dao.Entity;
using Dao.Models.Request;
using DID.Common;
using DID.Models.Base;
using Microsoft.Extensions.Logging;

namespace Dao.Services
{
    public interface IProposalService
    {
        /// <summary>
        /// 提交提案
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<Response> AddProposal(ProposalReq req);

        /// <summary>
        /// 获取提案详情
        /// </summary>
        /// <param name="proposalId"></param>
        /// <returns></returns>
        Task<Response<Proposal>> GetProposal(string proposalId);

        /// <summary>
        /// 获取提案列表
        /// </summary>
        /// <param name="type">0 最新10条 1 更多(所有)</param>
        /// <returns></returns>
        Task<Response<List<Proposal>>> GetProposalList(int type);

        /// <summary>
        /// 取消提案
        /// </summary>
        /// <param name="proposalId"></param>
        /// <returns></returns>
        Task<Response> CancelProposal(string proposalId);

        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<Response> ProposalVote(ProposalVoteReq req);
    }

    /// <summary>
    /// 提案服务
    /// </summary>
    public class ProposalService : IProposalService
    {
        private readonly ILogger<ProposalService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public ProposalService(ILogger<ProposalService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 提交提案
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response> AddProposal(ProposalReq req)
        {
            //todo: 10000EOTC 才能提案

            using var db = new NDatabase();
            var walletId = await db.SingleOrDefaultAsync<string>("select WalletId from Wallet where WalletAddress = @0 and Otype = @1 and Sign = @2 and IsLogout = 0 and IsDelete = 0",
                                                       req.WalletAddress, req.Otype, req.Sign);
            if(string.IsNullOrEmpty(walletId))
                return InvokeResult.Fail("钱包未找到!");

            var item = new Proposal
            {
                ProposalId = Guid.NewGuid().ToString(),
                WalletId = walletId,
                Summary = req.Summary,
                Title = req.Title,
                CreateDate = DateTime.Now,
                //IsCancel = IsCancelEnum.未取消,
                State = StateEnum.进行中
            };
            await db.InsertAsync(item);
            return InvokeResult.Success("提交成功!");
        }

        /// <summary>
        /// 获取提案详情
        /// </summary>
        /// <param name="proposalId"></param>
        /// <returns></returns>
        public async Task<Response<Proposal>> GetProposal(string proposalId)
        {
            using var db = new NDatabase();
            var item = await db.SingleOrDefaultByIdAsync<Proposal>(proposalId);
            return InvokeResult.Success(item);
        }

        /// <summary>
        /// 获取提案列表
        /// </summary>
        /// <param name="type">0 最新10条 1 更多(所有)</param>
        /// <returns></returns>
        public async Task<Response<List<Proposal>>> GetProposalList(int type)
        {
            using var db = new NDatabase();
            var list = new List<Proposal>();
            if (type == 0)//最新10条
            {
                list = await db.FetchAsync<Proposal>("select top 10 * from Proposal order by CreateDate Desc ");
            }
            else if (type == 1)//所有
            {
                list = await db.FetchAsync<Proposal>("select * from Proposal order by CreateDate Desc ");
            }
            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 取消提案
        /// </summary>
        /// <param name="proposalId"></param>
        /// <returns></returns>
        public async Task<Response> CancelProposal(string proposalId)
        {
            using var db = new NDatabase();
            var item = await db.SingleOrDefaultByIdAsync<Proposal>(proposalId);
            if(item.State == StateEnum.已终止)
                return InvokeResult.Fail("请勿重复设置!");
            item.State = StateEnum.已终止;
            await db.UpdateAsync(item);
            return InvokeResult.Success("取消成功!");
        }

        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response> ProposalVote(ProposalVoteReq req)
        {
            using var db = new NDatabase();

            var mail = await db.SingleOrDefaultAsync<string>("select b.Mail from Wallet a left join DIDUser b on a.DIDUserId = b.DIDUserId " +
                "where a.WalletAddress = @0 and a.Otype = @1 and a.Sign = @2 and a.IsLogout = 0 and a.IsDelete = 0",
                req.WalletAddress, req.Otype, req.Sign);
            //todo: 通过用户邮箱获取用户票数
            var voteNum = 1;

            var item = await db.SingleByIdAsync<Proposal>(req.ProposalId);

            if (req.Vote == VoteEnum.同意)
                item.FavorVotes += voteNum;
            else if (req.Vote == VoteEnum.反对)
                item.OpposeVotes += voteNum;

            item.PeopleNum++;

            //99人投票终止
            if (item.PeopleNum >= 99)
            {
                if (item.FavorVotes > item.OpposeVotes)
                    item.State = StateEnum.已通过;
            }

            return InvokeResult.Success("投票成功!");
        }
    }

}