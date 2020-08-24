
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
    MatTableModule,
    MatPaginatorModule,
    MatSortModule
} from '@angular/material';
import { ForgotPasswordUserListComponent } from './forgotPassword-userList.component';
import { ForgotPasswordUserListRoutes } from './forgotPassword-userList.route';
import {MatTooltipModule} from '@angular/material/tooltip';
import { SubMenuComponent } from '../submenus/submenu.component';
import { CommonDirectivesModule } from '../../directives/common-directives.module';
import { NgxSpinnerModule } from 'ngx-spinner';


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
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        MatChipsModule,
        MatCheckboxModule,
        MatDialogModule,
        MatRadioModule,
        MatTabsModule,
        MatInputModule,
        MatProgressBarModule,
        MatTooltipModule,
        ReactiveFormsModule ,
        CommonDirectivesModule,
        RouterModule.forChild(ForgotPasswordUserListRoutes)
],
declarations:[ForgotPasswordUserListComponent],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class ForgotPasswordUserListModule{

}