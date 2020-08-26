﻿using System;
using System.ComponentModel;
using NewLife;
using NewLife.Cube;

namespace AntJob.Web.Areas.Ant
{
    /// <summary>蚂蚁调度</summary>
    [DisplayName("蚂蚁调度")]
    public class AntArea : AreaBase
    {
        public AntArea() : base(nameof(AntArea).TrimEnd("Area")) { }

        static AntArea() => RegisterArea<AntArea>();
    }
}