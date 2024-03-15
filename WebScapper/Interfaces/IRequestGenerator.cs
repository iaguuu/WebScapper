using System.Collections.Generic;

public interface IRequestGenerator<T>
{
    List<HttpClientRequest> GenerateRequests(List<T> items);
}