
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
    MatPaginatorModule,
    MatTableModule,
    MatSortModule,
} from '@angular/material';
import { PermissionComponent } from './permission.component';
import { PermissionRoutes } from './permission.route';
import {MatTooltipModule} from '@angular/material/tooltip';
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
        MatDialogModule,
        MatRadioModule,
        MatTabsModule,
        MatSelectModule,
        MatInputModule,
        MatProgressBarModule,
        MatPaginatorModule,
        MatTableModule,
        MatSortModule,
        TranslateModule,
        MatTooltipModule,
        ReactiveFormsModule ,
        CommonDirectivesModule,
        RouterModule.forChild(PermissionRoutes)
],
declarations:[PermissionComponent],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class PermissionModule{

}