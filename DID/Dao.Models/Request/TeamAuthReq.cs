using Dao.Models.Base;
using DID.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Models.Request
{
    public class TeamAuthReq : DaoBaseReq
    {
        /// <summary>
        /// 团队审核编号
        /// </summary>
        public string TeamAuthId
        {
            get; set;
        }
        /// <summary>
        ///  审核类型  0 未审核 1 审核通过  2 未实名认证 3 其他
        /// </summary>
        public TeamAuditEnum AuditType
        {
            get; set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark
        {
            get; set;
        }
    }
}
