using ApiRRHH.Services.RequestResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRRHH.Services
{
    public interface IApiRRHHService
    {
        Task<BuscarCandidatosResponse> BuscarCandidatos(BuscarCandidatosRequest request);
        Task<AgregarCandidatoResponse> AgregarCandidato(AgregarCandidatoRequest request);
        Task<ModificarCandidatoResponse> ModificarCandidato(ModificarCandidatoRequest request);
        Task<EliminarCandidatoResponse> EliminarCandidato(EliminarCandidatoRequest request);

    }
}
