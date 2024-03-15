using System;
using System.Collections.Generic;

public class FundoImobiliario : Investimento
{
    public string segmento { get; set; }
    public string publicoAlvo { get; set; }
    public string mandato { get; set; }
    public string tipoDeFundo { get; set; }
    public string prazoDeDuracao { get; set; }
    public string tipoDeGestao { get; set; }
    public string taxaDeAdministracao { get; set; }
    public string vacancia { get; set; }
    public string numeroDeCotistas { get; set; }
    public string cotasEmitidas { get; set; }
    public string valorPatrimonialPorCota { get; set; }
    public string valorPatrimonial { get; set; }
    public string ultimoRendimento { get; set; }
    public List<Dividendo> DividendHistory { get; set; } = new List<Dividendo>();
    public List<Cotacao> CotacaoHistory { get; set; } = new List<Cotacao>();

    public FundoImobiliario(string ticker, string tipoInvestimento) : base(ticker, tipoInvestimento)
    {
        if (!ticker.EndsWith("11")) { throw new ArgumentException("O ticker do fundo imobiliário deve terminar com '11';"); }
        this.ticker = ticker;
        this.tipoInvestimento = StringToInvestimentoType(tipoInvestimento);
    }

}