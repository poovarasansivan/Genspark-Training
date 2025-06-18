import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserModel } from '../models/usermodel';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Notification } from '../notification/notification';

@Component({
  selector: 'app-form',
  imports: [CommonModule, FormsModule, Notification],
  templateUrl: './form.html',
  styleUrl: './form.css',
})
export class Form {
  successMessage = '';
  errorMessage = '';
  showSuccess = false;
  showError = false;

  user: UserModel = {
    id: 0,
    image: '',
    firstName: '',
    lastName: '',
    age: 0,
    gender: '',
    email: '',
    phone: '',
    username: '',
    password: '',
    address: { state: '' },
    role: '',
  };

  constructor(private http: HttpClient, private route: Router) {}

  addUser(
    fn: any,
    ln: any,
    age: any,
    gender: any,
    email: any,
    phone: any,
    user: any,
    passwod: any,
    state: any,
    role: any
  ): void {
    console.log(fn.control.touched);

    if (
      fn.control.errors || ln.control.errors || age.control.errors || gender.control.errors || email.control.errors || phone.control.errors || user.control.errors || passwod.control.errors ||
      state.control.errors || role.control.errors) {
      this.successMessage = '';
      this.showSuccess = false;
      this.showError = true;
      this.errorMessage = 'Please fill all required fields correctly.';
      return;
    }

    const payload = {
      firstName: this.user.firstName,
      lastName: this.user.lastName,
      maidenName: 'maiden',
      age: this.user.age,
      gender: this.user.gender,
      email: this.user.email,
      phone: this.user.phone,
      username: this.user.username,
      password: this.user.password,
      image: 'https://dummyjson.com/icon/emilys/128',
      bloodGroup: 'A+',
      height: 143.6,
      Weight: 66,
      eyeColor: 'blue',
      hair: {
        color: 'black',
        type: 'curly',
      },
      ip: '42.48.100.32',
      address: {
        address: '626 Main Street',
        city: 'Phoenix',
        state: this.user.address.state,
        stateCode: 'MS',
        postalCode: '29112',
        coordinates: {
          lat: -77.16213,
          lng: -92.084824,
        },
        country: 'United States',
      },
      macAddress: '47:fa:41:18:ec:eb',
      university: 'University of Wisconsin--Madison',
      bank: {
        cardExpire: '03/26',
        cardNumber: '9289760655481815',
        cardType: 'Elo',
        currency: 'CNY',
        iban: 'YPUXISOBI7TTHPK2BR3HAIXL',
      },
      company: {
        department: 'Engineering',
        name: 'Dooley, Kozey and Cronin',
        title: 'Sales Manager',
        address: {
          address: '263 Tenth Street',
          city: 'San Francisco',
          state: 'Wisconsin',
          stateCode: 'WI',
          postalCode: '37657',
          coordinates: {
            lat: 71.814525,
            lng: -161.150263,
          },
          country: 'United States',
        },
      },
      ein: '977-175',
      ssn: '900-590-289',
      userAgent:
        'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.93 Safari/537.36',
      crypto: {
        coin: 'Bitcoin',
        wallet: '0xb9fc2fe63b2a6c003f1c324c3bfa53259162181a',
        network: 'Ethereum (ERC20)',
      },
      role: this.user.role || 'user',
    };

    this.http.post('https://dummyjson.com/users/add', payload).subscribe({
      next: (res) => {
        localStorage.setItem('user', JSON.stringify(res));
        console.log(res);
        this.successMessage = 'User added successfully!';
        this.showSuccess = true;
        this.showError = false;
        setTimeout(() => {
          this.route.navigateByUrl('/usercard');
        }, 3000);
      },

      error: (err) => {
        console.log(err);
        this.successMessage = 'Failed to add user. Please try again later.';
        this.showSuccess = false;
        this.showError = true;
      },
    });
  }
}
