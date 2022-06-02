import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StrategyListingComponent } from './strategy-listing.component';

describe('StrategyListingComponent', () => {
  let component: StrategyListingComponent;
  let fixture: ComponentFixture<StrategyListingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StrategyListingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StrategyListingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
