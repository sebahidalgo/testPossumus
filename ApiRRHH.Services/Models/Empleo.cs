using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRRHH.Services.Models
{
    public class Empleo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string NombreEmpresa { get; set; }
        public string Periodo { get; set; }
        public int CandidatoId { get; set; }
        public virtual Candidato Candidato { get; set; } = null!;

    }
}
