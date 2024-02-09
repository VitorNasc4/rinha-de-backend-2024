namespace api.Models
{
    public class Cliente : BaseModel
    {
        public int Limite { get; set; }
        public int Saldo { get; set; }
        public List<Transacao>? Transacoes { get; set; }
    }
}