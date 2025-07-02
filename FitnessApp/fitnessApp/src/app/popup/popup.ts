import { CommonModule } from '@angular/common';
import { Component, Inject, Injectable } from '@angular/core';

@Component({
  selector: 'app-popup',
  imports: [CommonModule],
  templateUrl: './popup.html',
  styleUrl: './popup.css'
})

export class Popup {
  show = false;
  message = '';
  type: 'success' | 'error' | 'info' = 'info';

  display(message: string, type: 'success' | 'error' | 'info' = 'info') {
    this.message = message;
    this.type = type;
    this.show = true;

    setTimeout(() => {
      this.show = false;
    }, 3000);
  }
}


