using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

public class B3Processor : IWebProcessor<FundoImobiliario>
{
    public async Task ProcessRequestAsync(List<HttpResponseMessage> HttpResponseMessage, List<FundoImobiliario> entityList)
    {
        DataTable fundos = await RequestToDataTable(HttpResponseMessage);

        entityList.AddRange((from DataRow dr in fundos.Rows
                             let ticker = dr["TICKER"].ToString().EndsWith("11") ? dr["TICKER"].ToString() : dr["TICKER"].ToString() + "11"
                             where !entityList.Any(f => f.ticker == ticker)
                             select new FundoImobiliario(ticker, dr["TIPO_ATIVO"].ToString())).ToList());
    }

    private static async Task<DataTable> RequestToDataTable(List<HttpResponseMessage> HttpResponseMessage)
    {
        DataTable mergedTable = new DataTable();

        foreach (HttpResponseMessage responseMessage in HttpResponseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode) { continue; } //marcar log

            string content = await responseMessage.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content)) { continue; } //marcar log

            string requestResultDecoded = Functions.decodeBase64(content.Trim('"'), "ISO-8859-1");

            DataTable result = Functions.ConvertCsvStringToDataTable(false, requestResultDecoded, ';');

            result.Columns["Código"].ColumnName = "TICKER";

            DataColumn newColumn = new DataColumn("TIPO_ATIVO", typeof(System.String));
            newColumn.DefaultValue = TipoAtivo(responseMessage.RequestMessage.RequestUri.AbsoluteUri);
            result.Columns.Add(newColumn);

            mergedTable.Merge(result);
        }

        return mergedTable;
    }
    private static string TipoAtivo(string urlRequest)
    {
        if (urlRequest.Contains("eyJ0eXBlRnVuZCI6NywicGFnZU51bWJlciI6MSwicGFnZVNpemUiOjIwfQ==")) { return "FII"; }  //0 FII
        else if (urlRequest.Contains("eyJ0eXBlRnVuZCI6MjcsInBhZ2VOdW1iZXIiOjEsInBhZ2VTaXplIjoyMH0=")) { return "FII_INFRA"; } //1 FII_INFRA
        else if (urlRequest.Contains("eyJ0eXBlRnVuZCI6MzQsInBhZ2VOdW1iZXIiOjEsInBhZ2VTaXplIjoyMH0=")) { return "FII_AGRO"; } //2 FII_AGRO
        return "";
    }
}
