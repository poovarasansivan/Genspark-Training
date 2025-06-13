import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-menu',
  imports: [CommonModule],
  templateUrl: './menu.html',
  styleUrl: './menu.css'
})
export class Menu {
menuOpen = false;
  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }
}
