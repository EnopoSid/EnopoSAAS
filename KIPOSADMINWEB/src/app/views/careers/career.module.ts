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
import { CareerRoutes } from './career.route';
import {GetCareerComponent } from './career.component';
import { TranslateModule } from '@ngx-translate/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { CommonDirectivesModule } from 'src/app/directives/common-directives.module';
import { NgxMaskModule} from 'ngx-mask';

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
   RouterModule.forChild(CareerRoutes)
   ],
   providers: [DatePipe],
   declarations:[
      GetCareerComponent
   ],
   schemas:[CUSTOM_ELEMENTS_SCHEMA],
   exports:[]
})
export class CareerModule{}