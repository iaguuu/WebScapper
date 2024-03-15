using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

public class FundosImobiliariosRepository : IRepository<FundoImobiliario>
{

    private readonly string _connectionString;

    public FundosImobiliariosRepository(string connection)
    {
        _connectionString = connection;
    }

    public void BulkCopy(List<FundoImobiliario> fundosImobiliarios)
    {

        try
        {
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(_connectionString))
            {
                sqlBulkCopy.BatchSize = 10000;
                sqlBulkCopy.BulkCopyTimeout = 0;

                DataTable fundosImobiliariosDataTable = fundoImobiliarioAsDataTable(fundosImobiliarios);
                DataTable historicoCotacao = historicoCotacaoAsDataTable(fundosImobiliarios);
                DataTable historicoDividendos = historicoDividendoAsDataTable(fundosImobiliarios);

                Dictionary<string, string> columnMappingFundo = columnsMapping("FundoImobiliario");
                Dictionary<string, string> columnMappingCotacao = columnsMapping("COTACAO");
                Dictionary<string, string> columnMappingDividendo = columnsMapping("DIVIDEND");

                void CopyDataToDatabase(DataTable data, string tableName, Dictionary<string, string> columnMapping)
                {
                    foreach (var item in columnMapping)
                        sqlBulkCopy.ColumnMappings.Add(item.Key, item.Value);

                    sqlBulkCopy.DestinationTableName = tableName;
                    sqlBulkCopy.WriteToServer(data);
                    sqlBulkCopy.ColumnMappings.Clear();
                }

                CopyDataToDatabase(fundosImobiliariosDataTable, "FUNDOS_IMOBILIAROS", columnMappingFundo);
                CopyDataToDatabase(historicoCotacao, "COTACOES", columnMappingCotacao);
                CopyDataToDatabase(historicoDividendos, "DIVIDENDOS", columnMappingDividendo);

                return;
            }
        }
        catch (Exception ex)
        {
            //marcar log
            return;
        }
    }

    public List<FundoImobiliario> ListAll()
    {
        List<FundoImobiliario> fundosImobiliarios = new List<FundoImobiliario>
        {
                new FundoImobiliario("ALZR11", "FII"),
                new FundoImobiliario("RURA11", "FII"),
                new FundoImobiliario("JURO11", "FII"),
                new FundoImobiliario("CPTI11", "FII"),
        };
        return fundosImobiliarios;
       
    }
    
    private DataTable fundoImobiliarioAsDataTable(List<FundoImobiliario> fundosImobiliarios)
    {
        DataTable dt = CollectionHelper.ConvertTo(fundosImobiliarios);
        dt.Columns.Remove("DividendHistory");
        dt.Columns.Remove("CotacaoHistory");
        dt.AcceptChanges();
        return dt;
    }

    private DataTable historicoDividendoAsDataTable(List<FundoImobiliario> fundosImobiliarios)
    {
        //List<Dividend> historicoDividendos = fundosImobiliarios.SelectMany(fundo => fundo.DividendHistory).ToList();
        return CollectionHelper.ConvertTo(fundosImobiliarios.SelectMany(fundo => fundo.DividendHistory
                    .Select(dividendo =>
                    new { dividendo.valor, dividendo.dataPagamento, dividendo.tipoDividendo, dividendo.dataCom, fundo.ticker }
                    )).ToList());
    }
    private DataTable historicoCotacaoAsDataTable(List<FundoImobiliario> fundosImobiliarios)
    {
        //List<Cotacao> historicoCotacao = fundosImobiliarios.SelectMany(fundo => fundo.CotacaoHistory).ToList();
        return CollectionHelper.ConvertTo(fundosImobiliarios.SelectMany(fundo => fundo.CotacaoHistory
                    .Select(cotacao =>
                    new { cotacao.valor, cotacao.data, fundo.ticker }
                    )).ToList());
    }
    private Dictionary<string, string> columnsMapping(string mappingType)
    {

        Dictionary<string, string> mapping = new Dictionary<string, string>();

        if (mappingType == "FundoImobiliario")
        {
            mapping.Add("segmento", "SEGMENTO");
            mapping.Add("publicoAlvo", "PUBLICO_ALVO");
            mapping.Add("mandato", "MANDATO");
            mapping.Add("tipoDeFundo", "TIPO_FUNDO");
            mapping.Add("prazoDeDuracao", "PRAZO_DURACAO");
            mapping.Add("tipoDeGestao", "TIPO_GESTAO");
            mapping.Add("taxaDeAdministracao", "TAXA_ADMINISTRACAO");
            mapping.Add("vacancia", "VACANCIA");
            mapping.Add("numeroDeCotistas", "NUMERO_COTISTAS");
            mapping.Add("cotasEmitidas", "COTAS_EMITIDAS");
            mapping.Add("valorPatrimonialPorCota", "VALOR_PATRIMONIAL_POR_COTA");
            mapping.Add("valorPatrimonial", "VALOR_PATRIMONIAL");
            mapping.Add("ultimoRendimento", "ULTIMO_RENDIMENTO");
            mapping.Add("ticker", "TICKER");
            mapping.Add("tipoInvestimento", "TIPO_INVESTIMENTO");
            mapping.Add("cnpj", "CNPJ");
            mapping.Add("razaoSocial", "RAZAO_SOCIAL");
        }
        else if (mappingType == "COTACAO")
        {
            mapping.Add("ticker", "TICKER");
            mapping.Add("valor", "VALOR");
            mapping.Add("data", "DATA_COTACAO");
        }
        else if (mappingType == "DIVIDEND")
        {
            mapping.Add("ticker", "TICKER");
            mapping.Add("tipoDividendo", "TIPO_DIVIDENDO");
            mapping.Add("valor", "VALOR");
            mapping.Add("dataCom", "DATA_COM");
            mapping.Add("dataPagamento", "DATA_PAGAMENTO");
        }

        return mapping;
    }
}