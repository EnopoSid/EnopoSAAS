
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
} from '@angular/material';
import {MatTooltipModule} from '@angular/material/tooltip';
import { EnquiryRoutes, ConsumerRoutes } from './enquiry.route';
import { GetEnquiryComponent } from './enquiry.component';
import { TranslateModule } from '@ngx-translate/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { CommonDirectivesModule } from 'src/app/directives/common-directives.module';
import { UserAddEnquiryComponent } from 'src/app/views/enquiries/add-enquiry/user/add-enquiry.component';
import { NgxMaskModule } from 'ngx-mask';
import { RecaptchaModule } from 'ng-recaptcha';
import { ConsumerAddEnquiryComponent } from 'src/app/views/enquiries/add-enquiry/consumer/add-enquiry.component';

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
    NgxMaskModule.forRoot(),
    RouterModule.forChild(EnquiryRoutes),
    RouterModule.forChild(ConsumerRoutes),
     RecaptchaModule.forRoot(),

],
declarations:[
    GetEnquiryComponent,
     UserAddEnquiryComponent, 
    ConsumerAddEnquiryComponent
],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class EnquiryModule{

}