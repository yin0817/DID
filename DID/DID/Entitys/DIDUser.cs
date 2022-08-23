using NPoco;

namespace DID.Entitys
{
    /// <summary>
    /// 身份认证状态 0 未审核 1 审核中 2 审核成功 3 审核失败
    /// </summary>
    public enum AuthTypeEnum { 未审核, 审核中, 审核成功, 审核失败 }
    /// <summary>
    /// 用户信息表
    /// </summary>
    //[PrimaryKey("DIDUserId", AutoIncrement = false)]
    [PrimaryKey("Uid")]
    public class DIDUser
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string DIDUserId
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
        /// <summary>
        /// 密码（英文、数字 至少6 - 18位）
        /// </summary>
        public string PassWord
        {
            get; set;
        }
        /// <summary>
        /// 钱包地址
        /// </summary>
        //public string Wallet
        //{
        //    get; set;
        //}
        /// <summary>
        /// 网络类型
        /// </summary>
        //public string Otype
        //{
        //    get; set;
        //}
        /// <summary>
        /// 签名
        /// </summary>
        //public string Sign
        //{
        //    get; set;
        //}

        /// <summary>
        /// 身份认证状态 0 未认证 1 已认证
        /// </summary>
        public AuthTypeEnum AuthType
        {
            get; set;
        }
        /// <summary>
        /// 信用分
        /// </summary>
        public int CreditScore
        {
            get; set;
        }

        /// <summary>
        /// 用户认证当前编号
        /// </summary>
        public string UserAuthInfoId
        {
            get; set;
        }
        /// <summary>
        /// 用户节点
        /// </summary>
        public int UserNode
        {
            get; set;
        }
        /// <summary>
        /// 推荐人
        /// </summary>
        public string? RefUid
        {
            get; set;
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Mail
        {
            get; set;
        }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegDate
        {
            get; set;
        }
        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime? LoginDate
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
    }
}
