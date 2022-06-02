import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SleeveListingComponent } from './sleeve-listing.component';

describe('SleeveListingComponent', () => {
  let component: SleeveListingComponent;
  let fixture: ComponentFixture<SleeveListingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SleeveListingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SleeveListingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
