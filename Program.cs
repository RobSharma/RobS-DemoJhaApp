using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using RobS_DemoJhaApp.Models;
using RobS_DemoJhaApp.Processors;
using Server;
using Tweetinvi;
using Tweetinvi.Models;
using Timer = System.Timers.Timer;


namespace RobS_DemoJhaApp
{
    public class Program
    {
        private static Stopwatch _stopwatch = new Stopwatch();
        private static Timer _consoleTimer = new Timer(5000);

        public static async System.Threading.Tasks.Task Main(string[] args)
        {

            // Connect to twitter
            string consumerKey = ConfigurationManager.AppSettings.Get("ConsumerKey");
            string consumerSecret = ConfigurationManager.AppSettings.Get("ConsumerSecret");
            string accessToken = ConfigurationManager.AppSettings.Get("AccessToken");
            string accessTokenSecret = ConfigurationManager.AppSettings.Get("AccessTokenSecret");
            string bearerToken = ConfigurationManager.AppSettings.Get("BearerToken");

            var consumerOnlyCredentials =
                new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
            var appClient = new TwitterClient(consumerOnlyCredentials);
            
            await appClient.Auth.InitializeClientBearerTokenAsync();

            // Setup a rate limiter
            var limiter = new Throttle(1, TimeSpan.FromSeconds(5));
            long elapsedSeconds = 0;

            // Using the sample stream
            var stream = appClient.Streams.CreateSampleStream();
            stream.TweetMode = TweetMode.Extended;
            var tweetMetrics = new TweetMetricModel();
            stream.FilterLevel = Tweetinvi.Streaming.Parameters.StreamFilterLevel.Low;
            
            stream.TweetReceived += async (sender, t) =>
            {
                elapsedSeconds = (long) _stopwatch.Elapsed.TotalSeconds;
                tweetMetrics = ProcessTweets.GenerateMetrics(elapsedSeconds,t.Tweet, tweetMetrics);
                //if ((elapsed % 5) == 0)
                //{
                    var ct = new CancellationToken();
                    limiter.Enqueue(async () =>
                    {
                        UpdateConsole(tweetMetrics);
                    }, ct);
                //}
            };

            // Start
            _stopwatch.Start();
            try
            {
                await stream.StartAsync();
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        private static void UpdateConsole(TweetMetricModel tweetMetrics)
        {
            Console.Clear();
            Console.WriteLine("Total Tweets: " + tweetMetrics.TotalCount);
            Console.WriteLine("Time Elapsed: " + tweetMetrics.ElapsedSeconds);
            Console.WriteLine("Averages: Per Seconds: " + tweetMetrics.AvgSeconds + " Per Minute:" +
                              tweetMetrics.AvgMinutes +
                              " Per Hour: " + tweetMetrics.AvgHours);


            Console.WriteLine("Number of Tweets with Photos: " + tweetMetrics.NumOfTweetsWithPhoto);
            Console.WriteLine("Number of Tweets with URLs: " + tweetMetrics.NumTweetsWithUrls);
            Console.WriteLine("Number of Tweets with Emojis: <TBD>");

            foreach (var hshTg in tweetMetrics.TopHashTags)
            {
                    Console.WriteLine(string.Format($"Hash Tag {0}   Num Of Occurrences: {1}", hshTg.Key, hshTg.Value));
            }

            //topHashes = tweetMetrics.TopHashTags.Select(res => "Hash Tag " + res.Key + ": Num Occurrences = " + res.Value );
            //topUrls = tweetMetrics.TopDomainsOfUrls.Select(res => "Domain Url " +
            //       (new Uri(res.Key)).Host + ": Num Occurrences = " + res.Value);

            Console.WriteLine("\r\nTop Statistics:");
            Console.WriteLine("Top HashTags:" );
            foreach (var hshTg in tweetMetrics.TopHashTags)
            {
                Console.WriteLine($"Hash Tag {hshTg.Key}   Num Of Occurrences: {hshTg.Value}");
            }


            Console.WriteLine("Top Domains:");
            foreach (var dmns in tweetMetrics.TopDomainsOfUrls)
            {
                Console.WriteLine($"Hash Tag {dmns.Key}   Num Of Occurrences: {dmns.Value}");
            }
            Console.WriteLine("-------------------------------------");
        }
    }
}