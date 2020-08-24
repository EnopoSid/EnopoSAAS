import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CmsLandingPageComponent } from './cms-landing-page.component';

describe('CmsLandingPageComponent', () => {
  let component: CmsLandingPageComponent;
  let fixture: ComponentFixture<CmsLandingPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CmsLandingPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CmsLandingPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
