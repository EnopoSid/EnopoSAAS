import {NgModule, CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from "@angular/forms";
import {BrowserModule} from "@angular/platform-browser";
import {RouterModule} from "@angular/router";
import {TranslateModule} from '@ngx-translate/core';

import {
    MatSidenavModule,
    MatListModule,
    MatTooltipModule,
    MatDialogModule,
    MatOptionModule,
    MatSelectModule,
    MatMenuModule,
    MatSnackBarModule,
    MatGridListModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatRadioModule,
    MatCheckboxModule,
    MatCardModule
} from '@angular/material';
import {NotificationsComponent} from './notifications/notification.component';
 import {AdminLayoutComponent} from './layouts/admin-layout/admin-layout.component';
 import {AuthLayoutComponent} from './layouts/auth-layout/auth-layout.component';
 import { AnonymousLayoutComponent } from './layouts/anonymous-layout/anonymous-layout.component';
import { FooterComponent } from './footer/footer.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ConsumerLayoutComponent } from './layouts/consumer-layout/consumer-layout.component';
import { NavigationComponent } from './navigation/navigation.component';
import { TopbarComponent } from './topbar/topbar.component';
import { CommonDirectivesModule } from '../../directives/common-directives.module';

@NgModule({
    imports: [
        NgxSpinnerModule,
        CommonModule,
        FormsModule,
        BrowserModule,
        RouterModule,
        MatDialogModule,
        MatSidenavModule,
        MatIconModule,
        MatListModule,
        MatTooltipModule,
        MatOptionModule,
        MatSelectModule,
        MatMenuModule,
        MatSnackBarModule,
        MatGridListModule,
        MatToolbarModule,
        MatButtonModule,
        MatRadioModule,
        MatCheckboxModule,
        MatCardModule,
        CommonDirectivesModule,
        TranslateModule
    ],
    declarations: [
         AdminLayoutComponent,
         AuthLayoutComponent,
         AnonymousLayoutComponent,
         NavigationComponent,
         FooterComponent,
         NotificationsComponent, 
        ConsumerLayoutComponent,
        TopbarComponent
    ],

    exports: [],
    schemas: []
})
export class AppCommonModule {

}
