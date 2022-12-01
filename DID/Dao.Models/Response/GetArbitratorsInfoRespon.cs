using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dao.Entity;

namespace Dao.Models.Response
{
    public class GetArbitratorsInfoRespon : UserArbitrate
    {
        /// <summary>
        /// 审核人Uid
        /// </summary>
        public int? Uid
        {
            get; set;
        }
        /// <summary>
        /// 审核人
        /// </summary>
        public string? Name
        {
            get; set;
        }
    }

}
