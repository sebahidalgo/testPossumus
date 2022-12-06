using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRRHH.Services.Models
{
    public class Candidato
    {
        public Candidato()
        {
            Empleos = new HashSet<Empleo>();
        }

        [Key]
        public int Dni { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }

        public virtual ICollection<Empleo> Empleos { get; set; }
    }
}
