namespace Try_More_On_IEnumerable
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/IBHOxI
    /// </summary>
    public class T10测试网络连接
    {
        public static async Task Run()
        {
            var httpClient = new HttpClient();
            try
            {
                await Task.WhenAll(SendRequests());
                Console.WriteLine("当前网络连接正常");
            }
            catch (Exception e)
            {
                Console.WriteLine("当前网络不正常，请检查网络连接");
            }

            IEnumerable<Task> SendRequests()
            {
                yield return Task.Run(() => httpClient.GetAsync("http://www.baidu.com"));
                yield return Task.Run(() => httpClient.GetAsync("http://www.bing.com"));
                yield return Task.Run(() => httpClient.GetAsync("http://www.taobao.com"));
            }
        }
    }
}