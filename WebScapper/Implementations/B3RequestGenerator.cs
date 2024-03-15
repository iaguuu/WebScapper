using System.Collections.Generic;
using System.Net.Http;

public class B3RequestGenerator : IRequestGenerator<HttpClientRequest>
{
    public List<HttpClientRequest> GenerateRequests(List<HttpClientRequest> items = null)
    {
        return new List<HttpClientRequest>()
        {
            new HttpClientRequest { url = "https://sistemaswebb3-listados.b3.com.br/fundsProxy/fundsCall/GetListFundDownload/eyJ0eXBlRnVuZCI6NywicGFnZU51bWJlciI6MSwicGFnZVNpemUiOjIwfQ==", method = HttpMethod.Get },
            new HttpClientRequest { url = "https://sistemaswebb3-listados.b3.com.br/fundsProxy/fundsCall/GetListFundDownload/eyJ0eXBlRnVuZCI6MjcsInBhZ2VOdW1iZXIiOjEsInBhZ2VTaXplIjoyMH0=", method = HttpMethod.Get },
            new HttpClientRequest { url = "https://sistemaswebb3-listados.b3.com.br/fundsProxy/fundsCall/GetListFundDownload/eyJ0eXBlRnVuZCI6MzQsInBhZ2VOdW1iZXIiOjEsInBhZ2VTaXplIjoyMH0=", method = HttpMethod.Get }
        };
    }
}