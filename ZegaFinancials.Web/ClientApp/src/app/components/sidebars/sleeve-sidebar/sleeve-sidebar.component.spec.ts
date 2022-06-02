import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SleeveSidebarComponent } from './sleeve-sidebar.component';

describe('SleeveSidebarComponent', () => {
  let component: SleeveSidebarComponent;
  let fixture: ComponentFixture<SleeveSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SleeveSidebarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SleeveSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
