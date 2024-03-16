using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebScapper
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Main
            var scrapper = new WebScrapper();
            var fundosImobiliariosRepository = new FundosImobiliariosRepository("Server=IAGO\\MSSQLSERVER01;Database=RENDA_VARIAVEL;Trusted_Connection=True;");

            List<FundoImobiliario> fundosImobiliarios = fundosImobiliariosRepository.ListAll();

            //B3
            var B3RequestGenerator = new B3RequestGenerator();
            var B3Processor = new B3Processor();

            List<HttpClientRequest> b3HttpClientRequestList = B3RequestGenerator.GenerateRequests();
            List<HttpResponseMessage> responsefundosImobiliariosB3 = await scrapper.MakeMultipleRequestsAsync(b3HttpClientRequestList, 10, new TimeSpan(30));
            
            //ADICIONA FUNDOS NAO CADASTRADOS A LISTA
            await B3Processor.ProcessRequestAsync(responsefundosImobiliariosB3, fundosImobiliarios);
            
            //INVESTIDOR 10 FUNDO IMOBILIARIO
            var investidor10RequestGenerator = new Investidor10RequestGenerator<FundoImobiliario>(fundosImobiliarios);
            var scrapperInvestidor10 = new Investidor10Processor();       

            List<HttpClientRequest> requestListInvestidor10 = investidor10RequestGenerator.GenerateRequests();
            List<HttpResponseMessage> ResponseListinvestidor10 = await scrapper.MakeMultipleRequestsAsync(requestListInvestidor10, 10, new TimeSpan(30));

            //PROCESSA OS FUNDOS NO INVESTIDOR 10
            await scrapperInvestidor10.ProcessRequestAsync(ResponseListinvestidor10, fundosImobiliarios);

            //SALVA AS INFORMACOES
            fundosImobiliariosRepository.BulkCopy(fundosImobiliarios);
        }

    }
}
