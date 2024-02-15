namespace api.DTOs
{
    public class returnExtratoDTO
    {
        public ReadSaldoDTO? Saldo { get; set; }
        public List<ReadTransacoesDTO>? Ultimas_Transacoes { get; set; }
    }

    public class ReadSaldoDTO
    {
        public int Total { get; set; }
        public DateTime Data_Extrato { get; set; }
        public int Limite { get; set; }
    }

    public class ReadTransacoesDTO
    {
        public int Valor { get; set; }
        public string? Tipo { get; set; }
        public string? Descricao { get; set; }
        public DateTime Realizada_Em { get; set; }
    }
}