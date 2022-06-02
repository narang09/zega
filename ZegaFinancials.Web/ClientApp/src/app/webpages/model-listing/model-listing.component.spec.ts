import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModelListingComponent } from './model-listing.component';

describe('ModelListingComponent', () => {
  let component: ModelListingComponent;
  let fixture: ComponentFixture<ModelListingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModelListingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModelListingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
