import { HttpErrorResponse } from '@angular/common/http';
import { ProblemDetails } from '../models/problem-details.model';

// Maps the API's ProblemDetails / ValidationProblemDetails payloads to a single
// user-safe message. Anything unrecognized (network failure, 500, etc.) falls
// back to a generic message so raw technical detail never reaches the UI.
export function extractErrorMessage(error: unknown, fallback: string): string {
  if (error instanceof HttpErrorResponse) {
    const body = error.error as ProblemDetails | null;

    const firstValidationMessage = body?.errors && Object.values(body.errors)[0]?.[0];
    if (firstValidationMessage) {
      return firstValidationMessage;
    }

    if (body?.detail) {
      return body.detail;
    }
  }

  return fallback;
}
