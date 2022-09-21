using Dao.Common;
using Dao.Entity;
using Dao.Models.Base;
using Dao.Models.Request;
using Dao.Models.Response;
using DID.Common;
using DID.Models.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dao.Services
{
    /// <summary>
    /// 工单接口
    /// </summary>
    public interface IWorkOrderService
    {
        /// <summary>
        /// 添加工单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<Response> AddWorkOrder(AddWorkOrderReq req);

        /// <summary>
        /// 社区图片上传 1 请上传文件! 2 文件类型错误!
        /// </summary>
        /// <returns></returns>
        Task<Response> UploadImage(IFormFile file);

        /// <summary>
        /// 获取工单列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <returns></returns>
        Task<Response<List<GetWorkOrderListRespon>>> GetWorkOrderList(WorkOrderStatusEnum type, long page, long itemsPerPage);

    }

    /// <summary>
    /// 工单服务
    /// </summary>
    public class WorkOrderService : IWorkOrderService
    {
        private readonly ILogger<WorkOrderService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public WorkOrderService(ILogger<WorkOrderService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 添加工单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<Response> AddWorkOrder(AddWorkOrderReq req)
        {
            var walletId = WalletHelp.GetWalletId(req);

            using var db = new NDatabase();
            var model = new WorkOrder()
            {
                WorkOrderId = Guid.NewGuid().ToString(),
                CreateDate = DateTime.Now,
                WorkOrderType = req.WorkOrderType,
                Describe = req.Describe,
                Images = req.Images,
                Phone = req.Phone,
                Status = WorkOrderStatusEnum.待处理,
                WalletId = walletId
            };
            await db.InsertAsync(model);
            return InvokeResult.Success("提交成功!");
        }

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response> UploadImage(IFormFile file)
        {
            try
            {
                var dir = new DirectoryInfo(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Images/WorkOrderImges/"));

                //保存目录不存在就创建这个目录
                if (!dir.Exists)
                {
                    Directory.CreateDirectory(dir.FullName);
                }
                //var filename = upload.UserId + "_" + upload.Type + ".jpg";
                var filename = Guid.NewGuid().ToString() + ".jpg";
                using (var stream = new FileStream(dir.FullName + filename, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    //file.CopyTo(stream);
                }
                //return InvokeResult.Success("Images/AuthImges/" + upload.UId + "/" + filename);
                return InvokeResult.Success("Images/WorkOrderImges/" + filename);
            }
            catch (Exception e)
            {
                _logger.LogError("UploadImage", e);
                return InvokeResult.Fail("Fail");
            }
        }

        /// <summary>
        /// 获取工单列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <returns></returns>
        public async Task<Response<List<GetWorkOrderListRespon>>> GetWorkOrderList(WorkOrderStatusEnum type, long page, long itemsPerPage)
        {
            using var db = new NDatabase();
            var models = (await db.PageAsync<WorkOrder>(page, itemsPerPage, "select * from WorkOrder where WorkOrderStatus = @0 order by CreateDate desc", type)).Items;

            //获取提交人
            string getSubmitter(string walletId)
            {
                var uid = db.SingleOrDefault<string>("select b.Uid from Wallet a left join DIDUser b on a.DIDUserId = b.DIDUserId " +
                "where a.WalletId = @0 and a.IsLogout = 0 and a.IsDelete = 0", walletId);
                var name = db.SingleOrDefault<string>("select a.Name from UserAuthInfo a left join Wallet b on  a.DIDUserId = b.DIDUserId " +
                    "where a.WalletId = @0 and a.IsLogout = 0 and a.IsDelete = 0", walletId);
                return name + "(" + uid + ")";
            }

            var list = models.Select(x => new GetWorkOrderListRespon()
            {
                CreateDate = x.CreateDate,
                Describe = x.Describe,
                Status = x.Status,
                Submitter = getSubmitter(x.WalletId)
            }).ToList();

            return InvokeResult.Success(list);

        }
    }

}