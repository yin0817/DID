using Dao.Models.Base;
using DID.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Models.Request
{
    public class AuthReq : DaoBaseReq
    {
        /// <summary>
        /// 申述编号
        /// </summary>
        public string Id
        {
            get; set;
        }
        /// <summary>
        /// 审核类型
        /// </summary>
        public AuthAppealEnum Type
        {
            get; set;
        }
        /// <summary>
        /// 打回原因
        /// </summary>
        public string? Reason
        {
            get; set;
        }
    }
}
