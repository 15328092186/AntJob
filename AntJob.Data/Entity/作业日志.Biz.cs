using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using NewLife.Data;
using XCode;

namespace AntJob.Data.Entity
{
    /// <summary>作业日志</summary>
    public partial class JobLog : EntityBase<JobLog>
    {
        #region 对象操作
        static JobLog()
        {
            // 累加字段
            var df = Meta.Factory.AdditionalFields;
            df.Add(__.Error);
            df.Add(__.Times);
        }

        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew">是否插入</param>
        public override void Valid(Boolean isNew)
        {
            // 如果没有脏数据，则不需要进行任何处理
            if (!HasDirty) return;

            var len = _.Data.Length;
            if (len > 0 && !Data.IsNullOrEmpty() && Data.Length > len) throw new InvalidOperationException($"字段[{__.Data}]超长");

            // 截断错误信息，避免过长
            len = _.Message.Length;
            if (!Message.IsNullOrEmpty() && len > 0 && Message.Length > len) Message = Message.Substring(0, len);
        }
        #endregion

        #region 扩展属性
        /// <summary>应用</summary>
        [XmlIgnore]
        //[ScriptIgnore]
        public App App => Extends.Get(nameof(App), k => App.FindByID(AppID));

        /// <summary>应用</summary>
        [XmlIgnore]
        //[ScriptIgnore]
        [DisplayName("应用")]
        [Map(__.AppID)]
        public String AppName => App?.Name;

        /// <summary>作业</summary>
        [XmlIgnore]
        //[ScriptIgnore]
        public Job Job => Extends.Get(nameof(Job), k => Job.FindByID(JobID));

        /// <summary>作业</summary>
        [XmlIgnore]
        //[ScriptIgnore]
        [DisplayName("作业")]
        [Map(__.JobID)]
        public String JobName => Job?.Name;
        #endregion

        #region 扩展查询
        /// <summary>根据编号查找</summary>
        /// <param name="id">编号</param>
        /// <returns>实体对象</returns>
        public static JobLog FindByID(Int32 id)
        {
            if (id <= 0) return null;

            // 单对象缓存
            return Meta.SingleCache[id];
        }

        /// <summary>根据编号、作业、状态查找</summary>
        /// <param name="id">编号</param>
        /// <param name="jobid">作业</param>
        /// <param name="status">状态</param>
        /// <returns>实体列表</returns>
        public static IList<JobLog> FindAllByIDAndJobIDAndStatus(Int32 id, Int32 jobid, JobStatus status)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.ID == id && e.JobID == jobid && e.Status == status);

            return FindAll(_.ID == id & _.JobID == jobid & _.Status == status);
        }

        /// <summary>根据编号、状态查找</summary>
        /// <param name="id">编号</param>
        /// <param name="status">状态</param>
        /// <returns>实体列表</returns>
        public static IList<JobLog> FindAllByIDAndStatus(Int32 id, JobStatus status)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.ID == id && e.Status == status);

            return FindAll(_.ID == id & _.Status == status);
        }

        public static IList<JobLog> FindAllByAppID(Int32 appid)
        {
            if (appid == 0) return new List<JobLog>();

            return FindAll(_.AppID == appid);
        }

        public static IList<JobLog> FindAllByJobId(Int32 jobid)
        {
            if (jobid == 0) return new List<JobLog>();

            return FindAll(_.JobID == jobid);
        }

        public static Int32 FindCountByJobId(Int32 jobid) => (Int32)FindCount(_.JobID == jobid);

        /// <summary>查找作业下小于指定创建时间的最后一个任务</summary>
        /// <param name="jobid"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static JobLog FindLastByJobId(Int32 jobid, DateTime createTime)
        {
            return FindAll(_.JobID == jobid & _.CreateTime < createTime, _.CreateTime.Desc(), null, 0, 1).FirstOrDefault();
        }
        #endregion

        #region 高级查询
        public static IEnumerable<JobLog> Search(Int32 id, Int32 appid, Int32 jobid, JobStatus status, DateTime start, DateTime end, String client, String key, PageParameter p)
        {
            var exp = new WhereExpression();

            if (id > 0) exp &= _.ID == id;
            if (appid > 0) exp &= _.AppID == appid;
            if (jobid > 0) exp &= _.JobID == jobid;
            if (status >= JobStatus.就绪) exp &= _.Status == status;
            if (!client.IsNullOrEmpty()) exp &= _.Client == client;
            if (!key.IsNullOrEmpty()) exp &= _.Data.Contains(key) | _.Message.Contains(key) | _.Key == key;
            exp &= _.Start.Between(start, end);

            return FindAll(exp, p);
        }

        /// <summary>获取该任务下特定状态的任务项</summary>
        /// <param name="taskid"></param>
        /// <param name="end"></param>
        /// <param name="maxRetry"></param>
        /// <param name="stats"></param>
        /// <param name="count">要申请的任务个数</param>
        /// <returns></returns>
        public static IList<JobLog> Search(Int32 taskid, DateTime end, Int32 maxRetry, JobStatus[] stats, Int32 count)
        {
            var exp = new WhereExpression();
            if (taskid > 0) exp &= _.JobID == taskid;
            if (maxRetry > 0) exp &= _.Times < maxRetry;
            exp &= _.Status.In(stats);
            exp &= _.UpdateTime >= DateTime.Now.AddDays(-7);
            if (end > DateTime.MinValue)
            {
                exp &= _.UpdateTime < end;
            }

            // 限制任务的错误次数，避免无限执行
            exp &= _.Error < 32;

            return FindAll(exp, _.ID.Asc(), null, 0, count);
        }
        #endregion

        #region 业务操作
        /// <summary>重置</summary>
        public void Reset()
        {
            Row = 0;
            Total = 0;
            Success = 0;
            Status = JobStatus.就绪;

            Save();
        }

        /// <summary>删除该ID及以前的作业项</summary>
        /// <param name="jobid"></param>
        /// <param name="maxid"></param>
        /// <returns></returns>
        public static Int32 DeleteByID(Int32 jobid, Int32 maxid) => maxid <= 0 ? 0 : Delete(_.JobID == jobid & _.ID <= maxid);

        public static Int32 DeleteByAppId(Int32 appid) => Delete(_.AppID == appid);
        #endregion
    }
}