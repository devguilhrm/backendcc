namespace ATVbackend.DTOs
{
    public class EntradaProdutoDTO
    {
        public int? IdEntradaProduto { get; set; }

        public int IdItem { get; set; }

        public DateOnly DataEntradaProduto { get; set; }

        public decimal QuantidadeEntrada { get; set; }

        public string Unidade { get; set; }

        public decimal ValorUnitarioEntrada { get; set; }
    }
}
