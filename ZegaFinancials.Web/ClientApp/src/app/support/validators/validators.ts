import { AbstractControl, FormGroup, ValidatorFn, Validators } from "@angular/forms";


export class ZegaValidators {
  static forbiddenNameValidator(nameRe: RegExp): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const forbidden = nameRe.test(control.value);
      return forbidden ? { forbiddenName: { value: control.value } } : null;
    };
  }


  static matchConfirmPassword(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];

      // return if another validator has already found an error on the matchingControl
      if (matchingControl.errors && !matchingControl.errors.mustMatch)
        return;

      // set error on matchingControl if validation fails
      if (control.value !== matchingControl.value)
        matchingControl.setErrors({ mustMatch: true });
      else
        matchingControl.setErrors(null);

    }
  }
  static notMatched(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];

      // return if another validator has already found an error on the matchingControl
      if (matchingControl.errors && !matchingControl.errors.Match)
        return;

      // set error on matchingControl if validation fails
      if (control.value !== matchingControl.value)
        matchingControl.setErrors(null);

      else
        matchingControl.setErrors({ Match: true });

    }
  }
  static conditionallyRequiredValidator(masterControlLabel: string, conditionalValue: any, slaveControlLabel: string) {
    return (formGroup: FormGroup) => {
      const masterControl = formGroup.controls[masterControlLabel];
      const slaveControl = formGroup.controls[slaveControlLabel];
      if (masterControl.value === conditionalValue && Validators.required(slaveControl))
        slaveControl.setErrors({ conditionalRequired: true });

      else
        slaveControl.setErrors(null);
    }
  }

  static conditionallyRequiredDateMinToday(masterControlLabel: string, conditionalValue: any, slaveControlLabel: string, minDate: string) {
    return (formGroup: FormGroup) => {
      const masterControl = formGroup.controls[masterControlLabel];
      const slaveControl = formGroup.controls[slaveControlLabel];
      if (masterControl.value === conditionalValue && (new Date(slaveControl.value).valueOf() < new Date(minDate).valueOf()))
        slaveControl.setErrors({ Match: true });
      else
        slaveControl.setErrors(null);
    }
  }

}
