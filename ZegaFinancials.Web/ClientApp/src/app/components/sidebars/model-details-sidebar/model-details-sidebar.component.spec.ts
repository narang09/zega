import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModelDetailsSidebarComponent } from './model-details-sidebar.component';

describe('ModelDetailsSidebarComponent', () => {
  let component: ModelDetailsSidebarComponent;
  let fixture: ComponentFixture<ModelDetailsSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModelDetailsSidebarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModelDetailsSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
