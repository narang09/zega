import { Pipe, PipeTransform } from '@angular/core';
import { DropdownListModel } from '../../models/DataGrid/DataGridColumnModel/data-grid-column-model';

@Pipe({
  name: 'backendEnumToArray'
})
export class BackendEnumToArrayPipe implements PipeTransform {

  transform(value: any, ...args: unknown[]): Array<DropdownListModel> {
    return Object.entries(value).map(([key, val]: [string, any]) => { return { Value: val, Key: this.convertToTitlecase(key) } });
  }

  private convertToTitlecase(text: string) {
    return text.replace(/([A-Z])/g, " $1").trim();
  }

}
