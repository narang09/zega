import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BulkEditSidebarComponent } from './bulk-edit-sidebar.component';

describe('BulkEditSidebarComponent', () => {
  let component: BulkEditSidebarComponent;
  let fixture: ComponentFixture<BulkEditSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BulkEditSidebarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BulkEditSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
