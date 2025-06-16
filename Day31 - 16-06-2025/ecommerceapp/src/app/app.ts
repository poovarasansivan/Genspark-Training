import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { Navbar } from './navbar/navbar';
import { House} from 'lucide-angular';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,Navbar, RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'ecommerceapp';
  readonly HouseIcon = [House];
}

