import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCategory } from './edit-category';
import { appTestProviders } from '../../../testing/test-providers';

describe('EditCategory', () => {
  let component: EditCategory;
  let fixture: ComponentFixture<EditCategory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditCategory],
      providers: appTestProviders(),
    }).compileComponents();

    fixture = TestBed.createComponent(EditCategory);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('id', 'cat-1');
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
