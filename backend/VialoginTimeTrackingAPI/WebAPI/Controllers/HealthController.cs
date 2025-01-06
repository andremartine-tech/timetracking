namespace VialoginTimeTrackingAPI.WebAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Controlador de checagem de saúde da API.
    /// </summary>
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Endpoint de checagem de saúde da API.
        /// </summary>
        /// <returns>Retorna um OK com a mensagem "Healthy".</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "Healthy" });
        }
    }
}
