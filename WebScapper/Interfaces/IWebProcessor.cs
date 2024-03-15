using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public interface IWebProcessor<T>
{
    Task ProcessRequestAsync(List<HttpResponseMessage> HttpResponseMessage, List<T> entityList);
}