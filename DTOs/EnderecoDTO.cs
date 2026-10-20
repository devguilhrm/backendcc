namespace ATVbackend.DTOs
{
    public class EnderecoDTO
    {
        public int IdPessoa { get; set; }

        public int IdEndereco { get; set; }

        public string? CEP { get; set; }

        public long? Numero { get; set; }

        public string? Complemento { get; set; }
    }
}
