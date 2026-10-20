using System;
using System.Collections.Generic;

namespace ATVbackend.Models;

public partial class VwProdutosEstoque
{
    public string Produto { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public int QuantidadeEstoque { get; set; }
}
