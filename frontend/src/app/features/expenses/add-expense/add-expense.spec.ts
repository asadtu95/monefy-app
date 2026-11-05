import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddExpense } from './add-expense';
import { appTestProviders } from '../../../testing/test-providers';

describe('AddExpense', () => {
  let component: AddExpense;
  let fixture: ComponentFixture<AddExpense>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddExpense],
      providers: appTestProviders(),
    }).compileComponents();

    fixture = TestBed.createComponent(AddExpense);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
