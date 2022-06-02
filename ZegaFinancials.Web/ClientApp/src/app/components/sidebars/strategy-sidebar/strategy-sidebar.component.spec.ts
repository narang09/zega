import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StrategySidebarComponent } from './strategy-sidebar.component';

describe('StrategySidebarComponent', () => {
  let component: StrategySidebarComponent;
  let fixture: ComponentFixture<StrategySidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StrategySidebarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StrategySidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
