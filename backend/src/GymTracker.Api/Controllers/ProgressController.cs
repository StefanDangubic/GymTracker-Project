using GymTracker.Api.Common;
using GymTracker.Application.Progress;
using GymTracker.Application.Progress.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/progress")]
public class ProgressController : ControllerBase
{
    private readonly ProgressService _progressService;

    public ProgressController(ProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet]
    public async Task<ActionResult<MonthlyProgressResponseDto>> GetMonthlyProgress(
        [FromQuery] ProgressQueryDto query, CancellationToken cancellationToken)
    {
        var progress = await _progressService.GetMonthlyProgressAsync(
            User.GetUserId(), query.Year, query.Month, cancellationToken);
        return Ok(progress);
    }
}
