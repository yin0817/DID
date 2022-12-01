using DID.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Models.Response
{
    public class GetAuthAppealRespon : AuthAppeal
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
    }
}
