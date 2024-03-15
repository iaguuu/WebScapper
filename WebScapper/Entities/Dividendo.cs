using System;

public class Dividendo
{
    public DividendoType tipoDividendo { get; set; }
    public DateTime dataCom { get; set; }
    public DateTime dataPagamento { get; set; }
    public Double valor { get; set; }

    public Dividendo(string tipoDividendo, string dataCom, string dataPagamento, Double valor)
    {
        this.tipoDividendo = StringParaTipoDividendo(tipoDividendo);
        this.dataCom = stringDateToDateTime(dataCom);
        this.dataPagamento = stringDateToDateTime(dataPagamento);
        this.valor = valor;
    }
    private protected static DateTime stringDateToDateTime(string stringDate)
    {
        DateTime parsedDate;
        if (DateTime.TryParseExact(stringDate, new string[] { "dd/MM/yyyy", "yyyy-MM-dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
        {
            return DateTime.ParseExact(parsedDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
        }
        else
        {
            throw new ArgumentException("Data inválida. Certifique-se de fornecer uma data no formato 'dd/MM/yyyy' ou 'yyyy-MM-dd'.");
        }
    }
    private protected static DividendoType StringParaTipoDividendo(string texto)
    {
        texto = texto.ToUpper();
        if (texto == "JCP") return DividendoType.JCP;
        else if (texto == "DIVIDENDOS") return DividendoType.DIVIDENDO;
        else if (texto == "AMORTIZAÇÃO") return DividendoType.DIVIDENDO;
        throw new ArgumentException($"Tipo de dividendo inválido: {texto}");
    }
}