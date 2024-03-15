using System.Net.Http;
using System.Net;

public class HttpClientRequest
{
    public string url { get; set; }
    public HttpMethod method { get; set; }
    public WebHeaderCollection headers { get; set; }

}