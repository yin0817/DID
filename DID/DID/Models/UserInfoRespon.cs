using DID.Entitys;

namespace DID.Models
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfoRespon
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int Uid
        {
            get; set;
        }

        /// <summary>
        /// 身份认证状态 0 未认证 1 已认证
        /// </summary>
        public AuthTypeEnum? AuthType
        {
            get; set;
        }

        /// <summary>
        /// 信用分
        /// </summary>
        public int? CreditScore
        {
            get; set;
        }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Mail
        {
            get; set;
        }
    }
}
