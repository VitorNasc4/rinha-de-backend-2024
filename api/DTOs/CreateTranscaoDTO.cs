namespace api.DTOs
{
    public class CreateTranscaoDTO
    {
        public int Valor { get; set; }
        public string? Tipo { get; set; }
        public string? Descricao { get; set; }
        public bool IsValid()
        {
            if (Tipo != "c" && Tipo != "d")
            {
                return false;
            }
            if (string.IsNullOrEmpty(Descricao) || Descricao.Length > 10)
            {
                return false;
            }
            return true;
        }
    }
}