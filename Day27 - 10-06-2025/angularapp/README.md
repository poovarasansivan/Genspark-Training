# Angular Basic

- To create an angular app use below command.
  `ng new <appname> `

- To create a new component use below command.
  `ng g c <componentname>`

# Component Structure

`file.css` - In this file we will wrote our css styles.
`file.html` - In this file we will write the actual ui design html elements.
`file.ts` - Actual event handling functions and other scrpting goes here.
`file.spec.ts` - User for testing the components.

# Bindings

Check the `First.ts` file for the full funcitonality

1. Property Binding :
   `<input type="text" #uname [value]="name"/>`

2. Two-Way Binding using the FormsModel(ngmodel):
   `<input type="text" [(ngModel)]="name"/>`

3. Event Binding
   `<button (click)="onButtonClick(uname.value)">ClickMe</button>`

4. NgFor used to map the json datas to the html element (same as like mapitem in js).
