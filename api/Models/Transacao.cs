namespace api.Models
{
    public class Transacao : BaseModel
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
            if (string.IsNullOrEmpty(Descricao))
            {
                return false;
            }
            return true;
        }
    }
}