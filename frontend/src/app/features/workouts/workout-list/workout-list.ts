import { HttpErrorResponse } from '@angular/common/http';
import { Component, HostListener, inject, signal } from '@angular/core';
import { Workout } from '../../../shared/models/workout.model';
import { formatWorkoutDateTime } from '../../../shared/utils/date-time.util';
import { extractErrorMessage } from '../../../shared/utils/extract-error-message';
import { WorkoutForm } from '../workout-form/workout-form';
import { WorkoutService } from '../workout.service';

@Component({
  selector: 'app-workout-list',
  imports: [WorkoutForm],
  templateUrl: './workout-list.html',
  styleUrl: './workout-list.scss'
})
export class WorkoutList {
  private readonly workoutService = inject(WorkoutService);

  readonly workouts = signal<Workout[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  readonly isFormOpen = signal(false);
  readonly editingWorkout = signal<Workout | null>(null);

  readonly deletingId = signal<number | null>(null);
  readonly deleteError = signal<string | null>(null);

  readonly formatDate = formatWorkoutDateTime;

  constructor() {
    this.loadWorkouts();
  }

  loadWorkouts(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.workoutService.getAll().subscribe({
      next: (workouts) => {
        this.workouts.set(workouts);
        this.loading.set(false);
      },
      error: (error: unknown) => {
        this.errorMessage.set(
          extractErrorMessage(error, 'Unable to load your workouts right now. Please try again.')
        );
        this.loading.set(false);
      }
    });
  }

  openCreateForm(): void {
    this.editingWorkout.set(null);
    this.isFormOpen.set(true);
  }

  openEditForm(workout: Workout): void {
    this.editingWorkout.set(workout);
    this.isFormOpen.set(true);
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.editingWorkout.set(null);
  }

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    if (this.isFormOpen()) {
      this.closeForm();
    }
  }

  onSaved(): void {
    this.closeForm();
    this.loadWorkouts();
  }

  deleteWorkout(workout: Workout): void {
    const confirmed = window.confirm(
      `Delete this ${workout.workoutType.toLowerCase()} workout from ${this.formatDate(workout.workoutDateUtc)}? This can't be undone.`
    );
    if (!confirmed) {
      return;
    }

    this.deletingId.set(workout.id);
    this.deleteError.set(null);

    this.workoutService.delete(workout.id).subscribe({
      next: () => {
        this.workouts.update((list) => list.filter((w) => w.id !== workout.id));
        this.deletingId.set(null);
      },
      error: (error: unknown) => {
        this.deletingId.set(null);

        // Already gone (deleted elsewhere, or no longer accessible) - just drop it locally
        // instead of showing an error for something the user asked to remove anyway.
        if (error instanceof HttpErrorResponse && error.status === 404) {
          this.workouts.update((list) => list.filter((w) => w.id !== workout.id));
          return;
        }

        this.deleteError.set(
          extractErrorMessage(error, 'Unable to delete this workout right now. Please try again.')
        );
      }
    });
  }
}
