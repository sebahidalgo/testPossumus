using ApiRRHH.Services;
using ApiRRHH.Services.RequestResponses;
using Microsoft.AspNetCore.Mvc;

namespace ApiRRHH.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiRRHHController : ControllerBase
    {
        private readonly IApiRRHHService _apiRRHHService;
        private readonly ILogger _logger;
        /// <summary>
        /// Controller DecidirSdk
        /// </summary>
        /// <param name="servicioDecidirSdk"></param>
        public ApiRRHHController(IApiRRHHService apiRRHHService, ILogger<ApiRRHHController> logger)
        {
            _apiRRHHService = apiRRHHService;
            _logger = logger;
        }

        [HttpGet("BuscarCandidatos")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BuscarCandidatosResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BuscarCandidatos([FromQuery]  BuscarCandidatosRequest request)
        {
            var responseService = await this._apiRRHHService.BuscarCandidatos(request);
            _logger.LogError("GET BuscarCanditados OK 200");
            return Ok(responseService);
        }

        [HttpPost("AgregarCandidato")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AgregarCandidatoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AgregarCandidato(AgregarCandidatoRequest request)
        {
            var responseService = await this._apiRRHHService.AgregarCandidato(request);
            if (responseService.Errores.Any())
            {
                foreach (var error in responseService.Errores)
                {
                    ModelState.AddModelError(error.Item1, error.Item2);
                }
                return BadRequest(ModelState);
            }
            _logger.LogError("POST AgregarCandidato Created 201");
            return CreatedAtAction(nameof(AgregarCandidato), new { id = request.Candidato.Dni }, responseService); ;
        }

        [HttpDelete("EliminarCandidato")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EliminarCandidato(EliminarCandidatoRequest request)
        {
            var responseService = await this._apiRRHHService.EliminarCandidato(request);
            if (responseService.Encontrado == false)
            {
                return NotFound();
            }
            if (responseService.Errores.Any())
            {
                foreach (var error in responseService.Errores)
                {
                    ModelState.AddModelError(error.Item1, error.Item2);
                }

                return BadRequest(ModelState);
            }
            _logger.LogError("DELETE EliminarCandidato NoContent 204");
            return NoContent();
        }

        [HttpPut("ModificarCandidato")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ModificarCandidato(ModificarCandidatoRequest request)
        {
            var responseService = await this._apiRRHHService.ModificarCandidato(request);
            if (responseService.Encontrado == false)
            {
                return NotFound();
            }
            if (responseService.Errores.Any())
            {
                foreach (var error in responseService.Errores)
                {
                    ModelState.AddModelError(error.Item1, error.Item2);
                }

                return BadRequest(ModelState);
            }
            _logger.LogError("PUT ModificarCandidato NoContent 204");
            return NoContent(); 
        }
    }
}
