import {NgModule, CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {RouterModule} from '@angular/router';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
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
    MatFormFieldModule,
    MatOptionModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
} from '@angular/material';
import {GetUsersComponent} from './users.component';
import {GetAddUsersComponent} from './add-user/add-user.component';
import {GetUsersRoutes, ViewProfileRoutes} from "./users.route";
import {MatTooltipModule} from '@angular/material/tooltip';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { CommonDirectivesModule } from '../../directives/common-directives.module';
import { GetUserProfileComponent } from 'src/app/views/users/viewprofile/viewprofile.component';
import { NgxMaskModule } from 'ngx-mask';

@NgModule({
    imports: [
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
        MatSelectModule,
        FormsModule,
        ReactiveFormsModule,
        MatInputModule,
        MatFormFieldModule, 
        MatTableModule,
        MatSortModule,
        MatPaginatorModule,
        TranslateModule,
        MatTooltipModule,
        CommonDirectivesModule,
        NgxMaskModule.forRoot(),
        RouterModule.forChild(GetUsersRoutes),
        RouterModule.forChild(ViewProfileRoutes),
    ],
    declarations: [GetUsersComponent,GetAddUsersComponent,
        GetUserProfileComponent
    ],

    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    exports: [GetAddUsersComponent,
         GetUserProfileComponent
    ]
})
export class GetUsersModule {
}
