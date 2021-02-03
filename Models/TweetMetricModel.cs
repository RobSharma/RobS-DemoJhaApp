using System.Collections.Generic;

namespace RobS_DemoJhaApp.Models
{
    public class TweetMetricModel : ITweetMetricModel
    {
        public TweetMetricModel()
        {
            DomainsOfUrls = new Dictionary<string, int>();
            HashTags = new Dictionary<string, int>();
            Emojis = new Dictionary<string, int>();
            TopDomainsOfUrls = new Dictionary<string, int>();
            TopEmojis = new Dictionary<string, int>();
            TopHashTags = new Dictionary<string, int>();

        }
        public int NumTweetsWithEmojis { get; set; }

        public Dictionary<string, int> HashTags;
        public Dictionary<string, int> Emojis;
        public Dictionary<string, int> DomainsOfUrls;

        public Dictionary<string, int> TopHashTags;
        public Dictionary<string, int> TopEmojis;
        public Dictionary<string, int> TopDomainsOfUrls;

        public int NumTweetsWithUrls { get; set; }
        public int NumOfTweetsWithPhoto { get; set; }


        public long TotalCount { get; set; }
        public long AvgSeconds { get; set; }
        public long AvgMinutes { get; set; }
        public long AvgHours { get; set; }
        public long ElapsedSeconds { get; set; }
    }
}
