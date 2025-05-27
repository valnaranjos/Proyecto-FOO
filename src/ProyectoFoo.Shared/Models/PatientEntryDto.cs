using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Shared.Models
{
    public class PatientEntryDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ActualizationDate { get; set; }
    }
}
