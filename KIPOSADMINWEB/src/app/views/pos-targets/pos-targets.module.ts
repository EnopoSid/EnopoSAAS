
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
    MatFormFieldModule,
    MatOptionModule,
    MatSelectModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatDatepickerModule,
    MatNativeDateModule,
} from '@angular/material';
import {MatTooltipModule} from '@angular/material/tooltip';
import { POSTargetsRoutes } from './pos-targets.route';
import { TranslateModule } from '@ngx-translate/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { CommonDirectivesModule } from 'src/app/directives/common-directives.module';
import { NgxMaskModule } from 'ngx-mask';
import { RecaptchaModule } from 'ng-recaptcha';
import { POSTargetsComponent } from './pos-targets.component';
import { AddPOSTargetsComponent } from './addPOSTargets/addPOSTargets.component';

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
    MatDatepickerModule,
    MatNativeDateModule,
    NgxMaskModule.forRoot(),
    RouterModule.forChild(POSTargetsRoutes),
     RecaptchaModule.forRoot(),

],
declarations:[
    POSTargetsComponent,AddPOSTargetsComponent
],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[POSTargetsComponent,AddPOSTargetsComponent]
})
export class  POSTargetsModule{

}