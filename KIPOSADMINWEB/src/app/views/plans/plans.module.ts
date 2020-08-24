
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
    MatSelectModule,
    MatPaginatorModule,
    MatTableModule,
    MatSortModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatOptionModule,
    MatNativeDateModule,
} from '@angular/material';
import { PlansComponent } from './plans.component';
import { PlansRoutes } from './plans.route';
import {MatTooltipModule} from '@angular/material/tooltip';
import { CommonDirectivesModule } from '../../directives/common-directives.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { AddPlans } from './addPlans/addPlans.component';
import { NgxMaskModule } from 'ngx-mask';
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
        RouterModule.forChild(PlansRoutes)
],
declarations:[PlansComponent,AddPlans],
schemas:[CUSTOM_ELEMENTS_SCHEMA],
exports:[PlansComponent,AddPlans]
})
export class PlansModule{

}