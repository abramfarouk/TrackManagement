using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrackManagement.Application.DTOs;
using TrackManagement.Application.Interfaces.Services;

namespace TrackManagement.API.Controllers
{
    [ApiController]
    [Route("api/dsps")]
    public class DspsController : ControllerBase
    {
        private readonly IDspService _dspService;

        public DspsController(IDspService dspService)
        {
            _dspService = dspService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<DspDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var dsps = await _dspService.GetAllAsync(ct);
            return Ok(dsps);
        }
    }

}
