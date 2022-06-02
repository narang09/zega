import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialogModule } from '@angular/material/dialog';
import { MatChipsModule } from '@angular/material/chips';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { DxDataGridModule } from 'devextreme-angular/ui/data-grid';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDividerModule } from '@angular/material/divider';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTabsModule } from '@angular/material/tabs';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
  ],
  exports: [
    DxDataGridModule,
    MatListModule,
    MatSidenavModule,
    MatSelectModule,
    MatDialogModule,
    MatChipsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatSidenavModule,
    MatDividerModule,
    MatCardModule,
    MatListModule,
    MatButtonModule,
    MatIconModule,
    MatStepperModule,
    MatDialogModule,
    MatTabsModule,
    MatAutocompleteModule
  ],
  providers: [
    MatFormFieldModule,
    MatInputModule,
  ]


})
export class ZegaComponentsModule { }
