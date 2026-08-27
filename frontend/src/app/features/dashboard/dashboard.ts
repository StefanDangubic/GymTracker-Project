import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { Workout } from '../../shared/models/workout.model';
import { formatDuration, formatWorkoutDateTime } from '../../shared/utils/date-time.util';
import { WorkoutService } from '../workouts/workout.service';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard {
  private readonly authService = inject(AuthService);
  private readonly workoutService = inject(WorkoutService);
  private readonly now = new Date();

  readonly user = this.authService.user;
  readonly formatDate = formatWorkoutDateTime;

  readonly currentPeriodLabel = new Intl.DateTimeFormat('en-US', {
    month: 'long',
    year: 'numeric'
  }).format(this.now);

  readonly workouts = signal<Workout[]>([]);
  readonly loading = signal(true);

  // Reuses the existing GET /api/workouts response already fetched for the Workouts
  // page - no dedicated stats endpoint, just client-side aggregation over the same data.
  private readonly thisMonthWorkouts = computed(() =>
    this.workouts().filter((w) => this.isInCurrentMonth(w.workoutDateUtc))
  );

  readonly workoutCountThisMonth = computed(() => this.thisMonthWorkouts().length);

  readonly formattedDurationThisMonth = computed(() => {
    const totalMinutes = this.thisMonthWorkouts().reduce((sum, w) => sum + w.durationMinutes, 0);
    return formatDuration(totalMinutes);
  });

  readonly caloriesThisMonth = computed(() =>
    this.thisMonthWorkouts().reduce((sum, w) => sum + (w.caloriesBurned ?? 0), 0)
  );

  // Workouts are already returned most-recent-first by the API, so the first two
  // entries are exactly the most recent activity - no extra sorting needed.
  readonly recentWorkouts = computed(() => this.workouts().slice(0, 2));

  constructor() {
    this.workoutService.getAll().subscribe({
      next: (workouts) => {
        this.workouts.set(workouts);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  initials(fullName: string): string {
    return fullName
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((part) => part[0]?.toUpperCase())
      .join('');
  }

  private isInCurrentMonth(workoutDateUtc: string): boolean {
    const date = new Date(workoutDateUtc);
    return (
      date.getFullYear() === this.now.getFullYear() && date.getMonth() === this.now.getMonth()
    );
  }
}
