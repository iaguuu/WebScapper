using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public interface IWebScrapper
{
    Task<List<HttpResponseMessage>> MakeMultipleRequestsAsync(List<HttpClientRequest> HttpClientRequest, int batchSize, TimeSpan timeDelay);
    Task<HttpResponseMessage> MakeRequestAsync(HttpClientRequest HttpClientRequest);
    Task<HttpResponseMessage> MakeRequestSync(HttpClientRequest HttpClientRequest);
}