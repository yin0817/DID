using NPoco;

namespace DID.Entitys
{
    /// <summary>
    /// 身份认证状态 0 未认证 1 已认证
    /// </summary>
    public enum AuthTypeEnum { 未认证, 已认证 }
    /// <summary>
    /// 用户信息表
    /// </summary>
    [PrimaryKey("Uid")]
    public class DIDUser
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int Uid
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
        public int? RefUid
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


    }
}
