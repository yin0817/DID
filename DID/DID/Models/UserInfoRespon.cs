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

        /// <summary>
        /// 邀请人编号
        /// </summary>
        public int? RefUid
        {
            get; set;
        }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string? RefDIDUserId
        {
            get; set;
        }

        /// <summary>
        /// 电报群
        /// </summary>
        public string? Telegram
        {
            get; set;
        }

        /// <summary>
        /// 国家
        /// </summary>
        public string? Country
        {
            get; set;
        }

        /// <summary>
        /// 地区
        /// </summary>
        public string? Area
        {
            get; set;
        }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string? Name
        {
            get; set;
        }
        /// <summary>
        /// 手机号
        /// </summary>
        public string? PhoneNum
        {
            get; set;
        }
        /// <summary>
        /// 证件号
        /// </summary>
        public string? IdCard
        {
            get; set;
        }
    }
}
