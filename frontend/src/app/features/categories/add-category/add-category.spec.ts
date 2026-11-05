import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCategory } from './add-category';
import { appTestProviders } from '../../../testing/test-providers';

describe('AddCategory', () => {
  let component: AddCategory;
  let fixture: ComponentFixture<AddCategory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddCategory],
      providers: appTestProviders(),
    }).compileComponents();

    fixture = TestBed.createComponent(AddCategory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
