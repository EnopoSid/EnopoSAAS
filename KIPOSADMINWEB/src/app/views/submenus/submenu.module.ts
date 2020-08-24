
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
    MatSelectModule,
    MatInputModule,
    MatProgressBarModule,
    MatPaginatorModule,
    MatSortModule,
    MatTableModule
} from '@angular/material';

import {MatTooltipModule} from '@angular/material/tooltip';
import { SubMenuComponent } from './submenu.component';
import { SubMenuRoutes } from './submenu.route';
import { CommonDirectivesModule } from '../../directives/common-directives.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TranslateModule } from '@ngx-translate/core';

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
        MatTabsModule,
        MatPaginatorModule,
        MatSortModule,
        MatDialogModule,
        MatRadioModule,
        MatTabsModule,
        MatInputModule,
        MatProgressBarModule,
        MatSelectModule,
        TranslateModule,
        MatTooltipModule,
        MatTableModule,
        MatSortModule,
        MatPaginatorModule,
        ReactiveFormsModule ,
        CommonDirectivesModule,
        RouterModule.forChild(SubMenuRoutes)
],
declarations:[SubMenuComponent],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class SubMenuModule{

}