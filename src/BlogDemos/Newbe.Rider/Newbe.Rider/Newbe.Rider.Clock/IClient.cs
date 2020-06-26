using System;
using System.Threading.Tasks;

namespace Newbe.Rider.Clock
{
    public interface IClient
    {
        Task Post(string url, object body);
        Task Get(string url);
    }

    class ProxyClient : IClient
    {
        private readonly IClient _client;

        public ProxyClient(
            IClient client)
        {
            _client = client;
        }

        public Task Post(string url, object body)
        {
            return _client.Post(url, body);
        }

        public Task Get(string url)
        {
            return _client.Get(url);
        }
    }
}