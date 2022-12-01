using DID.Entity;
using DID.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Models.Response
{
    public class GetAuthAppealDetailsRespon : AuthAppeal
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
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
        /// 认证信息
        /// </summary>
        public List<AuthInfo>? Auths
        {
            get; set;
        }

        /// <summary>
        /// 审核人姓名
        /// </summary>
        public string AuditName
        {
            get; set;
        }
        /// <summary>
        /// 审核人Uid
        /// </summary>
        public int AuditUid
        {
            get; set;
        }
    }
}
