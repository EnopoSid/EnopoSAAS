import {NgModule, CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from "@angular/router";

import {
    MatProgressBarModule,
    MatButtonModule,
    MatInputModule,
    MatCardModule,
    MatCheckboxModule,
    MatIconModule
} from '@angular/material';
import {ForgotPasswordComponent} from './forgot-password/forgot-password.component';
import {SigninComponent} from './signin/signin.component';
import {SessionsRoutes} from "./sessions.route";
import { NgxSpinnerModule } from 'ngx-spinner';
import {TranslateModule} from '@ngx-translate/core';


@NgModule({
    imports: [
        NgxSpinnerModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        MatProgressBarModule,
        MatButtonModule,
        MatInputModule,
        MatCardModule,
        MatCheckboxModule,
        MatIconModule,
        RouterModule.forChild(SessionsRoutes),
        TranslateModule
    ],
    schemas: [],
    declarations: [
        ForgotPasswordComponent,
         SigninComponent,
        ]
})
export class SessionsModule {
}
