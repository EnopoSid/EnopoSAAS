import { AppConfirmService } from './app-confirm.service';
import {
  MatDialogModule,
  MatButtonModule
 } from '@angular/material';
import { NgModule } from '@angular/core';

import { AppComfirmComponent } from './app-confirm.component';

@NgModule({
  imports: [
    MatDialogModule,
    MatButtonModule,
  ],
  exports: [AppComfirmComponent],
  declarations: [AppComfirmComponent],
  providers: [AppConfirmService],
  entryComponents: [AppComfirmComponent]
})
export class AppConfirmModule {

}
