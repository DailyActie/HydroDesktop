using System.ComponentModel;

namespace HydroDesktop.DataDownload.DataAggregation
{
    /// <summary>
    /// Aggregation mode for calculating data values.
    /// </summary>
    internal enum AggregationMode
    {
        [Description("MIN")]
        Min,
        [Description("MAX")]
        Max,
        [Description("AVG")]
        Avg,
        [Description("SUM")]
        Sum,
    }
}