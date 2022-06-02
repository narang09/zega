import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RepCodesListingSidebarComponent } from './rep-codes-listing-sidebar.component';

describe('RepCodesListingSidebarComponent', () => {
  let component: RepCodesListingSidebarComponent;
  let fixture: ComponentFixture<RepCodesListingSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RepCodesListingSidebarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RepCodesListingSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
