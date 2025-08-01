import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  LucideAngularModule,
  Menu,
  X,
  ShoppingCart,
  CircleUser,
  Dumbbell,
  CircleX,
  AlignJustify,
  LogOut,
} from 'lucide-angular';
import { TokenService } from '../services/token-service';

@Component({
  selector: 'app-menu',
  imports: [CommonModule, RouterLink, LucideAngularModule],
  templateUrl: './menu.html',
  styleUrl: './menu.css',
})
export class Menus {
  menuIcon = Menu;
  closeIcon = CircleX;
  openIcon = AlignJustify;
  circleUserIcon = CircleUser;
  UserIcon = ShoppingCart;
  logoutIcon = LogOut;

  menuOpen = false;
  showWorkoutMenu = false;
  dropdownOpen = false;
  avatarDropdownOpen = false;
  dropdownTimeout: any;

  @ViewChild('avatarMenuRef') avatarMenuRef!: ElementRef;

  constructor(private router: Router, private tokenService: TokenService) {}

  loggedInUserRole: string = '';
  loggedInUser: string = '';

  ngOnInit() {
    this.loggedInUser = this.tokenService.getUserName()||'';
  }

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  toggleWorkoutMenu(): void {
    this.showWorkoutMenu = !this.showWorkoutMenu;
  }

  toggleAvatarDropdown(): void {
    this.avatarDropdownOpen = !this.avatarDropdownOpen;
  }

  closeAvatarDropdown(): void {
    this.avatarDropdownOpen = false;
  }

  onMouseLeaveDropdownArea(): void {
    this.scheduleDropdownClose();
  }

  showDropdown(): void {
    this.dropdownOpen = true;
    clearTimeout(this.dropdownTimeout);
  }

  scheduleDropdownClose(): void {
    this.dropdownTimeout = setTimeout(() => {
      this.dropdownOpen = false;
    }, 300);
  }

  cancelCloseTimeout(): void {
    clearTimeout(this.dropdownTimeout);
  }

  closeDropdown(): void {
    this.dropdownOpen = false;
  }

  logout(): void {
    localStorage.clear();
    this.router.navigate(['/login']);
  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: MouseEvent): void {
    if (
      this.avatarMenuRef &&
      !this.avatarMenuRef.nativeElement.contains(event.target)
    ) {
      this.avatarDropdownOpen = false;
    }
  }
}
