import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddGoal } from './add-goal';
import { appTestProviders } from '../../../testing/test-providers';

describe('AddGoal', () => {
  let component: AddGoal;
  let fixture: ComponentFixture<AddGoal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddGoal],
      providers: appTestProviders(),
    }).compileComponents();

    fixture = TestBed.createComponent(AddGoal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
