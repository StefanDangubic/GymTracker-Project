using GymTracker.Api.Common;
using GymTracker.Application.Workouts;
using GymTracker.Application.Workouts.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/workouts")]
public class WorkoutsController : ControllerBase
{
    private readonly WorkoutService _workoutService;

    public WorkoutsController(WorkoutService workoutService)
    {
        _workoutService = workoutService;
    }

    [HttpGet]
    public async Task<ActionResult<List<WorkoutResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var workouts = await _workoutService.GetUserWorkoutsAsync(User.GetUserId(), cancellationToken);
        return Ok(workouts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var workout = await _workoutService.GetByIdAsync(id, User.GetUserId(), cancellationToken);
        return Ok(workout);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutResponseDto>> Create(CreateWorkoutDto dto, CancellationToken cancellationToken)
    {
        var workout = await _workoutService.CreateAsync(User.GetUserId(), dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = workout.Id }, workout);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WorkoutResponseDto>> Update(int id, UpdateWorkoutDto dto, CancellationToken cancellationToken)
    {
        var workout = await _workoutService.UpdateAsync(id, User.GetUserId(), dto, cancellationToken);
        return Ok(workout);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _workoutService.DeleteAsync(id, User.GetUserId(), cancellationToken);
        return NoContent();
    }
}
