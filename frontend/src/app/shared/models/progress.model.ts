export interface WeeklyProgress {
  weekNumber: number;
  weekStartDateUtc: string;
  weekEndDateUtc: string;
  totalDurationMinutes: number;
  workoutCount: number;
  averageIntensityLevel: number | null;
  averageFatigueLevel: number | null;
}

export interface MonthlyProgress {
  year: number;
  month: number;
  weeks: WeeklyProgress[];
}
