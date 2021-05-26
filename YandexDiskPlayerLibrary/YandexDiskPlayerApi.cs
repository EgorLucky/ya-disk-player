using System;
using System.Net.Http;

namespace YandexDiskPlayerLibrary
{
    public class YandexDiskPlayerApi
    {
        protected string _accessToken;
        protected readonly HttpClient _client;
        public string AccessToken { set { _accessToken = value; } }

        public YandexDiskPlayerApi(HttpClient client)
        {
            _client = client;
        }

        public YandexDiskPlayerApi(HttpClient client, string accessToken)
        {
            _accessToken = accessToken;
            _client = client;
        }


    }
}
