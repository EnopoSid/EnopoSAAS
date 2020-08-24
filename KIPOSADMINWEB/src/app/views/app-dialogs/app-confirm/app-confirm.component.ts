import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-confirm',
    templateUrl: './app-confirm.component.html',
    styleUrls: ['./app-confirm.component.css']
})

export class AppComfirmComponent {

  public title: string;
  public message: string;

  constructor(public dialogRef: MatDialogRef<AppComfirmComponent>, @Inject(MAT_DIALOG_DATA) public data: any) { }
}
