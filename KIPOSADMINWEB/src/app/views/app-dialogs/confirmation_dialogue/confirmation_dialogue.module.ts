import { ConfirmDialogueService } from './confirmation_dialogue.service';
import {
  MatDialogModule,
  MatButtonModule
 } from '@angular/material';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

import { ComfirmComponent } from './confirmation_dialogue.component';

@NgModule({
  imports: [
    MatDialogModule,
    MatButtonModule,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  exports: [ComfirmComponent],
  declarations: [ComfirmComponent],
  providers: [ConfirmDialogueService],
  entryComponents: [ComfirmComponent]
})
export class ConfirmModule {

}
