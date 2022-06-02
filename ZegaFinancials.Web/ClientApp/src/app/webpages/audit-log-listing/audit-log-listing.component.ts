import { Component, OnInit } from '@angular/core';
import { DataGridNames } from '../../support/enums/data-grid.enum';

@Component({
  selector: 'zega-audit-log-listing',
  templateUrl: './audit-log-listing.component.html',
  styleUrls: ['./audit-log-listing.component.less']
})
export class AuditLogListingComponent implements OnInit {

  gridName: DataGridNames = DataGridNames.AuditLogListing;
  refreshGrid: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

  refreshDataGrid() {
    this.refreshGrid = !this.refreshGrid;
  }

}
