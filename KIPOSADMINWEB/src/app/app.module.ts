import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Http, HttpModule, XHRBackend, RequestOptions } from '@angular/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoggerModule, NgxLoggerLevel } from 'ngx-logger';
import { RoutePartsService } from './services/route-parts/route-parts.service';
import { NavigationService } from "./services/navigation/navigation.service";
import { AuthService } from './services/auth/auth.service';
import { ChangePasswordService } from './views/change-password/change-password.service';
import { UsersService } from './views/users/users.service';


 import {
     MatDialogModule, MatPaginatorIntl, MatSidenavModule
 } from '@angular/material';
import { NgxSpinnerModule } from 'ngx-spinner';
import { AppCommonModule } from 'src/app/components/common/app-common.module';
import { rootRouterConfig } from 'src/app/app.routes';
import { AppComponent } from 'src/app/app.component';
import { ConsumerService } from 'src/app/views/consumer/consumer.service';

import { MenuService } from './views/menus/menu.service';
 import { ComplaintsService } from './views/complaints/complaints.service';
 import { ConfigurationService } from './views/configurations/configuration.service';
 import { CommunicationService } from './views/communications/communication.service';
 import { PermissionService } from './views/permissions/permission.service';
 import { plansService } from './views/plans/plans.service';
 import { SubMenuService } from './views/submenus/submenu.service';
import { DepartmentService } from './views/departments/department.service';
import { InfoDialogComponent } from './views/app-dialogs/info-dialog/info-dialog.component';
import { ReportService } from './views/reports/report.service';
import { cancelorderService} from './views/cancelorder/cancelorder.service';
import { ExportService } from './services/common/exportToExcel.service';
import { DashboardService} from './views/dashboard/dashboard.service';
import { SendReceiveService } from './services/common/sendReceive.service';
import { AppInfoService } from './services/common/appInfo.service';
import { ConsumerLayoutComponent } from './components/common/layouts/consumer-layout/consumer-layout.component';
import { RoleService } from './views/roles/role.service';
import { AppComfirmComponent } from 'src/app/views/app-dialogs/app-confirm/app-confirm.component';
import {HttpClientModule, HttpClient} from '@angular/common/http';
import {TranslateModule, TranslateLoader} from '@ngx-translate/core';
import {TranslateHttpLoader} from '@ngx-translate/http-loader';
import { MaskService } from 'ngx-mask';
import { LogoutService } from 'src/app/services/logout/logout.service';
import {AmChartsModule} from "@amcharts/amcharts3-angular";
import { ForgotPasswordUserListService } from 'src/app/views/forgotPassword-usersList/forgotPassword-userList.service';
import { getCMSPaginatorIntl } from './helpers/paginator.helper';
import { MemberService } from './views/members/member.service';
import { ResetpasswordService } from './views/resetpassword/resetpassword.service';
import { SiteReviewService} from './views/siteReviews/siteReview.service';
import { orderService} from './views/orders/order.service';
import { EnquiryService } from './views/enquiries/enquiry.service';
import { AnonymousEmailService } from './views/anonymousEmails/anonymousEmail.service';
import { CareerService  } from './views/careers/career.service';
import { ContactUsService } from './views/contactus/contactus.service';
import { VendorEnquiryService } from './views/vendorenquiry/vendorenquiry.service';
import { SchedulerDetailsService } from './views/schedulerDetails/scheduler-details.service';
import { FreeMembershipService } from './views/freeMembership/freeMembership.service';
import { storesService } from './views/stores/stores.service';
import { PosUsersService } from './views/posusers/posusers.service';
import { paymentService } from './views/payment/payment.service';
import { PlanConversionService } from './views/planConversion/planConversion.service';
import { DatePipe, DecimalPipe } from '@angular/common';

import { POSOrderService } from './views/pos-order/pos-order.service';

