import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportProfilesComponent } from './import-profiles.component';

describe('ImportProfilesComponent', () => {
  let component: ImportProfilesComponent;
  let fixture: ComponentFixture<ImportProfilesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImportProfilesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportProfilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
