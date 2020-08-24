import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';
import {
    MatInputModule,
    MatCardModule,
    MatListModule,
    MatButtonModule,
    MatProgressBarModule,
    MatCheckboxModule,
    MatIconModule
} from '@angular/material';

import { AppConfirmModule } from '../..//views/app-dialogs/app-confirm/app-confirm.module';
import { AppConfirmService } from '../../views/app-dialogs/app-confirm/app-confirm.service';
import { DialogsRoutes } from '../../views/app-dialogs/app-dialogs.route';


@NgModule({
    imports: [
        ReactiveFormsModule,
        CommonModule,
        FormsModule,
        MatProgressBarModule,
        MatInputModule,
        MatCardModule,
        MatButtonModule,
        MatListModule,
        AppConfirmModule,
        RouterModule.forChild(DialogsRoutes)
    ],
    providers: [AppConfirmService]
})
export class AppDialogsModule {
}
