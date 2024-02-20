namespace api.Models
{
    public class Transacao : BaseModel
    {
        public int IdCliente { get; set; }
        public int Valor { get; set; }
        public string? Tipo { get; set; }
        public string? Descricao { get; set; }
        public bool IsValid()
        {
            if ((Tipo != "c" && Tipo != "d") || string.IsNullOrEmpty(Descricao) || Descricao.Length > 10 || Valor < 0)
            {
                return false;
            }
            return true;
        }
    }
}