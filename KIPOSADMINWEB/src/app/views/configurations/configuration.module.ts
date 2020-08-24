
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
    MatSelectModule,
    MatChipsModule,
    MatCheckboxModule,
    MatRadioModule,
    MatTabsModule,
    MatInputModule,
    MatProgressBarModule,
    MatTableModule,
    MatPaginator,
    MatPaginatorModule,
    MatSortModule
} from '@angular/material';
import { ConfigurationComponent } from './configuration.component';
import { ConfigurationRoutes } from './configuration.route';
import {MatTooltipModule} from '@angular/material/tooltip';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { CommonDirectivesModule } from '../../directives/common-directives.module';


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
        MatSelectModule,
        MatTabsModule,
        MatInputModule,
        MatProgressBarModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        TranslateModule,
        MatTooltipModule,
        ReactiveFormsModule ,
        CommonDirectivesModule,
        RouterModule.forChild(ConfigurationRoutes)
],
declarations:[ConfigurationComponent],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class ConfigurationModule{

}