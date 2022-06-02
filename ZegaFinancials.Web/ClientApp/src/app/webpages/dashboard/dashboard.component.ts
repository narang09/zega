import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DataGridRowDblClickModel } from '../../models/DataGrid/DataGridRowDBLClickModel/data-grid-row-dbl-click-model';
import { GridRowSelectionModel } from '../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { AuthenticationService } from '../../services/AuthenticationService/authentication.service';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { Constants } from '../../support/constants/constants';
import { DataGridNames } from '../../support/enums/data-grid.enum';
import { Utility } from '../../support/utility/utility';

@Component({
  selector: 'zega-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less']
})
export class DashboardComponent implements OnInit {

  gridName1: DataGridNames = DataGridNames.None;
  gridName2: DataGridNames = DataGridNames.None;
  refreshGrid: boolean = false;
  gridParent: string = '';
  IsAdmin: boolean = false;
  selectedAccount: number = 0;

  constructor(private router: Router, private authService: AuthenticationService, private route: ActivatedRoute, private messageService: AuthMessageService) { }

  ngOnInit(): void {
    let login = this.authService.getLoggedInUserInfo;
    if (login)
      this.IsAdmin = login.isAdmin;

    this.gridName1 = this.IsAdmin ? DataGridNames.DashboardAdmin : DataGridNames.DashboardAdvisor;
    this.gridName2 = this.IsAdmin ? DataGridNames.ImportHistory : DataGridNames.None;
    this.gridParent = this.IsAdmin ? 'halflisting' : 'listing';
  }


  editAccount() {
    if (this.IsAdmin)
      this.router.navigate(['/accounts']);
    else
      this.navigateToAccount(this.selectedAccount);
  }

  refreshDataGrids() {
    this.refreshGrid = !this.refreshGrid;
  }

  gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName1 && !this.IsAdmin) 
      this.selectedAccount = gridSelect.SelectedKeys.length ? gridSelect.SelectedKeys[0] : 0;
  }

  gridRowsDblClickedCallback(gridRowDblClk: DataGridRowDblClickModel) {
    if (gridRowDblClk.Grid == this.gridName1 && gridRowDblClk.Key)
      this.navigateToAccount(gridRowDblClk.Key);
  }

  public addAccount() {
    if (this.IsAdmin)
      this.navigateToAccount(0);
    else
      this.messageService.showErrorPopup({ message: Constants.AdminOnlyPermissionMsg });
  }

  private navigateToAccount(accountId: number) {
    let encrptedId = Utility.compressURL(accountId.toString());
    this.router.navigate(['/accounts', encrptedId], { relativeTo: this.route });
  }
}