import { SalesReportService } from './views/reports/salesReports/salesReport.service';
import { POSTargetsComponent } from './views/pos-targets/pos-targets.component';
import { POSTargetsService } from './views/pos-targets/pos-targets.service';
import { CmsLandingPageService } from './views/cms-landing-page/cms-landing-page.service';
import { OurStoryComponentService } from './views/cms-landing-page/ourStory/ourStory.service';
import { OurBrandComponentService } from './views/cms-landing-page/ourBrands/ourBrands.service';
import { BannerImageComponentService } from './views/cms-landing-page/bannerImages/bannerImage.service';
import { InstagramPageService } from './views/cms-landing-page/instagramImages/instagramImage.service';
import { ReferralSectionService } from './views/cms-landing-page/referralSection/referralSection.service';
import { FeaturedArticlesComponentService } from './views/cms-landing-page/featuredArticles/featuredArticles.service';
import { NewsLetterMailService } from './views/cms-landing-page/news-letter/news-letter-emails/news-letter-mail.service';
import { NewsLetterComponentService } from './views/cms-landing-page/news-letter/news-letter.service';
import { AboutUsComponentService } from './views/cms-landing-page/about-us/aboutUs.service';
import { pendingorderService } from './views/pendingorder/pendingorder.service';
import { completedorderService } from './views/completedorder/completedorder.service';
import { KpointsService } from './views/kpoints/kpoints.service';
import { AllReportsService } from './views/reports/all-reports/all-reports.service';
import { ALLOrdersService } from './views/allorders/allorders.service';
import { StoreTimingsService } from './views/store-timings/store-timings.service';
import { deliveryordersService } from './views/deliveryOrders/deliveryorders.service';
import { CategoriesService } from './views/categories/categories.service';
import { CorpMasterService } from './views/CorpMaster/corp-master.service';
import { IngradientCountReportService } from './views/reports/IngradientCountReport/IngradientCountReport.service';
import { ExcelService } from './services/common/ExcelService';




export function HttpLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http);
}



@NgModule({
    imports: [
        NgxSpinnerModule ,
        BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        MatDialogModule,
        MatSidenavModule,
        BrowserAnimationsModule,
        HttpModule,
        HttpClientModule,
        AppCommonModule,
        AmChartsModule,

        LoggerModule.forRoot({serverLoggingUrl: '/api/logs', level: NgxLoggerLevel.DEBUG, serverLogLevel: NgxLoggerLevel.ERROR}),
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: HttpLoaderFactory,
                deps: [HttpClient]
            }
            }),
        // TreeModule,
        RouterModule.forRoot(rootRouterConfig,
            {
                // useHash: true,
                enableTracing: true // <-- debugging purposes only
            }
        )
    ],
    declarations: [AppComponent, 
       InfoDialogComponent,
       AppComfirmComponent,
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    entryComponents: [ConsumerLayoutComponent,AppComfirmComponent,InfoDialogComponent],
    providers: [
         RoutePartsService,
         AuthService,
         NavigationService,
         MaskService,
        LogoutService,
        MaskService ,
        SalesReportService,
        ChangePasswordService,
         UsersService,
         AppInfoService,
         SendReceiveService,
         MenuService,
         DatePipe,
         DecimalPipe,
         ExcelService,
         RoleService,
         ComplaintsService,
         ConfigurationService,
         StoreTimingsService,
         CommunicationService,
         MemberService,
         ResetpasswordService,
         ALLOrdersService,
         AllReportsService,
         SiteReviewService,
         orderService,
         IngradientCountReportService,
         PermissionService,
         plansService,
         SubMenuService,
         ConsumerService,
         EnquiryService,
        DepartmentService,
        ReportService,
        cancelorderService,
        pendingorderService,
        deliveryordersService,
        completedorderService,
        ExportService,
        DashboardService,
        ForgotPasswordUserListService,
        AnonymousEmailService,
        CareerService,
        ContactUsService,
        VendorEnquiryService,
        SchedulerDetailsService,
        FreeMembershipService,
        storesService,
        PosUsersService,
        paymentService,
        PlanConversionService,
        KpointsService,
        POSOrderService,
        POSTargetsService,
        CmsLandingPageService,
        OurStoryComponentService,
        OurBrandComponentService,
      BannerImageComponentService,
      InstagramPageService,
      ReferralSectionService,
      CorpMasterService,
      FeaturedArticlesComponentService,
      NewsLetterMailService,
      NewsLetterComponentService,
      AboutUsComponentService,
      CategoriesService,
        { provide: MatPaginatorIntl, useValue: getCMSPaginatorIntl() }
      ],
    bootstrap: [AppComponent]
})
export class AppModule {
 }
