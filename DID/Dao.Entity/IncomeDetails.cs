using NPoco;

namespace Dao.Entity
{
    public enum TypeEnum { 处理工单, 处理仲裁, 处理审核 }

    [PrimaryKey("IncomeDetailsId", AutoIncrement = false)]
    public class IncomeDetails
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string IncomeDetailsId
        {
            get; set;
        }
        /// <summary>
        /// 钱包编号
        /// </summary>
        public string WalletId
        {
            get; set;
        }
        /// <summary>
        /// 收益数量
        /// </summary>
        public double EOTC
        {
            get; set;
        }
        /// <summary>
        /// 0=处理工单 1=处理仲裁 2=处理审核 
        /// </summary>
        public TypeEnum Type
        {
            get; set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks
        {
            get; set;
        }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }

    }
}
