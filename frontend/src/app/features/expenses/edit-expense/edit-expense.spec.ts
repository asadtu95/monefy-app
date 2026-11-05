import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditExpense } from './edit-expense';
import { appTestProviders } from '../../../testing/test-providers';

describe('EditExpense', () => {
  let component: EditExpense;
  let fixture: ComponentFixture<EditExpense>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditExpense],
      providers: appTestProviders(),
    }).compileComponents();

    fixture = TestBed.createComponent(EditExpense);
    component = fixture.componentInstance;

    // `id` is a required route-bound input; ngOnInit reads it.
    fixture.componentRef.setInput('id', '11111111-1111-1111-1111-111111111111');

    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
