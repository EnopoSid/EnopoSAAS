import { Routes } from '@angular/router';

import { AdminLayoutComponent } from './components/common/layouts/admin-layout/admin-layout.component';
import { AuthLayoutComponent } from './components/common/layouts/auth-layout/auth-layout.component';

import { AuthService } from './services/auth/auth.service';
import { ConsumerLayoutComponent } from './components/common/layouts/consumer-layout/consumer-layout.component';
import { AnonymousLayoutComponent } from './components/common/layouts/anonymous-layout/anonymous-layout.component';


export const rootRouterConfig: Routes = [
    {
        path: '',
        component: ConsumerLayoutComponent,
        children: [
            {
                path: '',
                loadChildren: './views/consumer/consumer.module#ConsumerModule',
                data: { title: '' }
            }
        ]
    },
    {
        path: 'sessions',
        component: AuthLayoutComponent, loadChildren: './views/sessions/sessions.module#SessionsModule',
    },
    {
        path: 'consumer_enquiry',
        component: AnonymousLayoutComponent,
        loadChildren: './views/enquiries/enquiry.module#EnquiryModule',
        data: { title: 'Consumer Enquiry', breadcrumb: 'Consumer Enquiry' }
    },
    {
        path: '',
        component: AdminLayoutComponent,
        canActivate: [AuthService],
        children: [
            {
                path: 'menus',
                canActivateChild: [AuthService],
                loadChildren: './views/menus/menu.module#MenusModule',
                data: { title: 'Menu', breadcrumb: 'Menu' }
            },
            {
                path: 'roles',
                canActivateChild: [AuthService],
                loadChildren: './views/roles/role.module#RoleModule',
                data: { title: 'Role', breadcrumb: 'Role' }
            },
            {
                path: 'configurations',
                canActivateChild: [AuthService],
                loadChildren: './views/configurations/configuration.module#ConfigurationModule',
                data: { title: 'Configurations', breadcrumb: 'Configurations' }
            },
            {
                path: 'permissions',
                canActivateChild: [AuthService],
                loadChildren: './views/permissions/permission.module#PermissionModule',
                data: { title: 'Permission', breadcrumb: 'Permission' }
            },
            {
                path: 'plans',
                canActivateChild: [AuthService],
                loadChildren: './views/plans/plans.module#PlansModule',
                data: { title: 'Plans', breadcrumb: 'Plans' }
            },
            {
                path: 'submenus',
                canActivateChild: [AuthService],
                loadChildren: './views/submenus/submenu.module#SubMenuModule',
                data: { title: 'SubMenu', breadcrumb: 'Menu' }
            },
            {
                path: 'rolepermissions',
                canActivateChild: [AuthService],
                loadChildren: './views/roles/rolepermissions/rolepermission.module#RolePermissionsModule',
                data: { title: 'RolePermissions', breadcrumb: 'RolePermissions' }
            },
            {
                path: 'change_password',
                canActivateChild: [AuthService],
                loadChildren: './views/change-password/change-password.module#ChangePasswordModule',
                data: { title: 'CHANGE PASSWORD', breadcrumb: 'CHANGE PASSWORD' }
            },
            {
                path: '',
                canActivateChild: [AuthService],
                loadChildren: './views/users/users.module#GetUsersModule',
                data: { title: 'Users', breadcrumb: 'USERS' }
            },
            {
                path: 'viewprofile',
                canActivateChild: [AuthService],
                loadChildren: './views/users/users.module#GetUsersModule',
                data: { title: 'Users', breadcrumb: 'USERS' }
            },
            {
                path: 'dashboard',
                canActivateChild: [AuthService],
                loadChildren: './views/dashboard/dashboard.module#DashboardModule',
                data: { title: 'Dashboard', breadcrumb: 'DASHBOARD' }
            },
            {
                path: 'orderreport',
                canActivateChild: [AuthService],
                loadChildren: './views/reports/report.module#ReportModule',
                data: { title: 'Report', breadcrumb: 'Report' }
            },
            {
                path: 'salesreport',
                canActivateChild: [AuthService],
                loadChildren: './views/reports/all-reports/all-reports.module#AllReportModule',
                data: { title: 'AllReport', breadcrumb: 'AllReport' }
            },
            {
                path: 'allreport',
                canActivateChild: [AuthService],
                loadChildren: './views/reports/all-reports/all-reports.module#AllReportModule',
                data: { title: 'AllReport', breadcrumb: 'AllReport' }
            },
            {
                path: 'IngredientCountReport',
                canActivateChild: [AuthService],
                loadChildren: './views/reports/IngradientCountReport/IngradientCountReport.module#IngradientCountReportModule',
                data: { title: 'IngradientCountReport', breadcrumb: 'IngradientCountReport' }
            },
            {
                path: 'cancelorder',
                canActivateChild: [AuthService],
                loadChildren: './views/cancelorder/cancelorder.module#CancelOrderModule',
                data: { title: 'refundorder', breadcrumb: 'refundorder' }
            },
            {
                path: 'pendingorder',
                canActivateChild: [AuthService],
                loadChildren: './views/pendingorder/pendingorder.module#pendingrderModule',
                data: { title: 'pendingorder', breadcrumb: 'pendingorder' }
            },
            {
                path: 'completedorder',
                canActivateChild: [AuthService],
                loadChildren: './views/completedorder/completedorder.module#completedorderModule',
                data: { title: 'completedorder', breadcrumb: 'completedorder' }
            },
            {
                path: 'allorders',	
                canActivateChild: [AuthService],
                loadChildren: './views/allorders/allorders.module#ALLOrdersModule',
                data: { title: 'allorders', breadcrumb: 'allorders' }
            },
            {
                path: 'deliveryorders',	
                canActivateChild: [AuthService],
                loadChildren: './views/deliveryOrders/deliveryorders.module#deliveryordersModule',
                data: { title: 'deliveryorders', breadcrumb: 'deliveryorders' }
            },
            {
                path: 'members',
                canActivateChild:[AuthService],
                loadChildren:'./views/members/member.module#MemberModule',
                data:{ title:'Members', breadcrumb: 'Members'}
            },
            {
                path: 'resetpassword',
                canActivateChild:[AuthService],
                loadChildren:'./views/resetpassword/resetpassword.module#ResetPasswordModule',
                data:{ title:'resetpassword', breadcrumb: 'resetpassword'} 
            },
            {
                path: 'nonmembers',
                canActivateChild:[AuthService],
                loadChildren:'./views/members/member.module#MemberModule',
                data:{ title:'Members', breadcrumb: 'Members'}
            },
            {
                path: 'membersubscription',
                canActivateChild:[AuthService],
                loadChildren:'./views/memberSubscriptions/memberSubscription.module#MemberSubscriptionModule',
                data:{ title:'MemberSubscription', breadcrumb: 'MemberSubscription'}
            },
            {
                path: 'anonymousemail',
                canActivateChild:[AuthService],
                loadChildren: './views/anonymousEmails/anonymousEmail.module#AnonymousemailModule',
                data:{ title:'AnonymousEmailComponent', breadcrumb: 'AnonymousEmailComponent'}
            },
            {
                path: 'careers',
                canActivateChild:[AuthService],
                loadChildren: './views/careers/career.module#CareerModule',
                data:{ title:'CareerComponent', breadcrumb: 'CareerComponent'}
            },
            {
                path: 'contactus',
                canActivateChild:[AuthService],
                loadChildren: './views/contactus/contactus.module#ContactUsModule',
                data:{ title:'ContactUsComponent', breadcrumb: 'ContactUsComponent'}
            },
            {
                path: 'vendorenquiry',
                canActivateChild:[AuthService],
                loadChildren: './views/vendorenquiry/vendorenquiry.module#VendorEnquiryModule',
                data:{ title:'VendorEnquiryComponent', breadcrumb: 'VendorEnquiryComponent'}
            },
            {
                path: 'landingpage',
                canActivateChild:[AuthService],
                loadChildren:'./views/cms-landing-page/cms-landing-page.module#CmsLandingPageModule',
                data:{ title:'CmsLandingPageComponent', breadcrumb: 'CmsLandingPageComponent'}
            },
            {
                path: 'categories',
                canActivateChild:[AuthService],
                loadChildren:'./views/categories/categories.module#CategoriesModule',
                data:{ title:'categories', breadcrumb: 'categories'}
            },
            {
                
                path:'orders',
                canActivate:[AuthService],
                loadChildren:'./views/orders/order.module#OrderModule',
                data:{ title:'orders', breadcrumb: 'orders'}

            },
            {
                path:'sitereviews',
                canActivate:[AuthService],
                loadChildren:'./views/siteReviews/siteReview.module#SiteReviewsModule',
                data:{ title:'SiteReviews', breadcrumb: 'SiteReviews'}

            },
            {
                path:'schedulerDetails',
                canActivate:[AuthService],
                loadChildren:'./views/schedulerDetails/scheduler-details.module#SchedulerDetailsModule',
                data:{ title:'SchedulerDetails', breadcrumb: 'SchedulerDetails'}

            },
            {
                path:'freeMembership',
                canActivate:[AuthService],
                loadChildren:'./views/freeMembership/freeMembership.module#FreeMembershipModule',
                data:{ title:'FreeMembership', breadcrumb: 'FreeMembership'}

            },
            {
                path: 'kpoints',
                canActivateChild:[AuthService],
                loadChildren:'./views/kpoints/kpoints.module#KpointsModule',
                data:{ title:'Kpoints', breadcrumb: 'Kpoints'}
            },
            {
                path: 'planConversion',
                canActivateChild:[AuthService],
                loadChildren:'./views/planConversion/planConversion.module#PlanConversionModule',
                data:{ title:'PlanConversion', breadcrumb: 'PlanConversion'}
            },
            {
                path: 'stores',
                canActivateChild: [AuthService],
                loadChildren: './views/stores/stores.module#StoresModule',
                data: { title: 'Stores', breadcrumb: 'Stores' }
            },
            {
                path: 'storetimings',
                canActivateChild: [AuthService],
                loadChildren: './views/store-timings/store-timings.module#StoreTimingsModule',
                data: { title: 'StoreTimings', breadcrumb: 'StoreTimings' }
            },
            {
                path: 'posusers',
                canActivateChild: [AuthService],
                loadChildren: './views/posusers/posusers.module#GetPosUsersModule',
                data: { title: 'PosUsers', breadcrumb: 'PosUSERS' }
            },
            {
                path: 'paymentoptions',
                canActivateChild: [AuthService],
                loadChildren: './views/payment/payment.module#PaymentModule',
                data: { title: 'Payment', breadcrumb: 'Payment' }
            },
            {
                path: 'corpdomain',
                canActivateChild: [AuthService],
                loadChildren: './views/CorpMaster/corp-master.module#CorpMasterModule',
                data: { title: 'corpdomain', breadcrumb: 'corpdomain' }
            },
            {
                path: 'posOrders',
                canActivateChild: [AuthService],
                loadChildren: './views/pos-order/pos-order.module#POSOrderModule',
                data: { title: 'POSOrder', breadcrumb: 'POSOrder' }
            },
             {
                
                path:'postargets',
                canActivate:[AuthService],
                loadChildren:'./views/pos-targets/pos-targets.module#POSTargetsModule',
                data:{ title:'POSTargets', breadcrumb: 'POSTargets'}

            }
        ]
    },

    {
        path: '**',
        redirectTo: 'sessions/404'
    }
];

