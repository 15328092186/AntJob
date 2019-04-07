﻿using System;
using AntJob;

namespace HelloWork
{
    internal class HelloJob : Job
    {
        public HelloJob()
        {
            // 今天零点开始，每5分钟一次
            var job = Model;
            job.Start = DateTime.Today;
            job.Step = 5 * 60;
        }

        protected override Int32 Execute(JobContext ctx)
        {
            // 当前任务时间
            var time = ctx.Task.Start;
            WriteLog("新生命蚂蚁调度系统！当前任务时间：{0}", time);

            // 成功处理数据量
            return 1;
        }
    }
}