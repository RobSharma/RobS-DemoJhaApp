namespace RobS_DemoJhaApp.Models
{
    public interface ITweetMetricModel
    {
        int NumTweetsWithEmojis { get; set; }
        int NumTweetsWithUrls { get; set; }
        int NumOfTweetsWithPhoto { get; set; }
        long TotalCount { get; set; }
        long AvgSeconds { get; set; }
        long AvgMinutes { get; set; }
        long AvgHours { get; set; }
        long ElapsedSeconds { get; set; }
    }
}