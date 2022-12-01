using Dao.Entity;
using Dao.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Models.Request
{
    public class AuditAuditorReq : DaoBaseByIdReq
    {
        /// <summary>
        /// 
        /// </summary>

        //public int Type
        //{
        //    get; set;
        //}
        /// <summary>
        /// 审核中, 审核成功, 审核失败
        /// </summary>
        public AuditStatusEnum State
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
    }
}
