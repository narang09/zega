import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModelListingSidebarComponent } from './model-listing-sidebar.component';

describe('ModelListingSidebarComponent', () => {
  let component: ModelListingSidebarComponent;
  let fixture: ComponentFixture<ModelListingSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModelListingSidebarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModelListingSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
