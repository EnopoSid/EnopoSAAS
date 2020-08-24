import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreTimingsComponent } from './store-timings.component';

describe('StoreTimingsComponent', () => {
  let component: StoreTimingsComponent;
  let fixture: ComponentFixture<StoreTimingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StoreTimingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StoreTimingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
