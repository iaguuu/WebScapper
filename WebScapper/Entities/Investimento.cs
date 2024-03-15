using System;

public abstract class Investimento
{
    public string ticker { get; set; }
    public InvestimentoType tipoInvestimento { get; set; }
    public string cnpj { get; set; }
    public string razaoSocial { get; set; }

    public Investimento(string ticker, string tipoInvestimento)
    {
        if (string.IsNullOrEmpty(ticker)) { throw new ArgumentException("Ticker vazio ou em branco. Certifique-se de preenchê-lo"); }
        this.ticker = ticker;
        this.tipoInvestimento = StringToInvestimentoType(tipoInvestimento);
    }
    protected InvestimentoType StringToInvestimentoType(string texto)
    {
        texto = texto.ToUpper();
        if (texto == "FII") return InvestimentoType.FII;
        else if (texto == "FII_INFRA") return InvestimentoType.FII_INFRA;
        else if (texto == "FII_AGRO") return InvestimentoType.FII_AGRO;
        else if (texto == "ACOES") return InvestimentoType.ACOES;
        else if (texto == "BDRS") return InvestimentoType.BDRS;
        throw new ArgumentException($"Tipo de ativo inválido: {texto}");
    }
}