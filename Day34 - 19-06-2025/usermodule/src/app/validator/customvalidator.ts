import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function bannedWordsValidator(bannedWords: string[]): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const hasBanned = bannedWords.some((word) =>
      control.value.toLowerCase().includes(word)
    );
    return hasBanned ? { bannedWords: true } : null;
  };
}

export function passwordMatchValidator(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const pass = group.get('password')?.value;
    const confirmPass = group.get('confirmPassword')?.value;
    return pass === confirmPass ? null : { notMatching: true };
  };
}

export function emailPatternValidator(
  pattern: RegExp = /^[a-zA-Z0-9._%+-]+@(gmail|outlook)\.com$/
): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const email = control.value;
    if (!email) return null;
    return pattern.test(email) ? null : { emailPatternValidator: true };
  };
}
