import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import {RouterModule } from '@angular/router';
import {CommonModule, DatePipe } from '@angular/common';
import {FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
    MatIconModule,
    MatDialogModule,
    MatButtonModule,
    MatCardModule,
    MatListModule,
    MatMenuModule,
    MatSlideToggleModule,
    MatGridListModule,
    MatChipsModule,
    MatCheckboxModule,
    MatRadioModule,
    MatTabsModule,
    MatInputModule,
    MatProgressBarModule,
    MatFormFieldModule,
    MatOptionModule,
    MatSelectModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
 } from '@angular/material';
 import { MatTooltipModule} from '@angular/material/tooltip';


import { TranslateModule } from '@ngx-translate/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { CommonDirectivesModule } from 'src/app/directives/common-directives.module';
import { NgxMaskModule} from 'ngx-mask';
import { NewsLetterMailRoutes } from './news-letter-mail.route';
import { NewsLetterMailComponent } from './news-letter-mail.component';

@NgModule ({
   imports:[
       NgxSpinnerModule,
       CommonModule,
       FormsModule,
       MatListModule,
       MatIconModule,
       MatButtonModule,
       MatCardModule,
       MatMenuModule,
       MatSelectModule,
       MatSlideToggleModule,
       MatGridListModule,
       MatChipsModule,
       MatOptionModule,
       MatCheckboxModule,
       MatDialogModule,
       MatRadioModule,
       MatTabsModule,
       FormsModule,
       ReactiveFormsModule,
       MatInputModule,
       MatFormFieldModule,
       MatProgressBarModule,
       TranslateModule,
       MatTableModule,
       MatSortModule,
       MatPaginatorModule,
    MatTooltipModule,
    MatSelectModule,
    CommonDirectivesModule,

    NgxMaskModule.forRoot(),
   RouterModule.forChild(NewsLetterMailRoutes)
   ],
   providers: [DatePipe],
   declarations:[
    NewsLetterMailComponent
   ],
   schemas:[CUSTOM_ELEMENTS_SCHEMA],
   exports:[]
})
export class NewsLetterModule{}