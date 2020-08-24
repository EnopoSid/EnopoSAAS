
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
import { MemberSubscriptionRoutes } from './memberSubscription.route';
import { GetMemberSubscriptionComponent } from './memberSubscription.component';
import { TranslateModule } from '@ngx-translate/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { CommonDirectivesModule } from 'src/app/directives/common-directives.module';
import { NgxMaskModule } from 'ngx-mask';
import { RecaptchaModule } from 'ng-recaptcha';

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
    RouterModule.forChild(MemberSubscriptionRoutes),
     RecaptchaModule.forRoot(),

],
declarations:[
GetMemberSubscriptionComponent
],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[]
})
export class MemberSubscriptionModule{

}