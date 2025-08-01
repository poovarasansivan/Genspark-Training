import { AbstractControl, ValidationErrors } from "@angular/forms";

export function futureDateValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;

  if (!value) return null; 

  const inputDate = new Date(value);
  if (isNaN(inputDate.getTime())) return { invalidDate: true };

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  return inputDate >= today ? null : { pastDate: true };
}
