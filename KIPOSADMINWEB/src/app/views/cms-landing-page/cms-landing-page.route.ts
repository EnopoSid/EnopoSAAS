import { Routes } from '@angular/router';
import { CmsLandingPageComponent } from './cms-landing-page.component';
import { OurStoryComponent } from './ourStory/ourStory.component';
import { OurBrandsComponent } from './ourBrands/ourBrands.component';
import { AddOurStoryComponent } from './ourStory/addOurStory/addOurStory.component';
import { AddOurBrandsComponent } from './ourBrands/addOurBrands/addOurBrands.component';
import { BannerImageComponent } from './bannerImages/bannerImage.component';
import { InstagramPageComponent } from './instagramImages/instagrampage.component';
import { ReferralSectionComponent } from './referralSection/referralSection.component';
import { AddReferralComponent } from './referralSection/addReferralSection/addReferral.component';
import { FeaturedArticlesComponent } from './featuredArticles/featuredArticles.component';
import { AddFeaturedArticlesComponent } from './featuredArticles/addFeaturedArticles/addFeaturedArticles.component';
import { NewsLetterComponent } from './news-letter/new-letter.component';
import { NewsLetterMailComponent } from './news-letter/news-letter-emails/news-letter-mail.component';
import { AddNewsLetterComponent } from './news-letter/addNewsLetter/addNewsLetter.component';
import { AboutUsComponent } from './about-us/aboutUs.component';
import { AddAboutUsComponent } from './about-us/addAboutUs/addAboutUs.component';
import { CategoriesComponent } from './categories/categories.component';




export const CmsLandingPageRoutes: Routes = [
    {path: '', component:CmsLandingPageComponent, data:{title: 'landingpage'}},
    { path: 'ourStory', component: OurStoryComponent, data: { title: 'Our-Story' }},
    { path: 'ourStory/addOurStory', component: AddOurStoryComponent, data: { title: 'Add-Our-Story' }},
    { path: 'ourStory/addOurStory/:id', component: AddOurStoryComponent, data: { title: 'View-Our-Story' }},
    { path: 'ourStory/addOurStory/:id', component: AddOurStoryComponent, data: { title: 'Edit-Our-Story' }},
    { path: 'ourBrands', component: OurBrandsComponent, data: { title: 'Our-Brands' }},
    { path: 'ourBrands/addourBrands', component: AddOurBrandsComponent, data: { title: 'Add-Our-Brands' }},
    { path: 'ourBrands/addourBrands/:id', component: AddOurBrandsComponent, data: { title: 'View-Our-Brands' }},
    { path: 'ourBrands/addourBrands/:id', component: AddOurBrandsComponent, data: { title: 'Edit-Our-Brands' }},
    { path: 'bannerImages', component: BannerImageComponent, data: { title: 'Our-Brands' }},
    { path: 'trendingBowls', component: InstagramPageComponent, data: { title: 'Instagram-Image' }},
    { path: 'referralSection', component: ReferralSectionComponent, data: { title: 'Referral-Section' }},
    { path: 'referralSection/addReferral', component: AddReferralComponent, data: { title: 'Add-Our-Story' }},
    { path: 'referralSection/addReferral/:id', component: AddReferralComponent, data: { title: 'View-Our-Story' }},
    { path: 'referralSection/addReferral/:id', component: AddReferralComponent, data: { title: 'Edit-Our-Story' }},
    { path: 'featuredArticles', component: FeaturedArticlesComponent, data: { title: 'Featured-Articles' }},
    { path: 'featuredArticles/addFeaturedArticles', component: AddFeaturedArticlesComponent, data: { title: 'Add-Featured-Articles' }},
    { path: 'featuredArticles/addFeaturedArticles/:id', component: AddFeaturedArticlesComponent, data: { title: 'View-Featured-Articles' }},
    { path: 'featuredArticles/addFeaturedArticles/:id', component: AddFeaturedArticlesComponent, data: { title: 'Edit-Featured-Articles' }},
    { path: 'newsLetter', component: NewsLetterComponent, data: { title: 'News-Letter' }},
    { path: 'newsLetter/addNewsLetter', component: AddNewsLetterComponent, data: { title: 'Add-News-Letter' }},
    { path: 'newsLetter/addNewsLetter/:id', component: AddNewsLetterComponent, data: { title: 'Add-News-Letter' }},
    { path:  'newsLetter/newsLetterMails', component:NewsLetterMailComponent, data:{title: 'News-Letter-Email'}},
    { path: 'aboutUs', component: AboutUsComponent, data: { title: 'About-Us' }},
    { path: 'aboutUs/addAboutUs', component: AddAboutUsComponent, data: { title: 'Add-About-Us' }},
    { path: 'aboutUs/addAboutUs/:id', component: AddAboutUsComponent, data: { title: 'View-About-Us' }},
    { path: 'aboutUs/addAboutUs/:id', component: AddAboutUsComponent, data: { title: 'Edit-About-Us' }},
    { path: 'categories', component: CategoriesComponent, data: { title: 'Categories' }}
  
];