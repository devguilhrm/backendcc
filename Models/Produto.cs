using System;
using System.Collections.Generic;

namespace ATVbackend.Models;

public partial class Produto
{
    public int Iditem { get; set; }

    public int CodigoPreco { get; set; }

    public decimal PrecoVenda { get; set; }

    public string Unidade { get; set; } = null!;

    public virtual Estoque? Estoque { get; set; }
}
