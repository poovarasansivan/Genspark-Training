import { Component } from '@angular/core';
import { Toast } from '../toast/toast';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../services/user-service';

@Component({
  selector: 'app-manage-tickets',
  imports: [Toast, FormsModule, CommonModule],
  templateUrl: './manage-tickets.html',
  styleUrl: './manage-tickets.css',
})
export class ManageTickets {
  contacts: any[] = [];
  filteredContacts: any[] = [];
  searchTerm: string = '';

  constructor(private http: HttpClient, private userService: UserService) {}

  ngOnInit(): void {
    this.fetchContacts();
  }

  fetchContacts() {
    this.userService.getTickets().subscribe({
      next: (data) => {
        this.contacts = data;
        this.filteredContacts = [...this.contacts];
      },
      error: (err) => {
        console.error('Error fetching contacts', err);
      },
    });
  }

  onSearchChange(term: string) {
    const search = term.trim().toLowerCase();
    this.filteredContacts = this.contacts.filter(
      (contact) =>
        contact.name.toLowerCase().includes(search) ||
        contact.email.toLowerCase().includes(search) ||
        contact.subject.toLowerCase().includes(search) ||
        contact.message.toLowerCase().includes(search)
    );
  }
}
