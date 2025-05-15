using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Shared.Models
{
    public class PatientNoteDto
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Content { get; set; }
    }
}
