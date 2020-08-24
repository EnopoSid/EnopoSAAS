import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IngradientCountReportComponent } from './IngradientCountReport.component';

describe('IngradientCountReportComponent', () => {
  let component: IngradientCountReportComponent;
  let fixture: ComponentFixture<IngradientCountReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IngradientCountReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IngradientCountReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
