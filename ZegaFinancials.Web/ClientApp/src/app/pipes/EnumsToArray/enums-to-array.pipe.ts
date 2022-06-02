import { Pipe, PipeTransform } from '@angular/core';
import { DropdownListModel } from '../../models/DataGrid/DataGridColumnModel/data-grid-column-model';

@Pipe({
  name: 'enumsToArray'
})
export class EnumsToArrayPipe implements PipeTransform {

  transform(value: any, ...args: unknown[]): Array<DropdownListModel> {
    return Object.keys(value).filter(e => !isNaN(+e)).map(o => { return { Value: +o, Key: this.convertToTitlecase(value[o]) } });
  }

  private convertToTitlecase(text: string) {
    return text.replace(/([A-Z])/g, " $1").trim();
  }

}
