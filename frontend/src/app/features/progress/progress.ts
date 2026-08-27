import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MonthlyProgress } from '../../shared/models/progress.model';
import { formatDate } from '../../shared/utils/date-time.util';
import { extractErrorMessage } from '../../shared/utils/extract-error-message';
import { ProgressService } from './progress.service';

const MONTH_NAMES = [
  'January',
  'February',
  'March',
  'April',
  'May',
  'June',
  'July',
  'August',
  'September',
  'October',
  'November',
  'December'
];

@Component({
  selector: 'app-progress',
  imports: [ReactiveFormsModule],
  templateUrl: './progress.html',
  styleUrl: './progress.scss'
})
export class Progress {
  private readonly fb = inject(FormBuilder);
  private readonly progressService = inject(ProgressService);
  private readonly now = new Date();

  readonly currentYear = this.now.getFullYear();
  private readonly currentMonth = this.now.getMonth() + 1;

  readonly months = MONTH_NAMES.map((name, index) => ({ value: index + 1, name }));
  readonly formatDate = formatDate;

  readonly form = this.fb.nonNullable.group({
    year: this.fb.nonNullable.control(this.currentYear, [
      Validators.required,
      Validators.min(2000),
      Validators.max(this.currentYear)
    ]),
    month: this.fb.nonNullable.control(this.currentMonth, [Validators.required])
  });

  // Selecting a future year is blocked by the Max validator above; selecting a future
  // month within the current year is blocked harder, by simply not offering those
  // options in the dropdown (avoidable at the source rather than validated after the fact).
  private readonly selectedYear = signal(this.currentYear);
  readonly availableMonths = computed(() => {
    const maxMonth = this.selectedYear() === this.currentYear ? this.currentMonth : 12;
    return this.months.filter((m) => m.value <= maxMonth);
  });

  readonly progress = signal<MonthlyProgress | null>(null);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  monthName(monthNumber: number): string {
    return MONTH_NAMES[monthNumber - 1];
  }

  constructor() {
    this.form.controls.year.valueChanges.subscribe((year) => {
      this.selectedYear.set(year);

      const maxMonth = year === this.currentYear ? this.currentMonth : 12;
      if (this.form.controls.month.value > maxMonth) {
        this.form.controls.month.setValue(maxMonth);
      }
    });

    this.loadProgress();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loadProgress();
  }

  loadProgress(): void {
    const { year, month } = this.form.getRawValue();

    this.loading.set(true);
    this.errorMessage.set(null);

    this.progressService.getMonthly(year, month).subscribe({
      next: (progress) => {
        this.progress.set(progress);
        this.loading.set(false);
      },
      error: (error: unknown) => {
        this.errorMessage.set(
          extractErrorMessage(error, 'Unable to load progress right now. Please try again.')
        );
        this.loading.set(false);
      }
    });
  }
}
