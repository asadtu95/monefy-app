import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddIncome } from './add-income';
import { appTestProviders } from '../../../testing/test-providers';

describe('AddIncome', () => {
  let component: AddIncome;
  let fixture: ComponentFixture<AddIncome>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddIncome],
      providers: appTestProviders(),
    }).compileComponents();

    fixture = TestBed.createComponent(AddIncome);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
