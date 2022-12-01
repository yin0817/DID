using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DID.Entity
{
    [PrimaryKey("AuthAppealId", AutoIncrement = false)]
    public class AuthAppeal
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string AuthAppealId
        {
            get; set;
        }
        /// <summary>
        /// 认证编号
        /// </summary>
        public string UserAuthInfoId
        {
            get; set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Describe
        {
            get; set;
        }
        /// <summary>
        /// 图片
        /// </summary>
        public string? Images
        {
            get; set;
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string? Phone
        {
            get; set;
        }
        /// <summary>
        /// 创建日期 默认为当前时间
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }
        /// <summary>
        /// 审核用户编号
        /// </summary>
        public string? AuditUserId
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
        //审核类型  0 未审核 1 审核通过  2 打回
        public AuthAppealEnum AuditType
        {
            get; set;
        }
        //原因
        public string? Reason
        {
            get; set;
        }
    }
    /// <summary>
    /// 0 未审核, 1 审核通过, 2 打回
    /// </summary>
    public enum AuthAppealEnum { 未审核, 审核通过, 打回 }
}
