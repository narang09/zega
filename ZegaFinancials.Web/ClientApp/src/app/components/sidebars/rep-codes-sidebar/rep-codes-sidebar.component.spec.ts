import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RepCodesSidebarComponent } from './rep-codes-sidebar.component';

describe('RepCodesSidebarComponent', () => {
  let component: RepCodesSidebarComponent;
  let fixture: ComponentFixture<RepCodesSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RepCodesSidebarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RepCodesSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
