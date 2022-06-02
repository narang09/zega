import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DxDataGridComponent } from 'devextreme-angular/ui/data-grid';
import { DxiDataGridColumn } from 'devextreme-angular/ui/nested/base/data-grid-column-dxi';
import CustomStore from 'devextreme/data/custom_store';
import { DataGridColumnModel } from '../../models/DataGrid/DataGridColumnModel/data-grid-column-model';
import { DataGridModel } from '../../models/DataGrid/DataGridModel/data-grid-model';
import { GridRowSelectionModel } from '../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { ResponseVO } from '../../models/ResponseVO/response-vo';
import { DataGridService } from '../../services/DataGridService/data-grid.service';
import { Constants } from '../../support/constants/constants';
import { DataGridNames } from '../../support/enums/data-grid.enum';
import { AdvancedFilterComponent } from '../advanced-filter/advanced-filter.component';
import { AdvancedFilterCommunicationModel } from '../../models/DataGrid/FilterExpressionModel/filter-expression-model';
import { DataGridEditModel } from '../../models/DataGrid/DataGridEditModel/data-grid-edit-model';
import { Utility } from '../../support/utility/utility';
import { DataGridRowDblClickModel } from '../../models/DataGrid/DataGridRowDBLClickModel/data-grid-row-dbl-click-model';
import { AuthenticationService } from '../../services/AuthenticationService/authentication.service';
import * as $ from 'jquery';


@Component({
  selector: 'zega-data-grid',
  templateUrl: './data-grid.component.html',
  styleUrls: ['./data-grid.component.less']
})
export class DataGridComponent implements OnInit, OnChanges, OnDestroy {
  @Input() grid: DataGridNames = DataGridNames.None;
  @Input() hasSubGrid: boolean = false;
  @Input() subGridDataIdentifier: string = '';
  @Input() subGrid: DataGridNames = DataGridNames.None;
  @Input() enableFilters: boolean = true;
  @Input() enablePagination: boolean = true;
  @Input() enableSorting: boolean = true;
  @Input() enableColResize: boolean = true;
  @Input() enableEditing: boolean = false;
  @Input() overrideDataAPI: boolean = false;
  @Input() sortingMode: string = 'none'; // "single" | "multiple" | "none" 
  @Input() gridDataAPICall: ((args: any) => Promise<any>) | undefined;
  @Input() gridDataEditCall: ((args: any) => Promise<any>) | undefined;
  @Input() apiAddnlParams: object = {};
  @Input() enableQuickSearch: boolean = true;
  @Input() gridParent: string = 'listing';
  @Input() gridCompId: string = 'defaultGridCompId';
  @Input() selectionMode: string = 'single'; //  'multiple' | 'none' | 'single'
  @Input() enableColumnChooser: boolean = true;
  @Input() requestRefresh: boolean = false;
  @Input() preSelectedRows: Array<any> = [];
  @Input() saveGridPreferences: boolean = true;
  @Input() hasAdvancedFilter: boolean = false;
  @Input() enableExportExcel: boolean = false;
  @Input() enableRowsDblClick: boolean = false;
  @Input() requireSelectionOverPages: boolean = false;
  @Input() deSelectRowsManually: Array<any> = [];

  @Output() gridHeadersLoaded: EventEmitter<DataGridNames> = new EventEmitter();
  @Output() gridRowsSelected: EventEmitter<GridRowSelectionModel> = new EventEmitter();
  @Output() gridRowsDblClicked: EventEmitter<DataGridRowDblClickModel> = new EventEmitter();
  @Output() gridDataLoaded: EventEmitter<DataGridNames> = new EventEmitter();

  gridLoadInit = false
  dataGridModel = new DataGridModel();
  dataGridCustomDataSrc: any = {};
  enableWarap: boolean = false;
  gridAllowedPageSizes: Array<number> = [25, 50, 100];
  private gridData: Array<any> = []
  private GridColumnDataFieldUIDict: any = {};
  private GridSelectedPaginatedRows: Array<any> = []
  private UserLogin: string = '';
  ngOnInit(): void {
    this.setInitialGridProps();
    if (this.grid != DataGridNames.None) 
      this.getGridHeaders();
    let login = this.authService.getLoggedInUserInfo;
    if (login)
      this.UserLogin = login.loginId;
  }

