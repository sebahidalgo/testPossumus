using ApiRRHH.Services.Models;
using ApiRRHH.Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRRHH.Services.RequestResponses
{
    public class BuscarCandidatosResponse : BaseResponse
    {
        public IEnumerable<CandidatoDTO> Candidatos { get; set; }
    }
}
