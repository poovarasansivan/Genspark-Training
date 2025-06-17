import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LucideAngularModule, Menu, X, Home, Info,ShoppingCart } from 'lucide-angular';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterLink, LucideAngularModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  menuIcon = Menu;
  closeIcon = X;
  homeIcon = Home;
  aboutIcon = Info;
  shoppingCartIcon = ShoppingCart;

  menuOpen = false;
  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }
}
