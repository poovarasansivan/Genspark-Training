import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Firstc } from './firstc';

describe('Firstc', () => {
  let component: Firstc;
  let fixture: ComponentFixture<Firstc>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Firstc]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Firstc);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
