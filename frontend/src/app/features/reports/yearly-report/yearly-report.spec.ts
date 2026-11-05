import { ComponentFixture, TestBed } from '@angular/core/testing';

import { YearlyReport } from './yearly-report';

describe('YearlyReport', () => {
  let component: YearlyReport;
  let fixture: ComponentFixture<YearlyReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [YearlyReport],
    }).compileComponents();

    fixture = TestBed.createComponent(YearlyReport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
