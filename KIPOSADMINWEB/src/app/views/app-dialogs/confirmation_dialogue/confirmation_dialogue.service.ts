import { Observable } from 'rxjs';
import { MatDialogRef, MatDialog, MatDialogConfig } from '@angular/material';
import { Injectable } from '@angular/core';
import { ComfirmComponent } from './confirmation_dialogue.component';



@Injectable()
export class ConfirmDialogueService {

  constructor(private dialog: MatDialog) { }

  public confirm(title: string, message: string): Observable<boolean> {
    let dialogRef: MatDialogRef<ComfirmComponent>;
    dialogRef = this.dialog.open(ComfirmComponent, {disableClose: true});
    dialogRef.updateSize('380px');
    dialogRef.componentInstance.title = title;
    dialogRef.componentInstance.message = message;
    return dialogRef.afterClosed();
  }
}