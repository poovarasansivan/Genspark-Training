import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ContactService } from '../services/contact-service';
import { Router } from '@angular/router';
import { About } from '../about/about';
import { Toast } from '../toast/toast';

@Component({
  selector: 'app-contact',
  imports: [CommonModule, ReactiveFormsModule, Toast],
  templateUrl: './contact.html',
  styleUrl: './contact.css',
})
export class Contact {
  @ViewChild(Toast) toast!: Toast;

  constructor(private contactService: ContactService, private route: Router) {}

  contactForm = new FormGroup({
    name: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    subject: new FormControl('', Validators.required),
    message: new FormControl('', Validators.required),
  });

  get name() {
    return this.contactForm.get('name');
  }

  get email() {
    return this.contactForm.get('email');
  }

  get subject() {
    return this.contactForm.get('subject');
  }

  get message() {
    return this.contactForm.get('message');
  }

  onSubmit() {
    if (this.contactForm.invalid) {
      this.contactForm.markAllAsTouched();
      return;
    }
    const contactDetails = {
      name: this.name?.value,
      email: this.email?.value,
      subject: this.subject?.value,
      message: this.message?.value,
    };
    this.contactService.submitContactForm(contactDetails).subscribe(
      (response) => {
        this.toast.display('Contact form submitted successfully', 'success');
        console.log('Contact form submitted successfully:', response);
        this.route.navigate(['/home']);
      },
      (error) => {
        this.toast.display('Error submitting contact form', 'error');
        console.error('Error submitting contact form:', error);
      }
    );
  }
}
