
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
    MatSelectModule,
    MatProgressBarModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule
} from '@angular/material';
import {MatTooltipModule} from '@angular/material/tooltip';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { CommonDirectivesModule } from 'src/app/directives/common-directives.module';
import { CategoriesComponent } from './categories.component';
import { CategoriesRoutes } from './categories.route';
import { ConfirmModule } from '../app-dialogs/confirmation_dialogue/confirmation_dialogue.module';


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
        MatSelectModule,
        MatProgressBarModule,
        MatTableModule,
        MatSortModule,
        MatPaginatorModule,
        TranslateModule,
        MatTooltipModule,
        ReactiveFormsModule,
        ConfirmModule,
        CommonDirectivesModule,
        RouterModule.forChild(CategoriesRoutes), 
],
declarations:[CategoriesComponent],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class CategoriesModule{
   
}