
import {NgModule, CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {RouterModule} from '@angular/router';
import {CommonModule} from '@angular/common';
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
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule
} from '@angular/material';

import {MatTooltipModule} from '@angular/material/tooltip';
import { CommonDirectivesModule } from '../../../directives/common-directives.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { IngradientCountReportComponent } from './IngradientCountReport.component';
import { IngradientCountRoute } from './IngradientCountReport.route';

@NgModule({
imports:[
    NgxSpinnerModule,
        CommonModule,
        FormsModule,
        MatListModule,
        MatIconModule,
        MatButtonModule,
        MatCardModule,
        MatMenuModule,
        MatSlideToggleModule,
        MatGridListModule,
        MatChipsModule,
        MatCheckboxModule,
        MatDialogModule,
        MatRadioModule,
        MatTabsModule,
        MatInputModule,
        MatProgressBarModule,
        MatSelectModule,
        MatTooltipModule,
        MatTableModule,
        MatSortModule,
        MatPaginatorModule,
        MatDatepickerModule,
        MatNativeDateModule,
        ReactiveFormsModule ,
        MatDatepickerModule,
        CommonDirectivesModule,
        RouterModule.forChild(IngradientCountRoute)
],
declarations:[IngradientCountReportComponent],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class IngradientCountReportModule{

}
