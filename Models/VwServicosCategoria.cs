using System;
using System.Collections.Generic;

namespace ATVbackend.Models;

public partial class VwServicosCategoria
{
    public string Servico { get; set; } = null!;

    public decimal Preco { get; set; }

    public string Categoria { get; set; } = null!;
}
