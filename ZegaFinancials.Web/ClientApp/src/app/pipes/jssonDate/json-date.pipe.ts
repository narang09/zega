import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'jsonDate'
})
export class JsonDatePipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    return null;
  }

}
