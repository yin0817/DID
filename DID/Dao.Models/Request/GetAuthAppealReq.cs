using Dao.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DID.Models.Request
{
    public class GetAuthAppealReq : DaoBasePageReq
    {
        /// <summary>
        /// 0 未审核 1 已审核
        /// </summary>
        public int Type
        {
            get; set;
        }
    }
}
