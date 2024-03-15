using System;

public class Cotacao
{
    public Double valor { get; set; }
    public DateTime data { get; set; }

    public Cotacao(Double valor, string data)
    {
        this.valor = valor;
        this.data = stringDateToDateTime(data);
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
}