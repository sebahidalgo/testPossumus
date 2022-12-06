using ApiRRHH.Services.Context;
using ApiRRHH.Services.Models;
using ApiRRHH.Services.RequestResponses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRRHH.Services
{
    public class ApiRRHHService : IApiRRHHService
    {
        private readonly ApiRRHHContext _context;
        private readonly ILogger _logger;

        public ApiRRHHService(ApiRRHHContext context, ILogger<ApiRRHHService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AgregarCandidatoResponse> AgregarCandidato(AgregarCandidatoRequest request)
        {
            AgregarCandidatoResponse response = new AgregarCandidatoResponse();
            
            //Verifica que no haya sido agregado anteriormente
            var existe = await _context.Candidatos.Where(c => c.Dni == request.Candidato.Dni).AnyAsync();

            if (existe)
            {
                response.AddError("Candidato", "Candidato agregado anteriormente");
                return response;
            }

            _context.Candidatos.Add(DTOToCandidato( request.Candidato));

            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<BuscarCandidatosResponse> BuscarCandidatos(BuscarCandidatosRequest request)
        {
            BuscarCandidatosResponse response = new BuscarCandidatosResponse();
            response.Candidatos = await _context.Candidatos.Include("Empleos").Select(x => CandidatoToDTO(x))
            .ToListAsync();
          
            return response;
        }

        public async Task<EliminarCandidatoResponse> EliminarCandidato(EliminarCandidatoRequest request)
        {
            EliminarCandidatoResponse response = new EliminarCandidatoResponse();
            response.Encontrado = true;

            //recupera DATA
            var candidato = await _context.Candidatos.Include("Empleos").FirstOrDefaultAsync( c => c.Dni == request.CandidatoId);
            if (candidato == null) {
                //DATA no encontrado
                response.Encontrado = false;
                return response;
            }

            foreach( var empleo in candidato.Empleos)
            {
                _context.Empleos.Remove(empleo);
            }
            _context.Candidatos.Remove(candidato);

            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<ModificarCandidatoResponse> ModificarCandidato(ModificarCandidatoRequest request)
        {
            ModificarCandidatoResponse response = new ModificarCandidatoResponse();
            response.Encontrado = true;

            //recupera DATA
            var candidato = await _context.Candidatos.Include("Empleos").FirstOrDefaultAsync(c => c.Dni == request.CandidatoDTO.Dni);
            if (candidato == null)
            {
                //DATA no encontrado
                response.Encontrado = false;
                return response;
            }

            //Por cada empleo en DATA, verifica la existencia en DTO.
            //Si no existe, elimina.
            foreach (var empleo in candidato.Empleos)
            {
                var existe = request.CandidatoDTO.Empleos.Any( e => e.Id == empleo.Id);
                if (!existe)
                {
                    _context.Empleos.Remove(empleo);
                }
            }

            //Por cada empleo en DTO, si ID > 0 => modifica. Sino, es un nuevoEmpleo a agregar.
            foreach (var empleoDTO in request.CandidatoDTO.Empleos)
            {
                if (empleoDTO.Id > 0)
                {
                    var empleo = candidato.Empleos.SingleOrDefault(e => e.Id == empleoDTO.Id);
                    if (empleo != null)
                    {
                        empleo.Periodo = empleoDTO.Periodo;
                        empleo.NombreEmpresa = empleoDTO.NombreEmpresa;
                    }
                }
                else
                {
                    //nuevo empleo
                    Empleo nuevoEmpleo = new Empleo
                    {
                        NombreEmpresa = empleoDTO.NombreEmpresa,
                        Periodo = empleoDTO.Periodo,
                        CandidatoId = candidato.Dni
                    };
                    candidato.Empleos.Add(nuevoEmpleo);
                }
            }

            //Actualiza entidad candidato con DTO
            candidato.Apellido = request.CandidatoDTO.Apellido;
            candidato.Email = request.CandidatoDTO.Email;
            candidato.Nombre = request.CandidatoDTO.Nombre;
            candidato.Telefono = request.CandidatoDTO.Telefono;

            await _context.SaveChangesAsync();
            return response;
        }

        /// <summary>
        /// Mapear entidad Empleo a EmpleoDTO
        /// </summary>
        /// <param name="empleo"></param>
        /// <returns></returns>
        private static EmpleoDTO EmpleoToDTO(Empleo empleo)
        {
            return new EmpleoDTO
            {
                Id = empleo.Id,
                NombreEmpresa = empleo.NombreEmpresa,
                Periodo = empleo.Periodo
            };
        }

        /// <summary>
        /// Mapear entidad EmpleoDTO a Empleo
        /// </summary>
        /// <param name="empleoDTO"></param>
        /// <param name="candidatoId"></param>
        /// <returns></returns>
        private static Empleo DTOToEmpleo(EmpleoDTO empleoDTO, int candidatoId)
        {
           return new Empleo
            {
                Id = empleoDTO.Id??0,
                NombreEmpresa = empleoDTO.NombreEmpresa,
                Periodo = empleoDTO.Periodo,
                CandidatoId = candidatoId
            };
        }

        /// <summary>
        /// Mapear entidad Candidato a CandidatoDTO
        /// </summary>
        /// <param name="candidato"></param>
        /// <returns></returns>
        private static CandidatoDTO CandidatoToDTO(Candidato candidato)
        {
            CandidatoDTO candidatoDTO = new CandidatoDTO();
            candidatoDTO.Dni = candidato.Dni;
            candidatoDTO.Email = candidato.Email;
            candidatoDTO.Nombre = candidato.Nombre;
            candidatoDTO.Apellido = candidato.Apellido;
            candidatoDTO.Telefono = candidato.Telefono;
            candidatoDTO.FechaNacimiento = candidato.FechaNacimiento;
            foreach ( var empleo in candidato.Empleos)
            {
                candidatoDTO.Empleos.Add( EmpleoToDTO(empleo));
            }
            return candidatoDTO;
        }

        /// <summary>
        /// Mapear entidad CandidatoDTO a Candidato
        /// </summary>
        /// <param name="candidatoDTO"></param>
        /// <returns></returns>
        private static Candidato DTOToCandidato(CandidatoDTO candidatoDTO)
        {
            Candidato candidato = new Candidato();
            candidato.Dni = candidatoDTO.Dni;
            candidato.Email = candidatoDTO.Email;
            candidato.Nombre = candidatoDTO.Nombre;
            candidato.Apellido = candidatoDTO.Apellido;
            candidato.Telefono = candidatoDTO.Telefono;
            candidato.FechaNacimiento = candidatoDTO.FechaNacimiento;
            foreach (var empleoDTO in candidatoDTO.Empleos)
            {
                candidato.Empleos.Add( DTOToEmpleo(empleoDTO, candidatoDTO.Dni));
            }
            return candidato;
        }

    }
}
