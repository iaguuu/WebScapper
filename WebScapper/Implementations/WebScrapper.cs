using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class WebScrapper : IWebScrapper
{

    public async Task<List<HttpResponseMessage>> MakeMultipleRequestsAsync(List<HttpClientRequest> HttpClientRequest, int batchSize, TimeSpan timeDelay)
    {
        List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

        using (var httpClient = new HttpClient())
        {

            for (int i = 0; i < HttpClientRequest.Count(); i += batchSize)
            {
                List<HttpClientRequest> batch = HttpClientRequest.Skip(i).Take(batchSize).ToList();

                foreach (HttpClientRequest request in batch)
                {
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(request.method, request.url);

                    if (request.headers != null)
                    {
                        foreach (string header in request.headers.AllKeys)
                            httpRequestMessage.Headers.Add(header, request.headers[header]);
                    }

                    try
                    {
                        responses.Add(await httpClient.SendAsync(httpRequestMessage));
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentNullException(nameof(ex)); //marcar log
                    }
                }

                await Task.Delay(timeDelay);
            }

        }

        return responses;
    }
    public async Task<HttpResponseMessage> MakeRequestAsync(HttpClientRequest HttpClientRequest)
    {
        using (var httpClient = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpClientRequest.method, HttpClientRequest.url);
            if (HttpClientRequest.headers != null)
            {
                foreach (var header in HttpClientRequest.headers.AllKeys)
                    request.Headers.Add(header, HttpClientRequest.headers[header]);
            }

            return await httpClient.SendAsync(request);
        }
    }
    public Task<HttpResponseMessage> MakeRequestSync(HttpClientRequest HttpClientRequest)
    {
        throw new NotImplementedException();
    }
}