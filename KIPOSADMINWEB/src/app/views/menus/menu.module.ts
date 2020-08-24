
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
    MatSelectModule,
    MatRadioModule,
    MatTabsModule,
    MatInputModule,
    MatProgressBarModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
} from '@angular/material';
import { MenusComponent} from "./menu.component";
import { MenuRoutes } from './menu.route';
import {MatTooltipModule} from '@angular/material/tooltip';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { CommonDirectivesModule } from '../../directives/common-directives.module';
import { NgxMaskModule } from 'ngx-mask';


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
        MatSortModule,
        MatPaginatorModule,
        TranslateModule,
        MatTooltipModule,
        NgxMaskModule.forRoot(),
        ReactiveFormsModule ,
        CommonDirectivesModule,
        RouterModule.forChild(MenuRoutes)
],
declarations:[MenusComponent],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class MenusModule{

}