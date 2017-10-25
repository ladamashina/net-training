﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncIO
{
    public static class Tasks
    {


        /// <summary>
        /// Returns the content of required uris.
        /// Method has to use the synchronous way and can be used to compare the performace of sync \ async approaches. 
        /// </summary>
        /// <param name="uris">Sequence of required uri</param>
        /// <returns>The sequence of downloaded url content</returns>
        public static IEnumerable<string> GetUrlContent(this IEnumerable<Uri> uris)
        {

            using (var webClient = new MyWebClient())
            {
                foreach (var link in uris)
                {
                    yield return webClient.DownloadString(link);
                }
                
            }  

        }


        /// <summary>
        /// Returns the content of required uris.
        /// Method has to use the asynchronous way and can be used to compare the performace of sync \ async approaches. 
        /// 
        /// maxConcurrentStreams parameter should control the maximum of concurrent streams that are running at the same time (throttling). 
        /// </summary>
        /// <param name="uris">Sequence of required uri</param>
        /// <param name="maxConcurrentStreams">Max count of concurrent request streams</param>
        /// <returns>The sequence of downloaded url content</returns>
        public static IEnumerable<string> GetUrlContentAsync(this IEnumerable<Uri> uris, int maxConcurrentStreams)
        {
            async Task<string> ProcessUrlAsync(string url)
            {
                using (var webClient = new MyWebClient())
                {
                    string data = await webClient.DownloadStringTaskAsync(new Uri(url));
                    
                    return data;
                }
            }

            var enumerable = uris as IList<Uri> ?? uris.ToList();
            if (enumerable.Count > 0)
            {
                var tasks = new List<Task<string>>();
                foreach (var url in enumerable)
                {
                    tasks.Add(ProcessUrlAsync(url.ToString()));
                }

                 Task.WaitAll(tasks.ToArray()); // blocking wait

               yield return  tasks;
                // could use await here and make this method async:
                // await Task.WhenAll(tasks.ToArray());
            }

        }

        /// <summary>
        /// Calculates MD5 hash of required resource.
        /// 
        /// Method has to run asynchronous. 
        /// Resource can be any of type: http page, ftp file or local file.
        /// </summary>
        /// <param name="resource">Uri of resource</param>
        /// <returns>MD5 hash</returns>
        public static Task<string> GetMD5Async(this Uri resource)
        {
            // TODO : Implement GetMD5Async
            throw new NotImplementedException();
        }

    }

    class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return request;
        }
    }

}
