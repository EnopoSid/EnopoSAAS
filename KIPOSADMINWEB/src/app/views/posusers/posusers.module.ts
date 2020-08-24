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
import {GetPosUsersComponent} from './posusers.component';
import {GetAddPosUsersComponent} from './add-posusers/add-posusers.component';
import {GetPosUsersRoutes} from "./posusers.route";
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
        RouterModule.forChild(GetPosUsersRoutes)
    ],
    declarations: [GetPosUsersComponent,GetAddPosUsersComponent
    ],

    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    exports: [GetAddPosUsersComponent,
        GetPosUsersComponent
    ]
})
export class GetPosUsersModule {
}
