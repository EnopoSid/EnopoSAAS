import { Inject, Component, ViewEncapsulation } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialog } from "@angular/material";

@Component({
    selector: 'info-dialog',
    templateUrl: 'info-dialog.component.html',
    styleUrls: ['info-dialog.component.css'],
    encapsulation: ViewEncapsulation.None
  })
  export class InfoDialogComponent {
    constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}
  }