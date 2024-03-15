using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

public class Investidor10Processor : IWebProcessor<FundoImobiliario>
{
    private static WebScrapper WebScrapper = new WebScrapper();

    public async Task ProcessRequestAsync(List<HttpResponseMessage> HttpResponseMessage, List<FundoImobiliario> entityList)
    {
        HtmlDocument doc = new HtmlDocument();

        foreach (HttpResponseMessage responseMessage in HttpResponseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode) { continue; }

            FundoImobiliario investimentoConsultado = entityList.First(f => f.ticker == ExtractTickerFromUrl(responseMessage.RequestMessage.RequestUri.AbsoluteUri));
            if (investimentoConsultado == null) { continue; }

            string content = await responseMessage.Content.ReadAsStringAsync();
            doc.LoadHtml(content);

            setIndicators(investimentoConsultado, doc);
            setDividendHistory(investimentoConsultado, doc);
            SetGraphs(investimentoConsultado, doc);
        }
    }
    private string ExtractTickerFromUrl(string url)
    {
        return new Uri(url).Segments.Last().Trim('/').ToUpper();
    }
    private static void setIndicators(FundoImobiliario fundo, HtmlDocument doc)
    {
        HtmlNodeCollection htmlNodeCollection = doc.DocumentNode.SelectNodes("//*[@id='table-indicators']/div[@class='cell']/div[@class='desc']");
        if (htmlNodeCollection == null) { return; }

        foreach (HtmlNode node in htmlNodeCollection)
        {
            HtmlNode spanNode = node.SelectSingleNode("span"); // DESCRIPTION 
            HtmlNode valueNode = node.SelectSingleNode("div[@class='value']/span"); // VALUE

            if (spanNode == null || valueNode == null) { continue; }

            string spanNodeText = spanNode.InnerText.Trim();
            string spanNodeValue = valueNode.InnerText.Trim();

            if (spanNodeText == "CNPJ") { fundo.cnpj = spanNodeValue; }
            else if (spanNodeText == "Razão Social") { fundo.razaoSocial = spanNodeValue; }
            else if (spanNodeText == "SEGMENTO") { fundo.segmento = spanNodeValue; }
            else if (spanNodeText == "PÚBLICO-ALVO") { fundo.publicoAlvo = spanNodeValue; }
            else if (spanNodeText == "MANDATO") { fundo.mandato = spanNodeValue; }
            else if (spanNodeText == "TIPO DE FUNDO") { fundo.tipoDeFundo = spanNodeValue; }
            else if (spanNodeText == "PRAZO DE DURAÇÃO") { fundo.prazoDeDuracao = spanNodeValue; }
            else if (spanNodeText == "TIPO DE GESTÃO") { fundo.tipoDeGestao = spanNodeValue; }
            else if (spanNodeText == "TAXA DE ADMINISTRAÇÃO") { fundo.taxaDeAdministracao = spanNodeValue; }
            else if (spanNodeText == "VACÂNCIA") { fundo.vacancia = spanNodeValue; }
            else if (spanNodeText == "NUMERO DE COTISTAS") { fundo.numeroDeCotistas = spanNodeValue; }
            else if (spanNodeText == "COTAS EMITIDAS") { fundo.cotasEmitidas = spanNodeValue; }
            else if (spanNodeText == "VAL. PATRIMONIAL P/ COTA") { fundo.valorPatrimonialPorCota = spanNodeValue; }
            else if (spanNodeText == "VALOR PATRIMONIAL") { fundo.valorPatrimonial = spanNodeValue; }
            else if (spanNodeText == "ÚLTIMO RENDIMENTO") { fundo.ultimoRendimento = spanNodeValue; }

        }
    }
    private void setDividendHistory(FundoImobiliario fundo, HtmlDocument doc)
    {
        HtmlNodeCollection htmlNodeCollection = doc.DocumentNode.SelectNodes("//*[@id='table-dividends-history']//tr//td");
        if (htmlNodeCollection == null) { return; }

        List<Dividendo> dividendsHistoryList = new List<Dividendo>();

        for (int i = 0; i < htmlNodeCollection.Count; i += 4)
        {
            string tipoDividendo = htmlNodeCollection[i].InnerText;
            string parsedDataCom = htmlNodeCollection[i + 1].InnerText;
            string parsedDataPagamento = htmlNodeCollection[i + 2].InnerText;
            Double paredValor = Double.Parse(htmlNodeCollection[i + 3].InnerText);            
            dividendsHistoryList.Add(new Dividendo(tipoDividendo, parsedDataCom, parsedDataPagamento, paredValor));
        }

        fundo.DividendHistory = dividendsHistoryList;
        return;
    }
    private void SetGraphs(FundoImobiliario fundo, HtmlDocument doc)
    {
        if (Int32.TryParse(ExtractId(doc), out int numValue))
        {
            string requestResult = GetCotacaoValuesFromGraph(numValue).Result;
            setCotacaoValuesFromGraph(fundo, requestResult);
        }
    }
    private string ExtractId(HtmlDocument doc)
    {
        Match match = Regex.Match(doc.Text, @"/chart/(\d+)/");
        if (match.Success) return match.Groups[1].Value.Trim();
        return "";
    }
    private async Task<string> GetCotacaoValuesFromGraph(int fundId)
    {
        HttpResponseMessage respose = await WebScrapper.MakeRequestAsync(new HttpClientRequest { url = $"https://investidor10.com.br/api/fii/cotacoes/chart/{fundId}/1825/real/adjusted/true", method = HttpMethod.Get });
        return respose.Content.ToString();
    }
    private static void setCotacaoValuesFromGraph(FundoImobiliario fundo, string json)
    {
        if (!Functions.IsValidJson(json)) { return; }

        List<Cotacao> cotacaoHistoryList = new List<Cotacao>();
        JObject jsonObject = JObject.Parse(json);

        if (!jsonObject.ContainsKey("real")) { return; }

        foreach (JToken item in jsonObject.Property("real").Values().ToList())
        {
            Cotacao cotacao = new Cotacao(item.Value<Double>("price"), item.Value<string>("created_at"));
            cotacaoHistoryList.Add(cotacao);
        }

        fundo.CotacaoHistory = cotacaoHistoryList;
        return;
    }


}