using System.Collections.Generic;
public interface IRequestGenerator
{
    List<HttpClientRequest> GenerateRequests();
}