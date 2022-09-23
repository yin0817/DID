using Dao.Common;
using Dao.Entity;
using Dao.Models.Base;
using Dao.Models.Request;
using Dao.Models.Response;
using DID.Common;
using DID.Entitys;
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
        Task<Response<GetProposalRespon>> GetProposal(string proposalId, DaoBaseReq wallet);

        /// <summary>
        /// 获取提案列表
        /// </summary>
        /// <param name="type">0 最新10条 1 更多(所有)</param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <returns></returns>
        Task<Response<List<ProposalListRespon>>> GetProposalList(int type, long? page, long? itemsPerPage);

        /// <summary>
        /// 获取我的提案列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<Response<List<ProposalListRespon>>> GetMyProposalList(DaoBaseReq req);

        /// <summary>
        /// 取消提案
        /// </summary>
        /// <param name="proposalId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<Response> CancelProposal(DaoBaseReq req, string proposalId);

        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<Response<int>> ProposalVote(ProposalVoteReq req);
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
            var walletId = WalletHelp.GetWalletId(req);
            var userId = WalletHelp.GetUserId(req);
            var item = new Proposal
            {
                ProposalId = Guid.NewGuid().ToString(),
                WalletId = walletId,
                Summary = req.Summary,
                Title = req.Title,
                CreateDate = DateTime.Now,
                //IsCancel = IsCancelEnum.未取消,
                State = StateEnum.进行中,
                DIDUserId = userId
            };
            await db.InsertAsync(item);
            return InvokeResult.Success("提交成功!");
        }

        /// <summary>
        /// 获取提案详情
        /// </summary>
        /// <param name="proposalId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response<GetProposalRespon>> GetProposal(string proposalId, DaoBaseReq req)
        {
            using var db = new NDatabase();
            var item = await db.SingleOrDefaultByIdAsync<Proposal>(proposalId);
            if(null == item)
                return InvokeResult.Fail<GetProposalRespon>("提案信息未找到!");

            var wallet = await db.SingleOrDefaultByIdAsync<Wallet>(item.WalletId);

            var userId = await db.SingleOrDefaultAsync<string>("select b.DIDUserId from Wallet a left join DIDUser b on a.DIDUserId = b.DIDUserId " +
               "where a.WalletAddress = @0 and a.Otype = @1 and a.Sign = @2 and a.IsLogout = 0 and a.IsDelete = 0", req.WalletAddress, req.Otype, req.Sign);

            var userVoteId = await db.SingleOrDefaultAsync<string>("select UserVoteId from UserVote where DIDUserId = @0 and ProposalId = @1",
                userId, proposalId);


            var model = new GetProposalRespon()
            {
                WalletAddress = wallet.WalletAddress,
                Title = item.Title,
                State = item.State,
                CreateDate = item.CreateDate,
                Summary = item.Summary,
                ProposalId = proposalId,
                FavorVotes = item.FavorVotes,
                OpposeVotes = item.OpposeVotes,
                PeopleNum = item.PeopleNum,
                IsVote = string.IsNullOrEmpty(userVoteId) ? 0 : 1
            };
            return InvokeResult.Success(model);
        }

        /// <summary>
        /// 获取提案列表
        /// </summary>
        /// <param name="type">0 最新10条 1 更多(所有)</param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <returns></returns>
        public async Task<Response<List<ProposalListRespon>>> GetProposalList(int type, long? page, long? itemsPerPage)
        {
            using var db = new NDatabase();
            var list = new List<ProposalListRespon>();
            if (type == 0)//最新10条
            {
                var items = await db.FetchAsync<Proposal>("select top 10 * from Proposal order by CreateDate Desc");
                list = items.Select(x => new ProposalListRespon() { 
                    State = x.State,
                    Title = x.Title,
                    Total = x.FavorVotes + x.OpposeVotes,
                    ProposalId = x.ProposalId
                }).ToList();
            }
            else if (type == 1)//所有
            {
                var items = (await db.PageAsync<Proposal>(page??0,itemsPerPage??0,"select * from Proposal order by CreateDate Desc")).Items;
                list = items.Select(x => new ProposalListRespon()
                {
                    State = x.State,
                    Title = x.Title,
                    Total = x.FavorVotes + x.OpposeVotes,
                    ProposalId = x.ProposalId
                }).ToList();
            }
            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 获取我的提案列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response<List<ProposalListRespon>>> GetMyProposalList(DaoBaseReq req)
        {
            using var db = new NDatabase();
            var list = new List<ProposalListRespon>();
            //var walletId = WalletHelp.GetWalletId(req);
            var userId = WalletHelp.GetUserId(req);
            var items = await db.FetchAsync<Proposal>("select * from Proposal where DIDUserId = @0 order by CreateDate Desc", userId);
            list = items.Select(x => new ProposalListRespon()
            {
                State = x.State,
                Title = x.Title,
                Total = x.FavorVotes + x.OpposeVotes,
                ProposalId = x.ProposalId
            }).ToList();
            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 取消提案
        /// </summary>
        /// <param name="req"></param>
        /// <param name="proposalId"></param>
        /// <returns></returns>
        public async Task<Response> CancelProposal(DaoBaseReq req,string proposalId)
        {
            var walletId = WalletHelp.GetWalletId(req);
            using var db = new NDatabase();
            var item = await db.SingleOrDefaultByIdAsync<Proposal>(proposalId);
            if (null == item)
                return InvokeResult.Fail("提案信息未找到!");
            if (item.WalletId != walletId)
                return InvokeResult.Fail("取消失败!");
            if (item.State == StateEnum.已终止)
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
        public async Task<Response<int>> ProposalVote(ProposalVoteReq req)
        {
            var walletId = WalletHelp.GetWalletId(req);

            using var db = new NDatabase();

            var userId = await db.SingleOrDefaultAsync<string>("select b.DIDUserId from Wallet a left join DIDUser b on a.DIDUserId = b.DIDUserId " +
                "where a.WalletAddress = @0 and a.Otype = @1 and a.Sign = @2 and a.IsLogout = 0 and a.IsDelete = 0", req.WalletAddress, req.Otype, req.Sign);

            var userVoteId = await db.SingleOrDefaultAsync<string>("select UserVoteId from UserVote where DIDUserId = @0 and ProposalId = @1",
                userId, req.ProposalId);
            if (!string.IsNullOrEmpty(userVoteId))
                return InvokeResult.Fail<int>("请勿重复投票!");

            var mail = await db.SingleOrDefaultAsync<string>("select b.Mail from Wallet a left join DIDUser b on a.DIDUserId = b.DIDUserId " +
                "where a.WalletAddress = @0 and a.Otype = @1 and a.Sign = @2 and a.IsLogout = 0 and a.IsDelete = 0",
                req.WalletAddress, req.Otype, req.Sign);
            //todo: 通过用户邮箱获取用户票数
            var voteNum = 1;

            var item = await db.SingleOrDefaultByIdAsync<Proposal>(req.ProposalId);
            if(null == item)
                return InvokeResult.Fail<int>("提案信息未找到!");
            if (item.State != StateEnum.进行中)
                return InvokeResult.Fail<int>("投票已结束!");
            if (req.Vote == VoteEnum.赞成)
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

            var userVote = new UserVote() { 
                UserVoteId = Guid.NewGuid().ToString(),
                ProposalId = req.ProposalId,
                DIDUserId = userId,
                Type = req.Vote,
                CreateDate = DateTime.Now,
                VoteNum = voteNum,
                WalletId = walletId
            };

            db.BeginTransaction();
            await db.InsertAsync(userVote);
            await db.UpdateAsync(item);
            db.CompleteTransaction();

            return InvokeResult.Success(voteNum);
        }
    }

}