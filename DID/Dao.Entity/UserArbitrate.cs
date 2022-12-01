﻿using DID.Entitys;
using NPoco;

namespace Dao.Entity
{
    /// <summary>
    /// 仲裁员信息表
    /// </summary>
    [PrimaryKey("UserArbitrateId", AutoIncrement = false)]
    public class UserArbitrate
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string UserArbitrateId
        {
            get; set;
        }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string DIDUserId
        {
            get; set;
        }

        /// <summary>
        /// 仲裁员编号
        /// </summary>
        public string Number
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
        /// 是否删除 0 否 1 是
        /// </summary>
        public IsEnum IsDelete
        {
            get; set;
        }
        /// <summary>
        /// 仲裁收益
        /// </summary>
        public double EOTC
        {
            get; set;
        }
        /// <summary>
        /// 仲裁次数
        /// </summary>
        public int ArbitrateNum
        {
            get; set;
        }
        /// <summary>
        /// 仲裁胜利次数
        /// </summary>
        public int VictoryNum
        {
            get; set;
        }

        /// <summary>
        /// 考试分数
        /// </summary>
        public double TestScore
        {
            get; set;
        }
        /// <summary>
        /// 审核状态 0 审核中 1 审核成功 2 审核失败
        /// </summary>
        public AuditStatusEnum Status
        {
            get; set;
        }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string? Reason
        {
            get; set;
        }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDate
        {
            get; set;
        }
        /// <summary>
        /// 审核用户编号
        /// </summary>
        public string? AuditUserId
        {
            get; set;
        }
    }
}
