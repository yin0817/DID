using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DID.Models.Request
{
    public class AuthAppealReq
    {
        /// <summary>
        /// 认证编号
        /// </summary>
        public string UserAuthInfoId
        {
            get; set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Describe
        {
            get; set;
        }
        /// <summary>
        /// 图片
        /// </summary>
        public string? Images
        {
            get; set;
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string? Phone
        {
            get; set;
        }
    }
}
