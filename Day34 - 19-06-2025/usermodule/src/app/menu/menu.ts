import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LucideAngularModule, Menu, X, Home, PlusCircle, UserIcon } from 'lucide-angular';

@Component({
  selector: 'app-menu',
  imports: [CommonModule, RouterLink, LucideAngularModule],
  templateUrl: './menu.html',
  styleUrl: './menu.css'
})
export class Menus {
  menuIcon = Menu;
  closeIcon = X;
  homeIcon = Home;
  plusIcon = PlusCircle;
  UserIcon = UserIcon;

  menuOpen = false;

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }
}
