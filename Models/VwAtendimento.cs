using System;
using System.Collections.Generic;

namespace ATVbackend.Models;

public partial class VwAtendimento
{
    public int Idatendimento { get; set; }

    public DateOnly DataAtendimento { get; set; }

    public string Cliente { get; set; } = null!;

    public string Item { get; set; } = null!;

    public int QuantidadeItens { get; set; }

    public decimal ValorunitarioEntrada { get; set; }

    public string Funcionario { get; set; } = null!;
}
