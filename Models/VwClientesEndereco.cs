using System;
using System.Collections.Generic;

namespace ATVbackend.Models;

public partial class VwClientesEndereco
{
    public string Nome { get; set; } = null!;

    public string Cep { get; set; } = null!;

    public long? Numero { get; set; }

    public string Complemento { get; set; } = null!;
}
