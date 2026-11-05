import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditIncome } from './edit-income';
import { appTestProviders } from '../../../testing/test-providers';

describe('EditIncome', () => {
  let component: EditIncome;
  let fixture: ComponentFixture<EditIncome>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditIncome],
      providers: appTestProviders(),
    }).compileComponents();

    fixture = TestBed.createComponent(EditIncome);
    component = fixture.componentInstance;

    // `id` is a required route-bound input; ngOnInit reads it.
    fixture.componentRef.setInput('id', '11111111-1111-1111-1111-111111111111');

    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
