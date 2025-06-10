import { Component } from '@angular/core';
import { Firstc } from "./firstc/firstc";
import { Customer } from "./customer/customer";
import { Products } from "./products/products";

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [Customer, Products]
})
export class App {
  protected title = 'angularapp';
}
