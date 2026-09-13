using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrackManagement.Application.DTOs;
using TrackManagement.Application.Interfaces.Services;

namespace TrackManagement.API.Controllers
{
    [ApiController]
    [Route("api/tracks")]
    public class TracksController : ControllerBase
    {
        private readonly ITrackService _trackService;

        public TracksController(ITrackService trackService)
        {
            _trackService = trackService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TrackDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] TrackFilterDto filter, CancellationToken ct)
        {
            var tracks = await _trackService.GetFilteredAsync(filter, ct);
            return Ok(tracks);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TrackDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var track = await _trackService.GetByIdAsync(id, ct);
            return Ok(track);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(TrackDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateTrackDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var created = await _trackService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("{id:int}/distribute")]
        [Authorize]
        [ProducesResponseType(typeof(TrackDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Distribute(int id, [FromBody] DistributeTrackDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _trackService.DistributeAsync(id, dto, ct);
            return Ok(result);
        }

        [HttpPatch("{id:int}/status")]
        [Authorize]
        [ProducesResponseType(typeof(TrackDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTrackStatusDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _trackService.UpdateStatusAsync(id, dto, ct);
            return Ok(result);
        }
    }

}
