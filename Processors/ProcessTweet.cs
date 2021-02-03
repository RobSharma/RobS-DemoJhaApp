using System;
using System.Linq;
using RobS_DemoJhaApp.Models;
using Tweetinvi.Models;

namespace RobS_DemoJhaApp.Processors
{
    public class ProcessTweets
    {
        public static TweetMetricModel GenerateMetrics(long elapsed, ITweet tweet, TweetMetricModel tweetMetrics)
        {
            tweetMetrics.TotalCount++;
            tweetMetrics.ElapsedSeconds = elapsed;
            tweetMetrics.AvgSeconds = (elapsed > 0) ? tweetMetrics.TotalCount / elapsed : 1;
            tweetMetrics.AvgMinutes = (elapsed > 60) ? tweetMetrics.TotalCount / (elapsed / 60) : 0;
            tweetMetrics.AvgHours = (elapsed > 3600) ? tweetMetrics.TotalCount / (elapsed / 3600) : 0;
            
            tweetMetrics = CheckHashTags(tweet, tweetMetrics);
            tweetMetrics = CheckUrls(tweet, tweetMetrics);
            tweetMetrics = CheckPhotos(tweet, tweetMetrics);

            return tweetMetrics;
        }

        private static TweetMetricModel CheckHashTags(ITweet tweet, TweetMetricModel tweetModel)
        {
            if (tweet.Entities.Hashtags.Count > 0)
            {
                try{
                    tweet.Entities.Hashtags.ForEach(h =>
                    {
                        if (tweetModel.TopHashTags.ContainsKey(h.Text))
                        {
                            tweetModel.TopHashTags[h.Text] += 1;
                        }
                        else
                        {
                            tweetModel.TopHashTags.Add(h.Text, 1);
                        }
                    });

                    tweetModel.TopHashTags = (from entry in tweetModel.HashTags orderby entry.Value descending select entry)
                        .Take(4)
                        .ToDictionary(pair => pair.Key, pair => pair.Value);
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }

            return tweetModel;
        }

        private static TweetMetricModel CheckPhotos(ITweet tweet, TweetMetricModel tweetModel)
        {
            if (tweet.Media.Count > 0)
            {
                tweet.Media.ForEach(m =>
                {
                    if (m.MediaType == "photo" &&
                        (m.DisplayURL.Contains("pic.twitter.com") || m.MediaURL.Contains("Instagram")))
                    {
                        tweetModel.NumOfTweetsWithPhoto++;
                    }
                });
            }

            return tweetModel;
        }

        private static TweetMetricModel CheckUrls(ITweet tweet, TweetMetricModel tweetModel)
        {
            if (tweet.Entities.Urls.Count > 0)
            {
                try
                {
                    tweetModel.NumTweetsWithUrls++;
                    tweet.Entities.Urls.ForEach(u =>
                    {
                        if (tweetModel.TopDomainsOfUrls.ContainsKey(u.URL))
                        {
                            tweetModel.DomainsOfUrls[u.URL] += 1;
                        }
                        else
                        {
                            tweetModel.DomainsOfUrls.Add(u.URL, 1);
                        }
                    });


                    tweetModel.TopDomainsOfUrls = (from entry in tweetModel.DomainsOfUrls orderby entry.Value descending select entry)
                        .Take(4)
                        .ToDictionary(pair => pair.Key, pair => pair.Value);
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }

            return tweetModel;
        }
    }
}