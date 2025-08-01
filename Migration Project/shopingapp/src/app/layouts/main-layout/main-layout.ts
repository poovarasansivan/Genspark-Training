import { Component } from '@angular/core';
import { Menus } from "../../menu/menu";
import { RouterOutlet } from '@angular/router';
import { Footer } from "../../footer/footer";

@Component({
  selector: 'app-main-layout',
  imports: [Menus, RouterOutlet, Footer],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayout {

}
