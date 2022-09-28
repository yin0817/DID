﻿using DID.Entitys;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Entity
{
    /// <summary>
    /// 状态 0 举证中 1 投票中 2 原告胜 3 被告胜
    /// </summary>
    public enum ArbitrateStatusEnum { 举证中, 投票中, 原告胜, 被告胜 }

    /// <summary>
    /// 仲裁信息
    /// </summary>
    [PrimaryKey("ArbitrateInfoId", AutoIncrement = false)]
    public class ArbitrateInfo
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
        /// 创建日期 默认为当前时间
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }
        
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId
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

        /// <summary>
        /// 状态 0 举证中 1 投票中 2 原告胜 3 被告胜
        /// </summary>
        public ArbitrateStatusEnum Status
        {
            get; set;
        }
        
        /// <summary>
        /// 仲裁人数
        /// </summary>
        public int Number
        {
            get; set;
        }

        /// <summary>
        /// 是否取消
        /// </summary>
        public IsEnum IsCancel
        {
            get; set;
        }

    }
}
