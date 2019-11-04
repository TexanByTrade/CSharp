using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;


namespace RedditCrawler
{
    class Program
    {
        public static List<string> visitedURLs = new List<string>();
        public static HashSet<string> visitedHash = new HashSet<string>(); //fast checking for previous occurence of url

        public static int totalScraped = 0;

        public static Regex hrefs = new Regex("r/\\w+?/comments/.+?\"", RegexOptions.Compiled);
        public static Regex users = new Regex("user/[^&]+?\"", RegexOptions.Compiled);

        public static HttpClient client = new HttpClient(); //USE A SHARED HTTPCLIENT
                                                            //yes, GetAsync is thread-safe

        static void Main(string[] args)
        {
            //Starting seed to begin scraping
            string seed = "http://old.reddit.com/r/all/";
            visitedURLs.Add(seed);
            visitedHash.Add(seed);

            List<string> queue = new List<string>() { seed }; // add seed to our queue and begin scraping
            List<Task<List<string>>> spiders = new List<Task<List<string>>>(); 

            while ((queue.Count > 0)||(spiders.Count>0))
            {
                for (int i = queue.Count - 1; i >= 0; --i)
                {
                    if (spiders.Count > 10000) //we use lots of spiders because much time is spent waiting for HTTP requests
                    {
                        break;
                    }
                    spiders.Add(ScrapeURL(queue[i]));
                    queue.RemoveAt(i);
                }

                Task sleep = Task.Delay(200); //we can't waste all the cpu on the HandleCompletedTasks loop;
                Task.WaitAll(sleep);
                if (!HandleCompletedTasks(queue, spiders))
                    break;

                //write stats:
                //Q = queue size, S = # of spiders, T = total URLs scraped
                Console.SetCursorPosition(0, Console.BufferHeight - 3);
                Console.Write("\n" + "Q:{0}\tS:{1}\tT:{2}", queue.Count, spiders.Count, totalScraped);
                Console.SetCursorPosition(0, Console.BufferHeight - 3);
            }

            Console.WriteLine("Cleaning up spiders (this might take awhile)...\n");
            while (spiders.Count > 0)
            {
                int i = Task.WaitAny(spiders.ToArray());
                spiders.RemoveAt(i);
                Console.Write("{0}        ", spiders.Count);
                Console.SetCursorPosition(0, Console.CursorTop);
            }
            visitedHash.Clear();

            //Dump found usernames/reddit posts
            HashSet<string> uniques = new HashSet<string>();
            using (StreamWriter sw = new StreamWriter("redditPages.txt"))
            using (StreamWriter su = new StreamWriter("redditUsers.txt"))
            {
                visitedURLs.Sort();
                foreach (string page in visitedURLs)
                {
                    if (page.Substring(23, 4) == "user")
                    {
                        int i = page.IndexOf('/', 28);
                        string trimmed = page.Substring(0, (i > 0 ? i : page.Length));
                        if (!uniques.Contains(trimmed))
                        {
                            uniques.Add(trimmed);
                            try
                            {
                                su.WriteLine(trimmed.Substring(23));
                            }
                            catch (ArgumentOutOfRangeException aoore)
                            {
                                //bad URL, just ignore
                            }
                        }
                    }
                    else
                    {
                        int i = page.IndexOf("/comments/");
                        int j = page.IndexOf('/', i + 10);
                        int k = page.IndexOf('/', j + 2);
                        string trimmed = page.Substring(0, (k > 0 ? k : page.Length));
                        if (!uniques.Contains(trimmed))
                        {
                            uniques.Add(trimmed);
                            try
                            {
                                sw.WriteLine(trimmed.Substring(23));
                            }
                            catch (ArgumentOutOfRangeException aoore) { }
                        }
                    }
                }
            }
        }

        static bool HandleCompletedTasks(List<string> queue, List<Task<List<string>>> spiders)
        {
            for (int i = spiders.Count - 1; i >= 0; --i)
            {
                //break if user presses Esc
                //We put this here instead of the main loop for maximum responsiveness
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        Console.Clear();
                        Console.WriteLine("Q:{0}    S:{1}    T:{2}\n", queue.Count, spiders.Count, totalScraped);
                        return false;
                    }
                }

                if (spiders[i].IsCompleted)
                {
                    List<string> news = spiders[i].Result;
                    for (int j = 0; j < news.Count; ++j)
                    {
                        string newU = news[j];

                        if (!visitedHash.Contains(newU))
                        {
                            visitedURLs.Add(newU);
                            visitedHash.Add(newU);
                            queue.Add(newU);

                            Console.Write(newU + "\n" + "Q:{0}\tS:{1}\tT:{2}\n", queue.Count, spiders.Count, ++totalScraped);
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                        }
                    }
                    spiders.RemoveAt(i);
                }

            }
            return true;
        }

        static async Task<List<string>> ScrapeURL(string url)
        {
            List<string> newURLs = new List<string>();
            
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (HttpContent content = response.Content)
                        {
                            string result = await content.ReadAsStringAsync();
                            MatchCollection urls = hrefs.Matches(result);
                            MatchCollection peeps = users.Matches(result);
                            foreach (Match m in urls)
                            {
                                try
                                {
                                    Uri parser = new Uri(new Uri("https://old.reddit.com/"), m.Value.TrimEnd('"'));
                                    newURLs.Add(parser.AbsoluteUri);
                                }
                                catch (Exception e) { return newURLs; }
                            }
                            foreach (Match m in peeps)
                            {
                                try
                                {
                                    Uri parser = new Uri(new Uri("https://old.reddit.com/"), m.Value.TrimEnd('"'));
                                    newURLs.Add(parser.AbsoluteUri);
                                }
                                catch (Exception e) { return newURLs; }
                            }
                        }
                    }
                }
            }
            catch (WebException we) { }
            catch (HttpRequestException hre) { }
            catch (Exception e) { }
           
            return newURLs;
        }
        
    }
}
