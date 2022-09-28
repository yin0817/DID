using NPoco;

namespace DID.Entitys
{
    /// <summary>
    /// 身份认证状态 0 未审核 1 审核中 2 审核成功 3 审核失败
    /// </summary>
    public enum AuthTypeEnum { 未审核, 审核中, 审核成功, 审核失败 }
    /// <summary>
    /// 风险等级 0 低风险 1 中风险 2 高风险
    /// </summary>s
    public enum RiskLevelEnum { 低风险, 中风险, 高风险 }
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
        /// 身份认证状态 0 未审核 1 审核中 2 审核成功 3 审核失败
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
        public string? RefUserId
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
        /// 省
        /// </summary>
        public string Province
        {
            get; set;
        }
        /// <summary>
        /// 市
        /// </summary>
        public string City
        {
            get; set;
        }
        /// <summary>
        /// 区
        /// </summary>
        public string Area
        {
            get; set;
        }

        /// <summary>
        /// 是否注销
        /// </summary>
        public IsEnum IsLogout
        {
            get; set;
        }

        /// <summary>
        /// 注销编号
        /// </summary>
        public string? UserLogoutId
        {
            get; set;
        }

        /// <summary>
        /// 用户申请社区编号
        /// </summary>
        public string? ApplyCommunityId
        {
            get; set;
        }

        /// <summary>
        /// Dao总收益
        /// </summary>
        public double DaoEOTC
        {
            get; set;
        }

        /// <summary>
        /// 是否为仲裁员 0 否 1 是 
        /// </summary>
        public IsEnum IsArbitrate
        {
            get; set;
        }

        /// <summary>
        /// 是否为审核员 0 否 1 是
        /// </summary>
        public IsEnum IsExamine
        {
            get; set;
        }

        /// <summary>
        /// 风险等级 0 低风险 1 中风险 2 高风险
        /// </summary>
        public RiskLevelEnum RiskLevel
        {
            get; set;
        }
    }
}
