export type WorkoutType = 'Cardio' | 'Strength' | 'Flexibility';

export const WORKOUT_TYPES: WorkoutType[] = ['Cardio', 'Strength', 'Flexibility'];

export interface Workout {
  id: number;
  workoutType: WorkoutType;
  durationMinutes: number;
  caloriesBurned: number | null;
  intensityLevel: number;
  fatigueLevel: number;
  notes: string | null;
  workoutDateUtc: string;
}

export interface WorkoutRequest {
  workoutType: WorkoutType;
  durationMinutes: number;
  caloriesBurned: number | null;
  intensityLevel: number;
  fatigueLevel: number;
  notes: string | null;
  workoutDateUtc: string;
}
