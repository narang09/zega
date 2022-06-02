import * as clone from 'clone';
import * as JSLZString from 'lz-string';
import * as $ from 'jquery';
export class Utility {

  static convertToCamelCase(str: string) {
    return str.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, function (match, index) {
      if (+match === 0) return ""; // or if (/\s+/.test(match)) for white spaces
      return index === 0 ? match.toLowerCase() : match.toUpperCase();
    });
  }

  static compressData(data: string) {
    return JSLZString.compress(data);
  }

  static decompressData(compressed: string) {
    return JSLZString.decompress(compressed);
  }

  static compressURL(data: string) {
    return JSLZString.compressToEncodedURIComponent(data);
  }

  static decompressURL(compressed: string) {
    return JSLZString.decompressFromEncodedURIComponent(compressed);
  }

  static convertEnumToArray(enumable: any) {
    var array: Array<any> = [];
    Object.entries(enumable).forEach(([key, val]) => array.push({ Key: key, Value: val }));
    return array;
  }

  static getRandomIdentifier() {
    return '_' + Math.random().toString(36).substr(2, 9);
  }

  static deepCopy<T>(source: T): T {
    return clone<T>(source);
  }

  static setSidebarContentHeight(adjustment: number = 0) {
    let hh = $('.common-sidebar-header').outerHeight() ?? 0;
    let fh = $('.common-sidebar-action-bar').outerHeight() ?? 0;
    let h = window.innerHeight - hh - fh - 35 - adjustment;
    $('.common-sidebar-content').css({ 'max-height': h, 'height': h });
  }
  static GetTodaysDate() {
    var date: any = new Date();
    var toDate: any = date.getDate();
    if (toDate < 10) {
      toDate = '0' + toDate;
    }
    var month: any = date.getMonth() + 1;
    if (month < 10) {
      month = '0' + month;
    }
    var year = date.getFullYear();
    var minDate = year + "-" + month + "-" + toDate;
    return minDate;
  }
  static cleanForm(formGroup: any) {
    return Object.keys(formGroup.controls).forEach((key) => { if (typeof formGroup.get(key).value == "string") { formGroup.get(key)!.setValue(formGroup.get(key)!.value.trim()) } else { formGroup.get(key)!.setValue(formGroup.get(key)!.value)}});
  }
}

