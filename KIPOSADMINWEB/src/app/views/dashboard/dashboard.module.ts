import {NgModule, CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {CommonModule} from '@angular/common';
import {
    MatIconModule,
    MatCardModule,
    MatMenuModule,
    MatProgressBarModule,
    MatButtonModule,
    MatChipsModule,
    MatSelectModule,
    MatListModule,
    MatGridListModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatSlideToggleModule,
    MatCheckboxModule,
    MatDialogModule,
    MatRadioModule,
    MatTabsModule,
    MatTooltipModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSidenavModule,
} from '@angular/material';
import {RouterModule} from '@angular/router';
import {ChartsModule} from 'ng2-charts/ng2-charts';
import {DashboardComponent} from './dashboard.component';
import {DashboardRoutes} from "./dashboard.route";
import { TranslateModule } from '@ngx-translate/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';
import {DatePipe} from '@angular/common';
import { CommonDirectivesModule } from 'src/app/directives/common-directives.module';
@NgModule({
    imports: [
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
        TranslateModule,
        MatTooltipModule,
        ReactiveFormsModule ,
        MatSelectModule,
        MatTableModule ,
        MatPaginatorModule,
        MatSortModule,
        MatDatepickerModule,
        MatNativeDateModule,
    ReactiveFormsModule ,
    MatDatepickerModule,
    MatSidenavModule,
    CommonDirectivesModule,
        RouterModule.forChild(DashboardRoutes)
    ],
    declarations: [DashboardComponent],
    providers: [DatePipe],
    exports: [],
    schemas: [],
})
export class DashboardModule {

}
