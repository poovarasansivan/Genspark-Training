import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Payments } from './payments';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

describe('Payments Component', () => {
  let component: Payments;
  let fixture: ComponentFixture<Payments>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Payments]
    }).compileComponents();

    fixture = TestBed.createComponent(Payments);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    // Clean up any spy overrides
    delete (window as any).Razorpay;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should have all form controls invalid initially', () => {
    expect(component.paymentForm.invalid).toBeTrue();
  });

  it('should mark email invalid if incorrect format', () => {
    component.email?.setValue('bademail');
    expect(component.email?.invalid).toBeTrue();
    component.email?.setValue('test@example.com');
    expect(component.email?.valid).toBeTrue();
  });

  it('should not call Razorpay if form is invalid', () => {
    // Provide no valid input
    const rzpConstructorSpy = jasmine.createSpy('Razorpay');
    (window as any).Razorpay = rzpConstructorSpy;

    component.onSubmit();

    expect(rzpConstructorSpy).not.toHaveBeenCalled();
  });

  it('should call Razorpay with correct options when form is valid', () => {
    // Fill valid data
    component.name?.setValue('John Doe');
    component.email?.setValue('john@example.com');
    component.phone?.setValue('9876543210');
    component.upiId?.setValue('test@upi');
    component.amount?.setValue('500');

    const rzpOpenSpy = jasmine.createSpy('open');

    // Razorpay constructor returns an object with open()
    const rzpConstructorSpy = jasmine.createSpy('Razorpay').and.returnValue({
      open: rzpOpenSpy
    });

    (window as any).Razorpay = rzpConstructorSpy;

    component.onSubmit();

    expect(rzpConstructorSpy).toHaveBeenCalled();
    const options = rzpConstructorSpy.calls.mostRecent().args[0];
    expect(options.amount).toBe(50000);
    expect(options.prefill.upi.vpa).toBe('test@upi');

    expect(rzpOpenSpy).toHaveBeenCalled();
  });

  it('should set loading flag true when submitting', () => {
    component.name?.setValue('John Doe');
    component.email?.setValue('john@example.com');
    component.phone?.setValue('9876543210');
    component.upiId?.setValue('test@upi');
    component.amount?.setValue('500');

    const rzpOpenSpy = jasmine.createSpy('open');

    const rzpConstructorSpy = jasmine.createSpy('Razorpay').and.returnValue({
      open: rzpOpenSpy
    });

    (window as any).Razorpay = rzpConstructorSpy;

    component.onSubmit();

    expect(component.loading).toBeTrue();
  });
});
