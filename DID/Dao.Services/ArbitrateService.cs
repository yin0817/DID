using Dao.Common;
using Dao.Entity;
using Dao.Models.Base;
using Dao.Models.Request;
using Dao.Models.Response;
using DID.Common;
using DID.Entitys;
using DID.Models.Base;
using Microsoft.Extensions.Logging;
using NPoco;

namespace Dao.Services
{
    /// <summary>
    /// 仲裁服务接口
    /// </summary>
    public interface IArbitrateService
    {
        /// <summary>
        /// 获取审核员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<GetArbitratorRespon>> GetArbitrator(string userId);

        /// <summary>
        /// 解除仲裁员身份
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response> RelieveArbitrator(string userId);


        /// <summary>
        /// 获取仲裁员列表
        /// </summary>
        /// <returns></returns>
        Task<Response<List<GetArbitratorsRespon>>> GetArbitrators();

        /// <summary>
        /// 获取仲裁公示
        /// </summary>
        /// <returns></returns>
        Task<Response<List<GetArbitrateInfoRespon>>> GetArbitrateInfo();

        /// <summary>
        /// 获取仲裁详情
        /// </summary>
        /// param name="arbitrateInfoId"
        /// <returns></returns>
        Task<Response<GetArbitrateDetailsRespon>> GetArbitrateDetails(string arbitrateInfoId, string userId);

        /// <summary>
        /// 提交仲裁
        /// </summary>
        /// <param name="plaintiff"></param>
        /// <param name="defendant"></param>
        /// <param name="orderId"></param>
        /// <param name="num"></param>
        /// <param name="memo"></param>
        /// <param name="images"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<Response> AddArbitrateInfo(string plaintiff, string defendant, string orderId, int num, string memo, string images, ArbitrateInTypeEnum type);


        /// <summary>
        /// 获取待处理 已仲裁（原告、被告）
        /// </summary>
        /// <returns></returns>
        Task<Response<List<GetArbitrateInfoRespon>>> GetUserArbitrate(string userId, int type);

        /// <summary>
        /// 获取待仲裁 已结案列表 0 待仲裁 1 已仲裁
        /// </summary>
        /// <returns></returns>
        Task<Response<List<GetArbitrateInfoRespon>>> GetArbitrateList(string userId, int type);


        /// <summary>
        /// 仲裁员投票
        /// </summary>
        /// <returns></returns>
        Task<Response> ArbitrateVote(string arbitrateInfoId, string userId, string reason, VoteStatusEnum status);

        /// <summary>
        /// 申请延期
        /// </summary>
        /// <returns></returns>
        Task<Response> ArbitrateDelay(string arbitrateInfoId, string userId, ReasonEnum reason, string explain, int day, IsEnum isArbitrate);

        /// <summary>
        /// 申请延期投票
        /// </summary>
        /// <returns></returns>
        Task<Response> ArbitrateDelayVote(string delayVoteId, DelayVoteStatus status);

        /// <summary>
        /// 追加举证
        /// </summary>
        /// <returns></returns>
        Task<Response> AddAdduceList(string arbitrateInfoId, string userId, string memo, string images);

        /// <summary>
        /// 取消仲裁
        /// </summary>
        /// <returns></returns>
        Task<Response> CancelArbitrate(string userId, string arbitrateInfoId);

        /// <summary>
        /// 获取风险用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response<RiskUserInfo>> GetUserInfo(string userId);


        /// <summary>
        /// 获取仲裁消息列表
        /// </summary>
        /// <returns></returns>
        Task<Response<List<GetArbitrateMessageRespon>>> GetArbitrateMessage(string userId);

        /// <summary>
        /// 获取原被告延期消息
        /// </summary>
        /// <returns></returns>
        Task<Response<GetArbitrateDelayRespon>> GetArbitrateDelay(string id, IsEnum isArbitrate);

        /// <summary>
        /// 获取取消仲裁消息
        /// </summary>
        /// <returns></returns>
        Task<Response<GetCancelArbitrateRespon>> GetCancelArbitrate(string id);

        /// <summary>
        /// 获取追加举证消息
        /// </summary>
        /// <returns></returns>
        Task<Response<GetAdduceListRespon>> GetAdduceList(string id);

        /// <summary>
        /// 获取结案通知
        /// </summary>
        /// <returns></returns>
        Task<Response<GetClosureRespon>> GetClosure(string id);

        /// <summary>
        /// 设置消息为已读
        /// </summary>
        /// <returns></returns>
        Task<Response> SetMessageIsOpen(string messageId);

