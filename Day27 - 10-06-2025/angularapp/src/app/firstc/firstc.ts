import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-firstc',
  imports: [FormsModule],
  templateUrl: './firstc.html',
  styleUrl: './firstc.css'
})
export class Firstc {

name : string
className: string = 'bi bi-balloon-heart';
toggle: boolean = false;
imageIconName:string ='';
imageltoggle:boolean = false;

  constructor() {
    this.name = 'Angular Training';
  }
  onButtonClick(newname: string) { 
    alert('Button clicked! by '+ newname);
  }
  onLike() {
  this.toggle = !this.toggle;
  if (this.toggle)
    this.className = "bi bi-balloon-heart";
  else
    this.className="bi bi-balloon-heart-fill"
  }

  
}
