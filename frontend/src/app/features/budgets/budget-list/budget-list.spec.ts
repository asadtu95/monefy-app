import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BudgetList } from './budget-list';
import { appTestProviders } from '../../../testing/test-providers';

describe('BudgetList', () => {
  let component: BudgetList;
  let fixture: ComponentFixture<BudgetList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BudgetList],
      providers: appTestProviders(),
    }).compileComponents();

    fixture = TestBed.createComponent(BudgetList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
