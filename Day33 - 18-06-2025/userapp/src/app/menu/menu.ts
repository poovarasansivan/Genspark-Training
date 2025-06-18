import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LucideAngularModule, Menu, X, Home, Info,ShoppingCart, ShieldUser } from 'lucide-angular';

@Component({
  selector: 'app-menu',
  imports: [CommonModule, RouterLink, LucideAngularModule],
  templateUrl: './menu.html',
  styleUrl: './menu.css'
})

export class Header {
  userIcon = ShieldUser;
}