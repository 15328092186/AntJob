﻿using System;
using System.ComponentModel;
using System.Reflection;
using NewLife.Xml;

namespace AntJob
{
    /// <summary>蚂蚁配置。主要用于网络型调度系统</summary>
    [XmlConfigFile("Config/Ant.config", 15000)]
    public class Setting : XmlConfig<Setting>
    {
        #region 属性
        /// <summary>调试开关。默认true</summary>
        [Description("调试开关。默认true")]
        public Boolean Debug { get; set; } = true;

        /// <summary>调度中心。逗号分隔多地址，主备架构</summary>
        [Description("调度中心。逗号分隔多地址，主备架构")]
        public String Server { get; set; } = "tcp://127.0.0.1:9999,tcp://ant.newlifex.com:9999";

        /// <summary>应用标识。调度中心以此隔离应用，默认当前应用</summary>
        [Description("应用标识。调度中心以此隔离应用，默认当前应用")]
        public String AppID { get; set; }

        /// <summary>应用密钥。</summary>
        [Description("应用密钥。")]
        public String Secret { get; set; }
        #endregion

        #region 方法
        /// <summary>重载</summary>
        protected override void OnLoaded()
        {
            if (AppID.IsNullOrEmpty())
            {
                var asm = Assembly.GetEntryAssembly();
                if (asm != null) AppID = asm.GetName().Name;
            }

            base.OnLoaded();
        }
        #endregion
    }
}