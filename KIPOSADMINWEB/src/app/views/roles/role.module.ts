
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
    MatPaginatorModule,
    MatSortModule
} from '@angular/material';
import { RoleComponent } from './role.component';
import { RoleRoutes, RolePermissionsRoutes } from './role.route';
import {MatTooltipModule} from '@angular/material/tooltip';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { CommonDirectivesModule } from 'src/app/directives/common-directives.module';
import { RolePermissionComponent } from 'src/app/views/roles/rolepermissions/rolepermission.component';




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
        MatPaginatorModule,
        MatSortModule,
        TranslateModule,
        MatTooltipModule,
        ReactiveFormsModule ,
        CommonDirectivesModule,
        RouterModule.forChild(RoleRoutes),
],
declarations:[RoleComponent],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class RoleModule{
   
}