import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConsumerLayoutComponent } from './consumer-layout.component';

describe('ConsumerLayoutComponent', () => {
  let component: ConsumerLayoutComponent;
  let fixture: ComponentFixture<ConsumerLayoutComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConsumerLayoutComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConsumerLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
