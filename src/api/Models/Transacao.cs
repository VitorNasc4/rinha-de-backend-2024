using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Transacao
    {
        public int Id { get; set; }
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