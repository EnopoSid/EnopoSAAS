//Default
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';

//Common Services
import { environment } from '../../../environments/environment';
import MyAppHttp from './myAppHttp.service';
import { SendReceiveService } from './sendReceive.service';

//Custom Services
import { AuthService } from './../auth/auth.service';
import { NgxSpinnerService } from 'ngx-spinner';
import {MatDialog, MatDialogConfig, MatDialogRef} from '@angular/material';
import { AppComfirmComponent } from 'src/app/views/app-dialogs/app-confirm/app-confirm.component';
import { LogoutService } from 'src/app/services/logout/logout.service';




declare var jsSHA: any;
declare var Hmac: any;

@Injectable()
export class AppInfoService {

    hmac: any;
    dialogRef: MatDialogRef<any>;
    constructor(
         private sendReceiveService: SendReceiveService,
        private router: Router,
        private authService: AuthService,
        private logoutService: LogoutService,
        private spinner: NgxSpinnerService,
        public dialog: MatDialog
    ) { }

    setSecurity(): void {
        this.initialize();
        this.sendReceiveService.onInit();
        let inputDataInitialize = {
            modelId: MyAppHttp.REQUESTS.INITIALIZE.modelId,
        };
        let tempRouterUrl = this.router.url;
        let accessToken = JSON.parse(localStorage.getItem('accessToken'));
        if(!!accessToken){

            this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,
            'api/Common/getIsPasswordChangedFlag?id='+accessToken.id+'', {}).subscribe(response =>{
                this.authService.setLoggedInIsPasswordChangedFlag(response.IsPasswordChanged);
            });
            this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,
            'api/Common/GetTableKeyNameByKeyTypeAndId?type=users&id='+accessToken.id+'', {}).subscribe(userName =>{
                this.authService.setLoggedInUser(userName);
            });
            
            this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,
                'api/Common/GetTableKeyNameByKeyTypeAndId?type=roles&id='+accessToken.roleId+'', {}).subscribe(roleName =>{
                    this.authService.setLoggedInRole(roleName);
            });

          
        }
        else if(MyAppHttp.ANONYMOUS_ROUTES.indexOf(tempRouterUrl) > -1){

        }
        else{
            if(tempRouterUrl!="/sessions/signin"){
                this.sendReceiveService.logoutService();
            }
        }
    }

    callback() {

    }
    setHMACKeys(): void {

    }
    setNs(): void {

    }
    setEs(): void {

    }
    setDataPacks(): void {

    }
    destroyAllServices(): void {
        try {
            this.resetAllServices();
            this.sendReceiveService.setSequence(1);
        }
        catch (e) {
        }
    }

    resetAllServices(): void {
        try {
        }
        catch (e) {
        }
    }
    initialize(): void {
        try {
            this.destroyAllServices();
            this.hmac = new Hmac();
        }
        catch (e) {
        }
    }

    gotoSplash() : void {
        this.setSecurity();
        setTimeout (() => {
            this.setSecurity();
           //this.destroyAllServices();
        }, 1000);
    }

    confirmationDialog(){
        this.dialogRef = this.dialog.open(AppComfirmComponent);
        
                this.dialogRef.componentInstance.message = 'Are you sure you want to delete?';
        
               return this.dialogRef.afterClosed().pipe(map(result => {
        
                    this.dialogRef = null;
                    return result;
                }));
                
    }

}
