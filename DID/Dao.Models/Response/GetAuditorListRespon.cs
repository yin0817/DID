using Dao.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Models.Response
{
    public class GetAuditorListRespon
    {


        /// <summary>
        /// 编号
        /// </summary>
        public string UserExamineId
        {
            get; set;
        }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string DIDUserId
        {
            get; set;
        }
        /// <summary>
        /// Uid
        /// </summary>
        public int Uid
        {
            get; set;
        }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get; set;
        }
        /// <summary>
        /// 考试分数
        /// </summary>
        public double TestScore
        {
            get; set;
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate
        {
            get; set;
        }
        /// <summary>
        /// 质押数量
        /// </summary>
        public double StakeEotc
        {
            get; set;
        }

        /// <summary>
        /// 审核状态 0 审核中 1 审核成功 2 审核失败
        /// </summary>
        public AuditStatusEnum Status
        {
            get; set;
        }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string? Reason
        {
            get; set;
        }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDate
        {
            get; set;
        }
        /// <summary>
        /// 信用分
        /// </summary>
        public int CreditScore
        {
            get; set;
        }
    }
}
