using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRRHH.Services.Models
{
    public class EmpleoDTO
    {
        public int? Id { get; set; }
        public string NombreEmpresa { get; set; }
        public string Periodo { get; set; }

    }
}
