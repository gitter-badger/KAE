﻿using System.Diagnostics;
using KJFramework.PerformanceProvider;

namespace KJFramework.Net.Channels
{
    /// <summary>
    ///   内部计数器
    /// </summary>
    [PerfCategory("KJFramework.Net.Channels", PerformanceCounterCategoryType.MultiInstance)]
    public class ChannelCounter
    {
        #region Members

        public static readonly ChannelCounter Instance = PerfCounterFactory.GetCounters<ChannelCounter>();

        /// <summary>
        ///   原地解析数据数
        /// </summary>
        [PerfCounter("Direct parse binary data in a long buffer. /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
        public PerfCounter RateOfDirectParse;
        /// <summary>
        ///   归还内存片段数
        /// </summary>
        [PerfCounter("Giveback mem-segment. /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
        public PerfCounter RateOfMemSegmentGiveback;
        /// <summary>
        ///   租借内存片段数
        /// </summary>
        [PerfCounter("Rent mem-segment. /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
        public PerfCounter RateOfRentMemSegment;

        #endregion

        #region Methods

        /// <summary>
        ///     Active the counters of performance.
        /// </summary>
        public void Initialize()
        {
            /*nothing to do.*/
        }

        #endregion
    }
}