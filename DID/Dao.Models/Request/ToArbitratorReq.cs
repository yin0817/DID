using Dao.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Models.Request
{
    public class ToArbitratorReq : DaoBaseReq
    {
        /// <summary>
        /// 分数
        /// </summary>
        public double Score
        {
            get; set;
        }
    }
}
