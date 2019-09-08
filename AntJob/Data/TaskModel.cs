﻿using System;

namespace AntJob.Data
{
    /// <summary>任务模型</summary>
    public partial class TaskModel
    {
        #region 属性
        /// <summary>编号</summary>
        public Int64 ID { get; set; }

        /// <summary>开始。大于等于</summary>
        public DateTime Start { get; set; }

        /// <summary>结束。小于，不等于</summary>
        public DateTime End { get; set; }

        /// <summary>偏移。距离实时时间的秒数，部分业务不能跑到实时，秒</summary>
        public Int32 Offset { get; set; }

        /// <summary>步进。最大区间大小，秒</summary>
        public Int32 Step { get; set; }

        /// <summary>批大小</summary>
        public Int32 BatchSize { get; set; }

        /// <summary>总数</summary>
        public Int32 Total { get; set; }

        /// <summary>成功</summary>
        public Int32 Success { get; set; }

        /// <summary>耗时，秒</summary>
        public Int32 Cost { get; set; }

        /// <summary>错误</summary>
        public Int32 Error { get; set; }

        /// <summary>次数</summary>
        public Int32 Times { get; set; }

        /// <summary>速度</summary>
        public Int32 Speed { get; set; }

        /// <summary>状态</summary>
        public JobStatus Status { get; set; }

        /// <summary>服务器</summary>
        public String Server { get; set; }

        /// <summary>进程</summary>
        public Int32 ProcessID { get; set; }

        /// <summary>数据</summary>
        public String Data { get; set; }

        /// <summary>最后键值</summary>
        public String Key { get; set; }

        /// <summary>内容</summary>
        public String Message { get; set; }
        #endregion
    }
}