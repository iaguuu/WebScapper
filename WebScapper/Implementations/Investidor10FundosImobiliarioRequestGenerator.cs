using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

public class Investidor10FundosImobiliarioRequestGenerator : IRequestGenerator<FundoImobiliario>
{
    public List<HttpClientRequest> GenerateRequests(List<FundoImobiliario> fundosImobiliarios)
    {
        return (from FundoImobiliario fii in fundosImobiliarios
                select new HttpClientRequest { url = $"https://investidor10.com.br/FIIS/{fii.ticker}", method = HttpMethod.Get }
                   ).ToList();
    }
}