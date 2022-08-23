using DID.Entitys;

namespace DID.Models
{
    /// <summary>
    /// 审核详情
    /// </summary>
    public class AuthSuccessRespon
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name
        {
            get; set;
        }
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNum
        {
            get; set;
        }
        /// <summary>
        /// 证件号
        /// </summary>
        public string IdCard
        {
            get; set;
        }
        public string Mail { get; set; }
        /// <summary>
        /// 邀请人姓名
        /// </summary>
        public string RefUid
        {
            get; set;
        }
        /// <summary>
        /// 审批记录
        /// </summary>
        public List<AuthInfo>? Auths
        {
            get; set;
        }
    }
}
