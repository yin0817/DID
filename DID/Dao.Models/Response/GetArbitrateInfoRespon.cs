using Dao.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Models.Response
{
    public class GetArbitrateInfoRespon
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string ArbitrateInfoId
        {
            get; set;
        }

        /// <summary>
        /// 原告
        /// </summary>
        public string Plaintiff
        {
            get; set;
        }

        /// <summary>
        /// 被告
        /// </summary>
        public string Defendant
        {
            get; set;
        }

        /// <summary>
        /// 原告票数
        /// </summary>
        public int PlaintiffNum
        {
            get; set;
        }

        /// <summary>
        /// 被告票数
        /// </summary>
        public int DefendantNum
        {
            get; set;
        }

        /// <summary>
        /// 状态 0 举证中 1 投票中 2 取消 3 原告胜 4 被告胜
        /// </summary>
        public ArbitrateStatusEnum Status
        {
            get; set;
        }

        /// <summary>
        /// 举证截至日期
        /// </summary>
        public DateTime AdduceDate
        {
            get; set;
        }

        /// <summary>
        /// 投票截止日期
        /// </summary>
        public DateTime VoteDate
        {
            get; set;
        }
    }
}