        /// <summary>
        /// 获取仲裁员是否有未读消息
        /// </summary>
        /// <returns></returns>
        Task<Response<bool>> GetMessageIsOpen(string userId);

        /// <summary>
        /// 获取被告待处理消息
        /// </summary>
        /// <returns></returns>
        Task<Response<int>> GetWaitMessage(string userId);
    }

    /// <summary>
    /// 仲裁服务
    /// </summary>
    public class ArbitrateService : IArbitrateService
    {
        private readonly ILogger<ArbitrateService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public ArbitrateService(ILogger<ArbitrateService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取仲裁员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<GetArbitratorRespon>> GetArbitrator(string userId)
        {
            var db = new NDatabase();

            var model = await db.SingleOrDefaultAsync<UserArbitrate>("select * from UserArbitrate where DIDUserId = @0 and IsDelete = 0", userId);


            return InvokeResult.Success(new GetArbitratorRespon
            {
                ArbitrateNum = model.ArbitrateNum,
                CreateDate = model.CreateDate,
                EOTC = model.EOTC,
                Number = model.Number,
                Name = WalletHelp.GetName(userId)
            });
        }


        /// <summary>
        /// 解除仲裁员身份
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response> RelieveArbitrator(string userId)
        {
            var db = new NDatabase();

            var user = await db.SingleOrDefaultAsync<DIDUser>("select * from DIDUser where DIDUserId = @0", userId);
            user.IsArbitrate = IsEnum.否;

            var model = await db.SingleOrDefaultAsync<UserArbitrate>("select * from UserArbitrate where DIDUserId = @0 and IsDelete = 0", userId);
            model.IsDelete = IsEnum.是;

            db.BeginTransaction();
            await db.UpdateAsync(user);
            await db.UpdateAsync(model);
            db.CompleteTransaction();

            return InvokeResult.Success("解除成功!");
        }

        /// <summary>
        /// 获取仲裁员列表
        /// </summary>
        /// <returns></returns>
        public async Task<Response<List<GetArbitratorsRespon>>> GetArbitrators()
        {
            var db = new NDatabase();

            var items = await db.FetchAsync<UserArbitrate>("select * from UserArbitrate where IsDelete = 0");

            var list = items.Select(a =>
            {
                return new GetArbitratorsRespon
                {
                    ArbitrateNum = a.ArbitrateNum,
                    CreateDate = a.CreateDate,
                    Name = CommonHelp.GetName(WalletHelp.GetName(a.DIDUserId)),
                    Number = a.Number
                };
            }).ToList();

            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 获取仲裁公示
        /// </summary>
        /// <returns></returns>
        public async Task<Response<List<GetArbitrateInfoRespon>>> GetArbitrateInfo()
        {
            var db = new NDatabase();

            var items = await db.FetchAsync<ArbitrateInfo>("select * from ArbitrateInfo where Status > 1 and IsCancel = 0 order by VoteDate Desc");//获取已判决仲裁列表

            var list = items.Select(a =>
            {
                return new GetArbitrateInfoRespon
                {
                    ArbitrateInfoId = a.ArbitrateInfoId,
                    Status = a.Status,
                    Defendant = WalletHelp.GetName(a.Defendant),
                    Plaintiff = WalletHelp.GetName(a.Plaintiff),
                    DefendantNum = db.SingleOrDefault<int>("select count(*) from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", a.ArbitrateInfoId, VoteStatusEnum.被告胜),
                    PlaintiffNum = db.SingleOrDefault<int>("select count(*) from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", a.ArbitrateInfoId, VoteStatusEnum.被告胜),
                    AdduceDate = a.AdduceDate,
                    VoteDate = a.VoteDate,
                    PlaintiffId = a.Plaintiff,
                    DefendantId = a.Defendant
                };
            }).ToList();

            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 获取仲裁详情
        /// </summary>
        /// param name="arbitrateInfoId"
        /// <returns></returns>
        public async Task<Response<GetArbitrateDetailsRespon>> GetArbitrateDetails(string arbitrateInfoId, string userId)
        {
            var db = new NDatabase();
            var item = await db.SingleOrDefaultByIdAsync<ArbitrateInfo>(arbitrateInfoId);

            var model = new GetArbitrateDetailsRespon
            {
                ArbitrateInfoId = item.ArbitrateInfoId,
                VoteDate = item.VoteDate,
                AdduceDate = item.AdduceDate,
                Status = item.Status,
                Defendant = WalletHelp.GetName(item.Defendant),
                Plaintiff = WalletHelp.GetName(item.Plaintiff),
                DefendantNum = await db.SingleOrDefaultAsync<int>("select count(*) from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", item.ArbitrateInfoId, VoteStatusEnum.被告胜),
                PlaintiffNum = await db.SingleOrDefaultAsync<int>("select count(*) from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", item.ArbitrateInfoId, VoteStatusEnum.被告胜),
                PlaintiffId = item.Plaintiff,
                DefendantId = item.Defendant
            };

            var users = await db.FetchAsync<ArbitrateVote>("select * from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus > 0 ", item.ArbitrateInfoId);//已投票记录

            var votes = new List<Vote>();

            users.ForEach(a =>
            {
                votes.Add(new Vote
                {
                    Name = WalletHelp.GetName(a.VoteUserId),
                    Number = db.SingleOrDefault<string>("select Number from UserArbitrate where DIDUserId = @0 and IsDelete = 0", a.VoteUserId),
                    VoteStatus = a.VoteStatus,
                    Reason = a.Reason
                });
            });
            model.Votes = votes;

            //举证记录
            var adduce = await db.FetchAsync<AdduceList>("select * from AdduceList where ArbitrateInfoId = @0", arbitrateInfoId);
            model.Adduce = adduce;

            //当前仲裁员判决
            var useradduce = await db.SingleOrDefaultAsync<ArbitrateVote>("select * from ArbitrateVote where  ArbitrateInfoId = @0 and VoteUserId = @1", arbitrateInfoId, userId);
            model.UserVote = new Vote
            {
                Name = WalletHelp.GetName(useradduce.VoteUserId),
                Number = db.SingleOrDefault<string>("select Number from UserArbitrate where DIDUserId = @0 and IsDelete = 0", useradduce.VoteUserId),
                VoteStatus = useradduce.VoteStatus,
                Reason = useradduce.Reason
            };

            model.EOTC = 100;

            return InvokeResult.Success(model);
        }

        /// <summary>
        /// 提交仲裁
        /// </summary>
        /// <param name="plaintiff">uId</param>
        /// <param name="defendant">uId</param>
        /// <param name="orderId"></param>
        /// <param name="num"></param>
        /// <param name="memo"></param>
        /// <param name="images"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<Response> AddArbitrateInfo(string plaintiff, string defendant, string orderId, int num, string memo, string images, ArbitrateInTypeEnum type)
        {
            using var db = new NDatabase();

            //通过UID获取用户编号
            var defendantUser = await db.SingleOrDefaultByIdAsync<DIDUser>(defendant);
            var plaintiffUser = await db.SingleOrDefaultByIdAsync<DIDUser>(plaintiff);
            if (null == plaintiffUser)
                return InvokeResult.Fail("原告信息未找到!");
            if (null == defendantUser)
                return InvokeResult.Fail("被告信息未找到!");
            
            var item = new ArbitrateInfo
            {
                ArbitrateInfoId = Guid.NewGuid().ToString(),
                CreateDate = DateTime.Now,
                AdduceDate = DateTime.Now.AddDays(3),
                Defendant = defendantUser.DIDUserId,
                OrderId = orderId,
                Plaintiff = plaintiffUser.DIDUserId,
                Number = num,
                Status = ArbitrateStatusEnum.举证中,
                VoteDate = DateTime.Now.AddDays(6),
                ArbitrateInType = type
            };

            //todo: 获取仲裁投票用户编号
            var userIds = new List<string>();
            userIds.Add("e8771b3c-3b05-4830-900d-df2be0a6e9f7");
            userIds.Add("d389e5db-37d0-40cd-9d8b-0d31a0ef2c12");
            userIds.Add("61d14a4f-c45f-4b13-a957-5bcaff9b3324");
            userIds.Add("7e88d292-7454-4e26-821a-b4e6049a7a95");
            userIds.Add("2a5bf1dd-e15b-40f4-94bb-b68cee2bbaf9");

            var votes = new List<ArbitrateVote>();
            userIds.ForEach(a =>
            {
                votes.Add(new ArbitrateVote
                {
                    ArbitrateVoteId = Guid.NewGuid().ToString(),
                    ArbitrateInfoId = item.ArbitrateInfoId,
                    CreateDate = DateTime.Now,
                    VoteStatus = VoteStatusEnum.未投票,
                    VoteUserId = a
                });
            });

            //举证
            var adduce = new AdduceList
            {
                AdduceListId = Guid.NewGuid().ToString(),
                ArbitrateInfoId = item.ArbitrateInfoId,
                AdduceUserId = plaintiff,
                CreateDate = DateTime.Now,
                Images = images,
                Memo = memo
            };

            db.BeginTransaction();
            await db.InsertAsync(item);
            await db.InsertBatchAsync(votes);
            await db.InsertAsync(adduce);
            db.CompleteTransaction();

            //默认3天举证时间
            ToDelay(item.ArbitrateInfoId, 4320000);

            return InvokeResult.Success("提交成功!");
        }

        /// <summary>
        /// 获取待处理 已仲裁（原告、被告）
        /// </summary>
        /// <returns></returns>
        public async Task<Response<List<GetArbitrateInfoRespon>>> GetUserArbitrate(string userId, int type)
        {
            using var db = new NDatabase();
            var items = new List<ArbitrateInfo>();
            if (type == 0)//待处理
                items = await db.FetchAsync<ArbitrateInfo>("select * from ArbitrateInfo where Status < 2 and (Plaintiff = @0 or Defendant = @0) and IsCancel = 0 order by VoteDate Desc", userId);
            else
                items = await db.FetchAsync<ArbitrateInfo>("select * from ArbitrateInfo where Status > 1 and (Plaintiff = @0 or Defendant = @0) order by VoteDate Desc", userId);

            var list = items.Select(a =>
            {
                return new GetArbitrateInfoRespon
                {
                    ArbitrateInfoId = a.ArbitrateInfoId,
                    Status = a.Status,
                    AdduceDate = a.AdduceDate,
                    VoteDate = a.VoteDate,
                    Defendant = WalletHelp.GetName(a.Defendant),
                    Plaintiff = WalletHelp.GetName(a.Plaintiff),
                    ArbitrateInType = a.ArbitrateInType,
                    DefendantNum = db.SingleOrDefault<int>("select count(*) from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", a.ArbitrateInfoId, VoteStatusEnum.被告胜),
                    PlaintiffNum = db.SingleOrDefault<int>("select count(*) from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", a.ArbitrateInfoId, VoteStatusEnum.被告胜),
                    IsCancel = a.IsCancel,
                    PlaintiffId = a.Plaintiff,
                    DefendantId = a.Defendant
                };
            }).ToList();

            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 获取待仲裁 已结案列表 0 待仲裁 1 已仲裁
        /// </summary>
        /// <returns></returns>
        public async Task<Response<List<GetArbitrateInfoRespon>>> GetArbitrateList(string userId, int type)
        {
            using var db = new NDatabase();
            var items = new List<ArbitrateVote>();
            if (type == 0)//待仲裁
                items = await db.FetchAsync<ArbitrateVote>("select * from ArbitrateVote where VoteStatus = 0 and VoteUserId = @0 and IsDelete = 0", userId);
            else
                items = await db.FetchAsync<ArbitrateVote>("select * from ArbitrateVote where (VoteStatus > 0 or IsDelete = 0) and VoteUserId = @0", userId);//已仲裁

            var list = new List<GetArbitrateInfoRespon>();
            items.ForEach(a =>
            {
                var model = db.SingleOrDefaultById<ArbitrateInfo>(a.ArbitrateInfoId);
                if (model.IsCancel == IsEnum.否)
                {
                    var delay = db.SingleOrDefault<ArbitrateDelay>("select * from ArbitrateDelay where ArbitrateInfoId = @0 and DelayUserId = @1", a.ArbitrateInfoId, userId);

                    list.Add(new GetArbitrateInfoRespon
                    {
                        ArbitrateInfoId = a.ArbitrateInfoId,
                        Status = model.Status,
                        AdduceDate = model.AdduceDate,
                        VoteDate = model.VoteDate,
                        ArbitrateInType = model.ArbitrateInType,
                        Defendant = WalletHelp.GetName(model.Defendant),
                        Plaintiff = WalletHelp.GetName(model.Plaintiff),
                        DefendantNum = db.SingleOrDefault<int>("select count(*) from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", a.ArbitrateInfoId, VoteStatusEnum.被告胜),
                        PlaintiffNum = db.SingleOrDefault<int>("select count(*) from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", a.ArbitrateInfoId, VoteStatusEnum.被告胜),
                        IsCancel = model.IsCancel,
                        VoteStatus = a.VoteStatus,
                        PlaintiffId = model.Plaintiff,
                        DefendantId = model.Defendant,
                        EOTC = 100,//EOTC数量
                        HasDelay = delay == null
                    });
                }
            });

            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 获取风险用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<RiskUserInfo>> GetUserInfo(string userId)
        {
            using var db = new NDatabase();
            var authinfo = await db.SingleOrDefaultAsync<UserAuthInfo>("select b.* from DIDUser a left join UserAuthInfo b on  a.UserAuthInfoId = b.UserAuthInfoId where a.DIDUserId = @0 and a.AuthType = 2", userId);
            if (authinfo == null) return InvokeResult.Fail<RiskUserInfo>("认证信息未找到!");//认证信息未找到!
            var model = new RiskUserInfo();
            model.Name = CommonHelp.GetName(authinfo.Name);

            //后四位变为*
            authinfo.PhoneNum = authinfo.PhoneNum.Remove(authinfo.PhoneNum.Length - 4, 4).Insert(authinfo.PhoneNum.Length - 4, "****");
            authinfo.IdCard = authinfo.IdCard.Remove(authinfo.IdCard.Length - 4, 4).Insert(authinfo.IdCard.Length - 4, "****");
            model.PhoneNum = authinfo.PhoneNum;
            model.IdCard = authinfo.IdCard;

            var auths = await db.FetchAsync<Auth>("select * from Auth where UserAuthInfoId = @0 order by AuditStep Desc", authinfo.UserAuthInfoId);
            model.PortraitImage = auths?[0].PortraitImage;
            model.NationalImage = auths?[0].NationalImage;
            model.HandHeldImage = auths?[0].HandHeldImage;

            return InvokeResult.Success(model);
        }


        /// <summary>
        /// 仲裁员投票
        /// </summary>
        /// <returns></returns>
        public async Task<Response> ArbitrateVote(string arbitrateInfoId, string userId, string reason, VoteStatusEnum status)
        {
            using var db = new NDatabase();
            var arbitrate = await db.SingleOrDefaultByIdAsync<ArbitrateInfo>(arbitrateInfoId);
            if (null == arbitrate)
                return InvokeResult.Fail("信息未找到!");
            if (DateTime.Now > arbitrate.VoteDate)
                return InvokeResult.Fail("投票已截止!");

            var vote = await db.SingleOrDefaultAsync<ArbitrateVote>("select * from ArbitrateVote where ArbitrateInfoId = @0 and VoteUserId = @1", arbitrateInfoId, userId);
            if (null == vote)
                return InvokeResult.Fail("信息未找到!");

            vote.VoteStatus = status;
            vote.VoteDate = DateTime.Now;
            vote.Reason = reason;

            await db.UpdateAsync(vote);

            //todo 
            //var list = await db.FetchAsync<ArbitrateVote>("select * from ArbitrateVote where ArbitrateInfoId = @0", arbitrateInfoId);
            //var num = 0;//原告票数
            //list.Select(a => a.VoteStatus == VoteStatusEnum.原告胜 ? num++ : num);

            //if (num > list.Count / 2)
            //{
            //    var delay = await db.SingleOrDefaultByIdAsync<ArbitrateDelay>(delayVote.ArbitrateDelayId);
            //    var model = await db.SingleOrDefaultByIdAsync<ArbitrateInfo>(delay.ArbitrateInfoId);
            //    model.AdduceDate.AddDays(delay.Days);
            //    await db.UpdateAsync(model);
            //}

            return InvokeResult.Success("判决成功!");
        }


        //3天默认时间
        private System.Timers.Timer t = new();//实例化Timer类，设置间隔时间为10000毫秒；
        //3天仲裁时间
        private readonly System.Timers.Timer t1 = new(3 * 24 * 3600 * 1000);

        /// <summary>
        /// 举证定时器 
        /// </summary>
        /// <param name="arbitrateInfoId"></param>
        /// <param name="time"></param>
        public void ToDelay(string arbitrateInfoId, int time)
        {
            t.Stop();
            t = new(time);
            t.Elapsed += new System.Timers.ElapsedEventHandler((object? source, System.Timers.ElapsedEventArgs e) =>
            {

                t.Stop(); //先关闭定时器
                t1.Stop(); //先关闭定时器        

                using var db = new NDatabase();
                var item = db.SingleOrDefaultById<ArbitrateInfo>(arbitrateInfoId);

                item.Status = ArbitrateStatusEnum.投票中;
                db.Update(item);

                t1.Elapsed += new System.Timers.ElapsedEventHandler((object? source, System.Timers.ElapsedEventArgs e) =>
                {
                    t1.Stop(); //先关闭定时器
                    using var db = new NDatabase();
                    var item = db.SingleOrDefaultById<ArbitrateInfo>(arbitrateInfoId);

                    //限时未出结果 未投票用户扣分 加入新人
                    if (item.Status == ArbitrateStatusEnum.投票中)
                    {
                        var votes = db.Fetch<ArbitrateVote>("select * from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", arbitrateInfoId, VoteStatusEnum.未投票);

                        votes.ForEach(a =>
                        {
                            var userId = a.VoteUserId;
                            //todo: 扣分
                            a.IsDelete = IsEnum.是;
                            db.UpdateAsync(a);

                            //新的投票对象
                            var newvote = new ArbitrateVote
                            {
                                ArbitrateVoteId = Guid.NewGuid().ToString(),
                                ArbitrateInfoId = item.ArbitrateInfoId,
                                CreateDate = DateTime.Now,
                                VoteStatus = VoteStatusEnum.未投票,
                                VoteUserId = a.VoteUserId
                            };

                            db.Insert(newvote);

                            var item1 = db.SingleOrDefaultById<ArbitrateInfo>(arbitrateInfoId);

                            item1.VoteDate = item1.VoteDate.AddDays(3);//投票时间延长3天
                            db.Update(item1);
                        });

                    }

                    db.Update(item);
                });
                t1.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
                t1.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                t1.Start(); //启动定时器
            });//到达时间的时候执行事件；
            t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            t.Start(); //启动定时器

        }

        /// <summary>
        /// 申请延期
        /// </summary>
        /// <returns></returns>
        public async Task<Response> ArbitrateDelay(string arbitrateInfoId, string userId, ReasonEnum reason, string explain, int day, IsEnum isArbitrate)
        {
            using var db = new NDatabase();

            var item = new ArbitrateDelay
            {
                ArbitrateDelayId = Guid.NewGuid().ToString(),
                ArbitrateInfoId = arbitrateInfoId,
                CreateDate = DateTime.Now,
                Days = day,
                DelayUserId = userId,
                Explain = explain,
                Reason = reason,
                IsArbitrate = isArbitrate
            };

            //todo: 获取仲裁投票用户编号
            var userIds = await db.FetchAsync<string>("select VoteUserId from ArbitrateVote where ArbitrateInfoId = @0", arbitrateInfoId);
            var votes = new List<DelayVote>();
            userIds.ForEach(a =>
            {
                votes.Add(new DelayVote
                {
                    DelayVoteId = Guid.NewGuid().ToString(),
                    ArbitrateDelayId = item.ArbitrateDelayId,
                    CreateDate = DateTime.Now,
                    Status = DelayVoteStatus.未投票,
                    VoteUserId = a
                });
            });

            db.BeginTransaction();
            await db.InsertAsync(item);
            await db.InsertBatchAsync(votes);
            db.CompleteTransaction();

            return InvokeResult.Success("申请成功!");
        }


        /// <summary>
        /// 申请延期投票
        /// </summary>
        /// <returns></returns>
        public async Task<Response> ArbitrateDelayVote(string delayVoteId, DelayVoteStatus status)
        {
            using var db = new NDatabase();

            var delayVote = await db.SingleOrDefaultByIdAsync<DelayVote>(delayVoteId);
            delayVote.Status = DelayVoteStatus.同意;
            await db.UpdateAsync(delayVote);

            var list = await db.FetchAsync<DelayVote>("select * from DelayVote where ArbitrateDelayId = @0", delayVote.ArbitrateDelayId);
            var num = 0;//同意票数
            list.Select(a => a.Status == DelayVoteStatus.同意 ? num++ : num);

            if (num > list.Count / 2)
            {
                var delay = await db.SingleOrDefaultByIdAsync<ArbitrateDelay>(delayVote.ArbitrateDelayId);
                var model = await db.SingleOrDefaultByIdAsync<ArbitrateInfo>(delay.ArbitrateInfoId);
                model.AdduceDate.AddDays(delay.Days);
                model.Status = ArbitrateStatusEnum.举证中;
                await db.UpdateAsync(model);

                //延期
                var time = (model.AdduceDate - DateTime.Now).Milliseconds;
                ToDelay(delay.ArbitrateInfoId, time);
            }
            return InvokeResult.Success("投票成功!");
        }

        /// <summary>
        /// 追加举证
        /// </summary>
        /// <returns></returns>
        public async Task<Response> AddAdduceList(string arbitrateInfoId, string userId, string memo, string images)
        {
            using var db = new NDatabase();

            var arbitrate = await db.SingleOrDefaultByIdAsync<ArbitrateInfo>(arbitrateInfoId);

            if (DateTime.Now > arbitrate.AdduceDate)
                return InvokeResult.Fail("举证已截止!");

            //举证
            var adduce = new AdduceList
            {
                AdduceListId = Guid.NewGuid().ToString(),
                ArbitrateInfoId = arbitrateInfoId,
                AdduceUserId = userId,
                CreateDate = DateTime.Now,
                Images = images,
                Memo = memo
            };

            await db.InsertAsync(adduce);
            return InvokeResult.Success("添加成功!");
        }

        /// <summary>
        /// 取消仲裁
        /// </summary>
        /// <returns></returns>
        public async Task<Response> CancelArbitrate(string userId, string arbitrateInfoId)
        {
            using var db = new NDatabase();

            var arbitrate = await db.SingleOrDefaultByIdAsync<ArbitrateInfo>(arbitrateInfoId);

            if (null == arbitrate)
                return InvokeResult.Fail("信息未找到!");
            if (userId != arbitrate.Plaintiff)
                return InvokeResult.Fail("没有权限!");

            arbitrate.IsCancel = IsEnum.是;
            await db.UpdateAsync(arbitrate);
            return InvokeResult.Success("取消成功!");
        }


        /// <summary>
        /// 获取仲裁消息列表
        /// </summary>
        /// <returns></returns>
        public async Task<Response<List<GetArbitrateMessageRespon>>> GetArbitrateMessage(string userId)
        {
            using var db = new NDatabase();

            var list = await db.FetchAsync<ArbitrateMessage>("select * from ArbitrateMessage where UserId = @0 order by CreateDate", userId);

            var items = new List<GetArbitrateMessageRespon>();
            list.ForEach(a =>
            {
                var reason = "";//原因
                var isArbitrate = IsEnum.否;//延期是否为仲裁员发起
                if (a.MessageType == MessageTypeEnum.申请延期)
                {
                    var model = db.SingleOrDefaultById<ArbitrateDelay>(a.AssociatedId);
                    if (model != null)
                    {
                        reason = Enum.GetName(model.Reason);
                        isArbitrate = model.IsArbitrate;
                    }
                }
                else if (a.MessageType == MessageTypeEnum.追加举证)
                {
                    var model = db.SingleOrDefaultById<AdduceList>(a.AssociatedId);
                    var info = db.SingleOrDefaultById<ArbitrateInfo>(model.ArbitrateInfoId);
                    if (model != null && info != null)
                    {
                        if (info.Defendant == model.AdduceUserId)
                            reason = "原告追加举证";
                        else if (info.Defendant == model.AdduceUserId)
                            reason = "被告追加举证";
                    }
                }
                else if (a.MessageType == MessageTypeEnum.仲裁取消)
                {
                    var model = db.SingleOrDefaultById<ArbitrateInfo>(a.AssociatedId);
                    if (model != null)
                    {
                        reason = Enum.GetName(model.CancelReason);
                    }
                }
                
                items.Add(new GetArbitrateMessageRespon
                {
                    ArbitrateMessageId = a.ArbitrateMessageId,
                    AssociatedId = a.AssociatedId,
                    CreateDate = a.CreateDate,
                    IsOpen = a.IsOpen,
                    MessageType = a.MessageType,
                    Reason = reason
                });
            });

            return InvokeResult.Success(items);
        }

        /// <summary>
        /// 获取原被告延期消息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<GetArbitrateDelayRespon>> GetArbitrateDelay(string id, IsEnum isArbitrate)
        {
            using var db = new NDatabase();

            var model = await db.SingleOrDefaultByIdAsync<ArbitrateDelay>(id);
            var info = await db.SingleOrDefaultByIdAsync<ArbitrateInfo>(model.ArbitrateInfoId);
            var respon = new GetArbitrateDelayRespon
            {
                ArbitrateInfoId = model.ArbitrateInfoId,
                ArbitrateInType = info.ArbitrateInType,
                Days = model.Days,
                Defendant = WalletHelp.GetName(info.Defendant),
                DefendantId = info.Defendant,
                Plaintiff = WalletHelp.GetName(info.Plaintiff),
                PlaintiffId = info.Defendant,
                DelayUserId = model.DelayUserId,
                Explain = model.Explain,
                Reason = Enum.GetName(model.Reason)
            };

            if (isArbitrate == IsEnum.是)
            {
                respon.Name = WalletHelp.GetName(model.DelayUserId);
                respon.Number = db.SingleOrDefault<string>("select Number from UserArbitrate where DIDUserId = @0 and IsDelete = 0", model.DelayUserId);
            }

            return InvokeResult.Success(respon);
        }

        /// <summary>
        /// 获取取消仲裁消息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<GetCancelArbitrateRespon>> GetCancelArbitrate(string id)
        {
            using var db = new NDatabase();
            var info = await db.SingleOrDefaultByIdAsync<ArbitrateInfo>(id);
            var respon = new GetCancelArbitrateRespon
            {
                ArbitrateInfoId = id,
                ArbitrateInType = info.ArbitrateInType,
                Defendant = WalletHelp.GetName(info.Defendant),
                DefendantId = info.Defendant,
                Plaintiff = WalletHelp.GetName(info.Plaintiff),
                PlaintiffId = info.Defendant,
                Reason = Enum.GetName(info.CancelReason)
            };

            return InvokeResult.Success(respon);
        }

        /// <summary>
        /// 获取追加举证消息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<GetAdduceListRespon>> GetAdduceList(string id)
        {
            using var db = new NDatabase();
            var model = await db.SingleOrDefaultByIdAsync<AdduceList>(id);
            var info = await db.SingleOrDefaultByIdAsync<ArbitrateInfo>(model.ArbitrateInfoId);
            var respon = new GetAdduceListRespon
            {
                ArbitrateInfoId = model.ArbitrateInfoId,
                ArbitrateInType = info.ArbitrateInType,
                Defendant = WalletHelp.GetName(info.Defendant),
                DefendantId = info.Defendant,
                Plaintiff = WalletHelp.GetName(info.Plaintiff),
                PlaintiffId = info.Defendant,
                AdduceUserId = model.AdduceUserId,
                Images = model.Images,
                Memo = model.Memo
            };

            return InvokeResult.Success(respon);
        }

        /// <summary>
        /// 获取结案通知
        /// </summary>
        /// <returns></returns>
        public async Task<Response<GetClosureRespon>> GetClosure(string id)
        {
            using var db = new NDatabase();
            var info = await db.SingleOrDefaultByIdAsync<ArbitrateInfo>(id);
            var respon = new GetClosureRespon
            {
                ArbitrateInfoId = info.ArbitrateInfoId,
                Status = info.Status,
                Defendant = WalletHelp.GetName(info.Defendant),
                Plaintiff = WalletHelp.GetName(info.Plaintiff),
                DefendantNum = await db.SingleOrDefaultAsync<int>("select count(*) from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", id, VoteStatusEnum.被告胜),
                PlaintiffNum = await db.SingleOrDefaultAsync<int>("select count(*) from ArbitrateVote where ArbitrateInfoId = @0 and VoteStatus = @1", id, VoteStatusEnum.被告胜),
                PlaintiffId = info.Plaintiff,
                DefendantId = info.Defendant,
                EOTC = 100
            };

            return InvokeResult.Success(respon);
        }

        /// <summary>
        /// 设置消息为已读
        /// </summary>
        /// <returns></returns>
        public async Task<Response> SetMessageIsOpen(string messageId)
        {
            using var db = new NDatabase();
            var info = await db.SingleOrDefaultByIdAsync<ArbitrateMessage>(messageId);

            info.IsOpen = IsEnum.是;
            await db.UpdateAsync(info);
            return InvokeResult.Success("设置成功!");
        }

        /// <summary>
        /// 获取仲裁员是否有未读消息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<bool>> GetMessageIsOpen(string userId)
        {
            using var db = new NDatabase();
            var info = await db.FetchAsync<ArbitrateMessage>("select * from ArbitrateMessage where UserId = @0 and IsOpen = 0", userId);

            if(info.Count > 0)
                return InvokeResult.Success(true);
            return InvokeResult.Success(false);
        }

        /// <summary>
        /// 获取被告待处理消息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<int>> GetWaitMessage(string userId)
        {
            using var db = new NDatabase();
            var info = await db.SingleOrDefaultAsync<int>("select * from ArbitrateInfo where Status = 2 and Defendant = @0 and IsCancel = 0", userId);

            return InvokeResult.Success(info);
        }
    }
}