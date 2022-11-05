using DID.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DID.Models.Response
{
    public class GetCommunityInfoRespon : Community
    { 
        /// <summary>
        /// 上级社区
        /// </summary>
        public string RefComName
        {
            get; set;
        }
    }
}
