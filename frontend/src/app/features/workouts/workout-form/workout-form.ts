import { Component, effect, inject, input, output, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import { Workout, WORKOUT_TYPES, WorkoutType } from '../../../shared/models/workout.model';
import {
  fromDateTimeLocalInputValue,
  toDateTimeLocalInputValue
} from '../../../shared/utils/date-time.util';
import { extractErrorMessage } from '../../../shared/utils/extract-error-message';
import { WorkoutService } from '../workout.service';

// GymTracker records completed workouts, not planned ones - mirrors the backend's
// WorkoutService.EnsureNotFutureDate check so the user sees the problem before submitting.
function notFutureDateValidator(control: AbstractControl): ValidationErrors | null {
  if (!control.value) {
    return null;
  }

  const workoutDate = new Date(fromDateTimeLocalInputValue(control.value));
  return workoutDate.getTime() > Date.now() ? { futureDate: true } : null;
}

@Component({
  selector: 'app-workout-form',
  imports: [ReactiveFormsModule],
  templateUrl: './workout-form.html',
  styleUrl: './workout-form.scss'
})
export class WorkoutForm {
  private readonly fb = inject(FormBuilder);
  private readonly workoutService = inject(WorkoutService);

  readonly workout = input<Workout | null>(null);
  readonly saved = output<void>();
  readonly cancelled = output<void>();

  readonly workoutTypes = WORKOUT_TYPES;
  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    workoutType: this.fb.nonNullable.control<WorkoutType>('Cardio', [Validators.required]),
    workoutDateUtc: ['', [Validators.required, notFutureDateValidator]],
    durationMinutes: [30, [Validators.required, Validators.min(1)]],
    caloriesBurned: this.fb.control<number | null>(null, [Validators.min(0)]),
    intensityLevel: [5, [Validators.required, Validators.min(1), Validators.max(10)]],
    fatigueLevel: [5, [Validators.required, Validators.min(1), Validators.max(10)]],
    notes: this.fb.control<string | null>(null, [Validators.maxLength(1000)])
  });

  constructor() {
    effect(() => {
      const current = this.workout();
      this.errorMessage.set(null);

      if (current) {
        this.form.setValue({
          workoutType: current.workoutType,
          workoutDateUtc: toDateTimeLocalInputValue(current.workoutDateUtc),
          durationMinutes: current.durationMinutes,
          caloriesBurned: current.caloriesBurned,
          intensityLevel: current.intensityLevel,
          fatigueLevel: current.fatigueLevel,
          notes: current.notes
        });
      } else {
        this.form.reset({
          workoutType: 'Cardio',
          workoutDateUtc: '',
          durationMinutes: 30,
          caloriesBurned: null,
          intensityLevel: 5,
          fatigueLevel: 5,
          notes: null
        });
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const payload = {
      workoutType: raw.workoutType,
      durationMinutes: raw.durationMinutes,
      caloriesBurned: raw.caloriesBurned,
      intensityLevel: raw.intensityLevel,
      fatigueLevel: raw.fatigueLevel,
      notes: raw.notes,
      workoutDateUtc: fromDateTimeLocalInputValue(raw.workoutDateUtc)
    };

    this.submitting.set(true);
    this.errorMessage.set(null);

    const editing = this.workout();
    const request$ = editing
      ? this.workoutService.update(editing.id, payload)
      : this.workoutService.create(payload);

    request$.subscribe({
      next: () => {
        this.submitting.set(false);
        this.saved.emit();
      },
      error: (error: unknown) => {
        this.submitting.set(false);
        this.errorMessage.set(
          extractErrorMessage(error, 'Unable to save this workout right now. Please try again.')
        );
      }
    });
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
