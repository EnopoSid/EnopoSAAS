
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
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule
} from '@angular/material';
import { TranslateModule } from '@ngx-translate/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { MatTooltipModule} from '@angular/material/tooltip';
import { ComplaintListComponent } from './complaint-list.component';
import { ConsumerAddComplaintComponent } from './add-complaint/consumer/add-complaint.component';
import { UserAddComplaintComponent } from './add-complaint/user/add-complaint.component';
import { ComplaintsRoutes, ConsumerRoutes } from '../../views/complaints/complaints.route';
import { CommonDirectivesModule } from '../../directives/common-directives.module';
import { RecaptchaModule } from 'ng-recaptcha';
import { ComplaintSummaryComponent } from './complaint-summary/complaint-summary.component';
import {NgxMaskModule} from 'ngx-mask';
import { CheckStatusComponent } from 'src/app/views/consumer/check-status/check-status.component';


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
        MatDatepickerModule,
        MatNativeDateModule,
        MatTabsModule,
        MatInputModule,
        MatSelectModule,
        MatProgressBarModule,
        TranslateModule,
        MatTooltipModule,
        ReactiveFormsModule ,
        MatSelectModule,
        MatInputModule,
        MatTableModule ,
        MatPaginatorModule,
        MatSortModule,
        NgxMaskModule.forRoot(),
        RouterModule.forChild(ComplaintsRoutes),
        RouterModule.forChild(ConsumerRoutes),
         RecaptchaModule.forRoot(),
        CommonDirectivesModule,
        //CommonPipesModule,
],
declarations:[
    ComplaintListComponent,
    ConsumerAddComplaintComponent,
    UserAddComplaintComponent,
    CheckStatusComponent,
     ComplaintSummaryComponent
],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class ComplaintsModule{

}