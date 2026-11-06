import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonthlyReport } from './monthly-report';
import { appTestProviders } from '../../../testing/test-providers';

describe('MonthlyReport', () => {
  let component: MonthlyReport;
  let fixture: ComponentFixture<MonthlyReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MonthlyReport],
      providers: appTestProviders(),
    }).compileComponents();

    fixture = TestBed.createComponent(MonthlyReport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
