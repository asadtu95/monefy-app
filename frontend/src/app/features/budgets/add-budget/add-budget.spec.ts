import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddBudget } from './add-budget';

describe('AddBudget', () => {
  let component: AddBudget;
  let fixture: ComponentFixture<AddBudget>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddBudget],
    }).compileComponents();

    fixture = TestBed.createComponent(AddBudget);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
