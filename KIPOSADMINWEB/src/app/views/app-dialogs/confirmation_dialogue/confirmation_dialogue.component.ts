import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-confirmdialogue',
    templateUrl: './confirmation_dialogue.component.html',
    styleUrls: ['./confirmation_dialogue.component.css']
})

export class ComfirmComponent {

  public title: string;
  public message: string;

  constructor(public dialogRef: MatDialogRef<ComfirmComponent>,@Inject(MAT_DIALOG_DATA) public data: any) {}


  ngOnInit() {
  }
    onDismiss(): void {
    this.dialogRef.close(false);
  }

  onConfirm(): void {
    this.dialogRef.close(true);
  }
}
