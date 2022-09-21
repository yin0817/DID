using Dao.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Models.Response
{
    public class GetWorkOrderListRespon
    {
        /// <summary>
        /// 创建日期 默认为当前时间
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }
        /// <summary>
        /// 状态 选项：0=待处理 1=处理中 2=已处理
        /// </summary>
        public WorkOrderStatusEnum Status
        {
            get; set;
        }
        /// <summary>
        /// 提交人
        /// </summary>
        public string Submitter
        {
            get; set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe
        {
            get; set;
        }
    }
}
