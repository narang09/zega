import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuditLogListingComponent } from './audit-log-listing.component';

describe('AuditLogListingComponent', () => {
  let component: AuditLogListingComponent;
  let fixture: ComponentFixture<AuditLogListingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuditLogListingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AuditLogListingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
