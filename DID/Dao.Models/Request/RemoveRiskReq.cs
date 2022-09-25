using Dao.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Models.Request
{
    public class RemoveRiskReq : DaoBaseReq
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string UserRiskId
        {
            get; set;
        }
    }
}
