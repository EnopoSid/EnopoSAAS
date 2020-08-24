import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import {RouterModule } from '@angular/router';
import {CommonModule, DatePipe } from '@angular/common';
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
 import { MatTooltipModule} from '@angular/material/tooltip';

import { TranslateModule } from '@ngx-translate/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { CommonDirectivesModule } from 'src/app/directives/common-directives.module';
import { NgxMaskModule} from 'ngx-mask';
import {CmsLandingPageRoutes } from './cms-landing-page.route';
import { CmsLandingPageComponent } from './cms-landing-page.component';
import { OurBrandsComponent } from './ourBrands/ourBrands.component';
import { OurStoryComponent } from './ourStory/ourStory.component';

import { AddOurBrandsComponent } from './ourBrands/addOurBrands/addOurBrands.component';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { AddOurStoryComponent } from './ourStory/addOurStory/addOurStory.component';
import { BannerImageComponent } from './bannerImages/bannerImage.component';
import { InstagramPageComponent } from './instagramImages/instagrampage.component';
import { ConfirmModule } from '../app-dialogs/confirmation_dialogue/confirmation_dialogue.module';
import { ReferralSectionComponent } from './referralSection/referralSection.component';
import { AddReferralComponent } from './referralSection/addReferralSection/addReferral.component';
import { FeaturedArticlesComponent } from './featuredArticles/featuredArticles.component';
import { AddFeaturedArticlesComponent } from './featuredArticles/addFeaturedArticles/addFeaturedArticles.component';
import { NewsLetterMailComponent } from './news-letter/news-letter-emails/news-letter-mail.component';
import { NewsLetterComponent } from './news-letter/new-letter.component';
import { AddNewsLetterComponent } from './news-letter/addNewsLetter/addNewsLetter.component';
import { AboutUsComponent } from './about-us/aboutUs.component';
import { AddAboutUsComponent } from './about-us/addAboutUs/addAboutUs.component';
import { CategoriesComponent } from './categories/categories.component';


@NgModule ({
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
    ConfirmModule,
    AngularEditorModule,
    NgxMaskModule.forRoot(),
   RouterModule.forChild(CmsLandingPageRoutes)
   ],

   declarations:[
      CmsLandingPageComponent,
      OurStoryComponent,
      OurBrandsComponent,
      AddOurStoryComponent,
      AddOurBrandsComponent,
       BannerImageComponent,
       InstagramPageComponent,
       ReferralSectionComponent,
       AddReferralComponent, 
       FeaturedArticlesComponent,
       AddFeaturedArticlesComponent,
       NewsLetterMailComponent,
       NewsLetterComponent,
       AddNewsLetterComponent,
       AboutUsComponent,
       AddAboutUsComponent,
       CategoriesComponent
   ],
   schemas:[CUSTOM_ELEMENTS_SCHEMA],
   exports:[CmsLandingPageComponent,
      OurStoryComponent,
      OurBrandsComponent,
      AddOurStoryComponent,
      AddOurBrandsComponent,
      BannerImageComponent,
      InstagramPageComponent,
      ReferralSectionComponent,
      AddReferralComponent, 
      FeaturedArticlesComponent,
      AddFeaturedArticlesComponent,
      NewsLetterMailComponent,
      NewsLetterComponent,
      AddNewsLetterComponent,
      AboutUsComponent,
       AddAboutUsComponent,
       CategoriesComponent
   ]
})
export class CmsLandingPageModule{
   
}