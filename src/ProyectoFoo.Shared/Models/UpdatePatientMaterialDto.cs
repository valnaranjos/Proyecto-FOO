﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Shared.Models
{
    public class UpdatePatientMaterialDto
    {
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public string? Content { get; set; }
    }
}