  @ViewChild(DxDataGridComponent, { static: false }) dxDataGrid!: DxDataGridComponent;

  constructor(private gridService: DataGridService, private matDialog: MatDialog, private authService: AuthenticationService) {
    this.gridLoadInit = false;
    this.dataGridCustomDataSrc = new CustomStore({
      key: "id",
      load: (loadOptions: any) => this.getRAWDataforDataGrid(loadOptions),
      update: (key: any, values: any) => this.gridDataEditCallback(key, values),
      onLoaded: (result: Array<any>) => this.gridRAWDataLoadedCallback(result)
    });
    this.dataGridModel.PaginationSize = this.gridAllowedPageSizes[0];
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.requestRefresh && !changes.requestRefresh.firstChange && changes.requestRefresh.previousValue !== changes.requestRefresh.currentValue) {
      if (this.requireSelectionOverPages && changes.preSelectedRows && !changes.preSelectedRows.firstChange && changes.preSelectedRows.previousValue !== changes.preSelectedRows.currentValue)
        this.GridSelectedPaginatedRows = this.preSelectedRows;
      this.refreshDataGrid(false);
    }
    else if (changes.deSelectRowsManually && !changes.deSelectRowsManually.firstChange && changes.deSelectRowsManually.previousValue !== changes.deSelectRowsManually.currentValue)
      this.manuallyDesectRow(changes.deSelectRowsManually.currentValue);
  }

  private manuallyDesectRow(delectedRowa: Array<any>) {
    let deSelectedKeys = delectedRowa.map((sr: any) => sr.id);
    let curentPageDeSelectedIds = this.gridData.filter(gd => deSelectedKeys.includes(gd.id)).map(sr => sr.id);
    if (curentPageDeSelectedIds.length)
      this.dxDataGrid.instance.deselectRows(curentPageDeSelectedIds);
  }

  ngOnDestroy(): void {
    if (this.saveGridPreferences) {
      var savePreferencesModel = this.getGridColumnPreferencesModel()
      if (savePreferencesModel)
        this.gridService.saveGridPreferences(savePreferencesModel).subscribe();
    }
  }

  private getRAWDataforDataGrid(loadOptions: any) {
    var gridModel = this.getDataGridModel(loadOptions);
    var promise;
    if (this.overrideDataAPI && this.gridDataAPICall != undefined)
      promise = this.gridDataAPICall(gridModel);
    else
      promise = this.gridService.getGridData(gridModel).toPromise();
    return promise.then((retData: ResponseVO) => {
      if (retData.success) {
        this.dataGridModel.TotalCount = retData.response.totalRecords;
        return {
          data: this.processDataForGridUI(retData.response),
          totalCount: retData.response.totalRecords
        };
      } else
        return {
          data: this.gridData,
          totalCount: this.dataGridModel.TotalCount
        };
    })
  }

  private gridRAWDataLoadedCallback(result: any): any {
    this.gridData = result.data;
    this.gridDataLoaded.emit(this.grid);
  }

  private gridDataEditCallback(key: any, values: any): any {
    if (this.enableEditing && this.gridDataEditCall && this.gridDataEditCall != undefined) {
      let gridEditModel: DataGridEditModel = {
        Grid: this.grid,
        EditRowKey: key,
        UpdatedValues: values
      }
      return this.gridDataEditCall(gridEditModel)
        .then((edittedRow: any) => {
          return edittedRow;
        });
    }
  }

  toggleFavourite(row: any) {
    if (this.enableEditing && this.gridDataEditCall && this.gridDataEditCall != undefined) {
      let colName: string = row.column.dataField;
      let gridEditModel: DataGridEditModel = {
        Grid: this.grid,
        EditRowKey: row.key,
        UpdatedValues: {}
      }
      gridEditModel.UpdatedValues[colName] = true;
      this.gridDataEditCall(gridEditModel);
    }
  }

  private setInitialGridProps() {
    this.dataGridModel.Grid = this.grid;
    if (this.sortingMode === 'none' && this.enableSorting)
      this.sortingMode = 'multiple';
    if (this.enablePagination)
      this.dataGridModel.PaginationSize = this.gridAllowedPageSizes[0];
    if (this.preSelectedRows.length)
      this.GridSelectedPaginatedRows = this.preSelectedRows;
    if (this.dataGridModel.Grid === DataGridNames.AuditLogListing)
      this.enableWarap = true;
  }

  private getGridHeaders() {
    this.gridService.getGridHeader(JSON.stringify(this.dataGridModel.GridName))
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          let retData = response['response'];
          this.dataGridModel.ColumnDef = retData.gridHeaders ? retData.gridHeaders.map((col: any) => new DataGridColumnModel(col)) : [];
          if (retData.sortDescriptions && retData.sortDescriptions.length)
            this.setPreSortingonGrid(retData.sortDescriptions);
          this.GridColumnDataFieldUIDict = this.prepareColumnFieldDict()
          this.gridLoadInit = true;
          this.gridHeadersLoaded.emit(this.grid);
        }
      });
  }

  private setPreSortingonGrid(sortDescriptions: Array<any>) {
    sortDescriptions.forEach((item: any, index: number) => {
      let Col = this.dataGridModel.ColumnDef.find(col => col.DataField === item.field);
      if (Col) {
        Col.SortIndex = index;
        Col.SortOrder = item.fieldDirection
      }
    })
  }

  private prepareColumnFieldDict() {
    const initialValue = {};
    return this.dataGridModel.ColumnDef.reduce((obj, col: any) => {
      return {
        ...obj,
        [col['DataFieldUI']]: col['DataField'],
      };
    }, initialValue);
  }

  private processDataForGridUI(respObject: any) {
    let rawData: Array<any> = [];
    switch (this.dataGridModel.Grid) {
      case DataGridNames.AccountListing: case DataGridNames.DashboardAdvisor: rawData = respObject['accounts']; break;
      case DataGridNames.ModelListing: case DataGridNames.ModelListingSidebar: rawData = respObject['models']; break;
      case DataGridNames.SleeveListing: rawData = respObject['sleeves']; break;
      case DataGridNames.AdvisorListing: rawData = respObject['advisors']; break;
      case DataGridNames.AuditLogListing: rawData = respObject['auditLogs']; break;
      case DataGridNames.StrategyListing: rawData = respObject['strategies']; break;
      case DataGridNames.DashboardAdmin: rawData = respObject['advisorsList']; break;
      case DataGridNames.ImportHistory: rawData = respObject['importHistory']; break;
      case DataGridNames.RepCodeListing: rawData = respObject['repCodes']; break;
      default: rawData = respObject['gridData'];
    }
    return rawData;
  }

  private getGridColumnPreferencesModel() {
    let sortDesc: Array<any> = [];
    if (this.dxDataGrid) {
      let state = this.dxDataGrid.instance.state();
      let dxCols: Array<DxiDataGridColumn> = state.columns;
      if (dxCols && dxCols.length) {
        this.dataGridModel.ColumnDef.forEach(col => {
          let dxCol = dxCols.find(c => c.dataField === col.DataFieldUI);
          if (dxCol) {
            col.IsVisible = dxCol.visible;
            col.DisplayIndex = dxCol.visibleIndex;
            if (dxCol.sortOrder) {
              sortDesc.push({ Field: col.DataField, FieldDirection: dxCol.sortOrder, Priority: dxCol.sortIndex ?? 0 });
            }
          }
        })
      }
      return {
        Grid: this.dataGridModel.Grid,
        GridName: this.dataGridModel.GridName,
        SortDescriptions: sortDesc,
        DataGridColumnObjects: this.dataGridModel.ColumnDef,
        StartIndex: this.dataGridModel.Page - 1,
        PaginationSize: this.dataGridModel.PaginationSize,
        UserLogin: this.UserLogin,
      }
    } else
      return null;
  }

  private getDataGridModel(gridRawModel: any) {
    return {
      Grid: this.dataGridModel.Grid,
      GridName: this.dataGridModel.GridName,
      DataGridColumnObjects: this.dataGridModel.ColumnDef,
      SearchText: this.dataGridModel.QuickSearch ?? '',
      IsPaginationActive: this.enablePagination,
      StartIndex: (this.dataGridModel.Page - 1) * (this.dataGridModel.PaginationSize),
      PaginationSize: this.dataGridModel.PaginationSize,
      SortDescriptions: this.getSortDescription(gridRawModel.sort),
      GridAdditionalParameters: this.apiAddnlParams
    }
  }

  private getSortDescription(sortCols: Array<any>) {
    return sortCols && sortCols.length ? sortCols.map((col, index) => { return { Field: this.GridColumnDataFieldUIDict[col.selector], FieldDirection: col.desc ? 'desc' : 'asc', Priority: index } }) : [];
  }

  contentReady(event: any) {
    this.selectPreSelectedRows();
  }

  private selectPreSelectedRows() {
    if (this.requireSelectionOverPages && this.GridSelectedPaginatedRows.length) {
      let preSelectedKeys = this.GridSelectedPaginatedRows.map((sr: any) => sr.id);
      let curentPageSelectedIds = this.gridData.filter(gd => preSelectedKeys.includes(gd.id)).map(sr => sr.id);
      if (curentPageSelectedIds.length)
        this.dxDataGrid.instance.selectRows(curentPageSelectedIds, false);
    }
  }

  dataGridPageChanged(event: any) {
    this.refreshDataGrid(false);
  }

  dataGridPageSizeChanged(event: any) {
    this.refreshDataGrid(true);
  }

  onRowDblClick(event: any) {
    if (this.enableRowsDblClick) {
      let gridDblClickModel: DataGridRowDblClickModel = {
        Grid: this.grid,
        Key: event.key,
        RowData: event.data
      }
      this.gridRowsDblClicked.emit(gridDblClickModel);
    }
  }

  onSelectionChanged(event: any) {
    let selectedRows = [];
    if (this.requireSelectionOverPages) {
      if (event.currentDeselectedRowKeys && event.currentDeselectedRowKeys.length) {
        let currentPageDelectedKeys = this.gridData.filter(gd => event.currentDeselectedRowKeys.includes(gd.id));
        currentPageDelectedKeys.forEach((uk: any) => {
          let ind = this.GridSelectedPaginatedRows.findIndex((sr: any) => sr.id == uk.id)
          if (ind > -1)
            this.GridSelectedPaginatedRows.splice(ind, 1)
        });
      }
      if (event.currentSelectedRowKeys && event.currentSelectedRowKeys.length) {
        if (this.selectionMode === 'single')
          this.GridSelectedPaginatedRows = [];
        event.currentSelectedRowKeys.forEach((sk: number) => {
          let ind = this.GridSelectedPaginatedRows.findIndex((sr: any) => sr.id == sk);
          if (ind == -1) {
            let rowData = event.selectedRowsData.find((d: any) => d.id == sk);
            if (rowData)
              this.GridSelectedPaginatedRows.push(rowData);
          }
        });
      }
      selectedRows = this.GridSelectedPaginatedRows;
    } else
      selectedRows = event.selectedRowsData;
    let gSelectModel = new GridRowSelectionModel();
    gSelectModel.Grid = this.grid;
    gSelectModel.SelectedRows = selectedRows;
    gSelectModel.Key = 'id';
    this.dataGridModel.SelectedCount = selectedRows.length;
    this.gridRowsSelected.emit(gSelectModel);
  }

  getGridHeight() {
    var height = 0;
    var pagination = this.enablePagination ? 75 : 0;
    let lh = document.getElementById(this.gridCompId)?.offsetTop
    switch (this.gridParent) {
      case 'listing':
        height = window.innerHeight - Number(lh) - pagination - 10;
        break;
      case 'halflisting':
        height = (window.innerHeight - Number(lh) - pagination - 10) / 2;
        break;
      case 'section':
      case 'subGrid':
        height = 300;
        break;
      case 'sidebar':
        height = window.innerHeight - Number(lh) - pagination - 200;
        break;
    }
    return height - 35;
  }

  private refreshDataGrid(resetPage: boolean) {
    if (resetPage)
      this.dataGridModel.Page = 1;
    this.dxDataGrid.instance.refresh();
    if (!this.requireSelectionOverPages || !this.GridSelectedPaginatedRows.length) {
      this.dxDataGrid.instance.deselectAll();
      this.dxDataGrid.instance.clearSelection();
    }
  }

  applyQuickSearch(event: any) {
    var qSrhValue = event.value ? event.value.trim() : String(document.querySelector('#dataGridQuickSearchInput' + this.dataGridModel.Grid)?.querySelector('input')?.value).trim();
    if (this.enableQuickSearch && this.dataGridModel.QuickSearch !== qSrhValue)
      this.applyQuickSearchInternal(qSrhValue);
  }

  private applyQuickSearchInternal(searchText: string) {
    this.dataGridModel.QuickSearch = searchText;
    this.refreshDataGrid(true);
  }

  onToolbarPreparing(e: any) {
    let buttonsToAdd: Array<any> = []
    if (this.hasAdvancedFilter) {
      let advFilterBtn = {
        location: 'before',
        widget: 'dxButton',
        options: {
          text: "Advanced Filter",
          onClick: this.openAdvancedFilterPopup.bind(this),
          elementAttr: {
            id: 'dataGridAdvancedFilterBtn',
            class: ''
          }
        }
      };
      buttonsToAdd.push(advFilterBtn);
    }

    if (this.enableQuickSearch) {
      let quickSearchInput = {
        location: 'before',
        widget: 'dxTextBox',
        options: {
          elementAttr: {
            id: 'dataGridQuickSearchInput' + this.dataGridModel.Grid,
            class: ''
          },
          placeholder: 'Quick Search',
          showClearButton: true,
          onEnterKey: this.applyQuickSearch.bind(this),
          onValueChanged: this.applyQuickSearch.bind(this),
        }
      };
      let quickSearchIcon = {
        location: 'before',
        widget: 'dxButton',
        options: {
          icon: 'search',
          onClick: this.applyQuickSearch.bind(this)
        }
      };
      buttonsToAdd.push(quickSearchInput, quickSearchIcon);
    }

    if (this.enableExportExcel) {
      let exportExcelBtn = {
        location: 'after',
        widget: 'dxButton',
        options: {
          icon: 'xlsfile',
          onClick: this.exportGridToExcel.bind(this),
        }
      };
      buttonsToAdd.push(exportExcelBtn);
    }

    if (this.enableColumnChooser) {
      let columnChooserBtn = {
        location: 'after',
        widget: 'dxButton',
        options: {
          icon: 'columnchooser',
          onClick: this.columnChoooserClicked.bind(this),
          elementAttr: {
            id: 'dataGridColumnVisibilityDropdownBtn' + this.dataGridModel.Grid,
            class: ''
          }
        }
      };
      buttonsToAdd.push(columnChooserBtn);
    }

    // Adding to Header bar
    if (buttonsToAdd.length)
      e.toolbarOptions.items.unshift(...buttonsToAdd);

  }

  private columnChoooserClicked(event: any) {
    let element = $('#dataGridColumnVisibilityDropdownBtn' + this.dataGridModel.Grid);
    let columnDropElement = $('.grid-column-visibility-dropdown');
    let offset = element?.offset();
    let top = offset?.top ?? 0 + 50;
    let left = (offset?.left ?? 0) - (columnDropElement?.width() ?? 0) + 25;
    columnDropElement?.css({ top: top, left: left, visibility: 'visible' });
    let shaddowDiv = document.createElement('div');
    shaddowDiv.className = 'overlay modal-backdrop';
    shaddowDiv.id = 'dataGridColumnVisibilityShaddow';
    shaddowDiv.addEventListener('click', this.closeColumnVisibilityDropodown);
    document.body.append(shaddowDiv);
  }

  closeColumnVisibilityDropodown() {
    $('.grid-column-visibility-dropdown').css({ top: 0, left: 0, visibility: 'hidden' });
    let elemment = document.querySelector('#dataGridColumnVisibilityShaddow');
    elemment?.removeEventListener('click', this.closeColumnVisibilityDropodown);
    elemment?.remove();
  }

  private advancedFilterCallback = (result: AdvancedFilterCommunicationModel) => {
    if (result && result.ApplyFilter) {
      this.dataGridModel.ColumnDef = result.GridColumns;
      if (this.dataGridModel.ColumnDef.filter(c => c.IsFilterAdded).length)
        $('#dataGridAdvancedFilterBtn').addClass('common-active-btn-color');
      else
        $('#dataGridAdvancedFilterBtn').removeClass('common-active-btn-color');
      this.refreshDataGrid(true);
    }
  }

  private openAdvancedFilterPopup() {
    let gridCols: Array<DataGridColumnModel> = Utility.deepCopy<Array<DataGridColumnModel>>(this.dataGridModel.ColumnDef);
    const dialogRef = this.matDialog.open(AdvancedFilterComponent, {
      panelClass: 'set-padding',
      width: '80%',
      data: { GridColumns: gridCols, ApplyFilter: false } // Avoiding References - Deep copy
    });

    dialogRef.afterClosed().subscribe(this.advancedFilterCallback);
  }

  private cloneGridColumns(gridCols: Array<DataGridColumnModel>) {
    return gridCols.map(col => {
      let column = new DataGridColumnModel({});
      column = col;
      return column;
    })
  }

  private exportGridToExcel() {
    if (this.enableExportExcel && this.enablePagination) {
      let gridPrefModel = this.getGridColumnPreferencesModel();
      if (gridPrefModel) {
        let exportModel = {
          ...gridPrefModel,
          SearchText: this.dataGridModel.QuickSearch ?? '',
          IsPaginationActive: false,
          GridAdditionalParameters: this.apiAddnlParams,
        };
        this.gridService.exportPaginatedGridToExcel(exportModel)
          .subscribe((response: any) => this.downLoadFile(response, response?.type));
      }
    }
  }

  private downLoadFile(data: any, type: string) {
    try {
      let filename = 'Export_' + this.dataGridModel.GridName + '.csv';
      var file = typeof File === 'function' ? new File([data], filename, { type: type }) : new Blob([data], { type: type });

      var URL = window.URL || window.webkitURL;
      var downloadUrl = URL.createObjectURL(file);

      // use HTML5 a[download] attribute to specify filename
      var a = document.createElement("a");

      if (typeof a.download === 'undefined') {  // safari doesn't support this yet
        window.open(downloadUrl)
      } else {
        a.href = downloadUrl;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
      }
      document.body.removeChild(a);   //remove the link "a"
      setTimeout(function () {   // cleanup
        URL.revokeObjectURL(downloadUrl);
      }, 100);
    } catch {
      console.error('Errro in downloading file');
    }
  }

  getSubGridData(gridModel: any) {
    return new Promise(resolveCallback => {
      let gridRows = gridModel.GridAdditionalParameters.IsSubGrid ? gridModel.GridAdditionalParameters.ParentRowData[gridModel.GridAdditionalParameters.SubGridDataIdentifier] : [];
      var data = {
        success: true,
        response: {
          totalRecords: gridRows.length ?? 0,
          gridData: gridRows ?? []
        }
      }
      resolveCallback(data);
    });
  }

  getSubGridAdditionalParams(gridRow: any) {
    return {
      IsSubGrid: this.hasSubGrid,
      SubGridDataIdentifier: this.subGridDataIdentifier,
      ParentRowData: gridRow.data,
      Key: gridRow.key,

    };
  }


}
