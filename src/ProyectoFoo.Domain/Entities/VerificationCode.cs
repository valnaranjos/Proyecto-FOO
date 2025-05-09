using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Domain.Entities
{
    public class VerificationCode
    {
        public int UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
    }
}
