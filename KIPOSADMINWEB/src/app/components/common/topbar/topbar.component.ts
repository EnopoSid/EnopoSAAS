import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { LogoutService } from '../../../services/logout/logout.service';
import {Router} from '@angular/router';
import { AppInfoService } from '../../../services/common/appInfo.service';
import { SendReceiveService } from '../../../services/common/sendReceive.service';
import { AuthService } from '../../../services/auth/auth.service';

import {MatDialog, MatDialogRef} from '@angular/material';

import {UsersService} from '../../../views/users/users.service';
 import{PermissionService} from '../../../views/permissions/permission.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { AppComfirmComponent } from 'src/app/views/app-dialogs/app-confirm/app-confirm.component';
import Utils from 'src/app/services/common/utils';

@Component({
    selector: 'topbar',
    templateUrl: './topbar.component.html'
})
export class TopbarComponent implements OnInit {
    @Input() sidenav;
    @Input() notificPanel;
    @Input() isFromConsumer: boolean = true;
    @Input() x: boolean;
    @Input() isFromAnonymous: boolean;
    @Output() onLangChange = new EventEmitter<any>();
    dialogRef: MatDialogRef<any>;
    public now: Date = new Date();
    rows = [];
    temp = [];
    userId: number = this.sendReceiveService.globalUserId;
    currentLang = 'en';
    text;
    availableLangs = [{
        name: 'English',
        code: 'en',
    }, {
        name: 'Spanish',
        code: 'es',
    }];
    egretThemes;
    userName;
    roleName;

    public avatarDataCircle3: any = {
        size: 50,
        background: '#d1e6b5',
        fontColor: '#972C22',
        border: "0px solid #d3d3d3",
        isSquare: false,
        text: "",
        fixedColor:true
    };


    constructor(private router: Router,
                private  spinner: NgxSpinnerService,
                public logoutService: LogoutService,
                 public appInfoService: AppInfoService,
                 public service: UsersService,
                 public sendReceiveService: SendReceiveService,
                 public permissionservice:PermissionService,
                 public dialog: MatDialog,
                 public authService: AuthService
              ) { 
                setInterval(() => {
                    this.now = new Date();
                  }, 1);
              }

    ngOnInit() {
       
        this.appInfoService.setSecurity();
        let accessToken = JSON.parse(localStorage.getItem('accessToken'));
        if(!!accessToken){
        this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,
            'api/Common/GetTableKeyNameByKeyTypeAndId?type=users&id='+accessToken.id+'', {}).subscribe(userName =>{
                this.authService.setLoggedInUser(userName);

                let user = this.authService.getLoggedInUser();
                this.userName = Utils.capitalizeFirstLetter(user);
                this.text = "Welcome "+  (!!this.userName  ? this.userName.split(' ')[0] : '');
                 this.avatarDataCircle3.text = this.userName;
            });
            
            this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,
                'api/Common/GetTableKeyNameByKeyTypeAndId?type=roles&id='+accessToken.roleId+'', {}).subscribe(roleName =>{
                    this.authService.setLoggedInRole(roleName);

                    let role=this.authService.getLoggedInRole();
        this.roleName=Utils.capitalizeFirstLetter(role);
            });
        }
    }

    logout () {

        this.dialogRef = this.dialog.open(AppComfirmComponent);

        this.dialogRef.componentInstance.message = 'DO_YOU_WANT_TO_LOGOUT.LABEL';

        this.dialogRef.afterClosed().subscribe(result => {
            this.dialogRef = null;

            if (result === true) {
               this.sendReceiveService.logoutService();
               this.appInfoService.setSecurity();
            }
        });
    }
    ViewUser()
    {
         if(!!this.sendReceiveService.globalUserId)
         this.router.navigate(['viewprofile']);
    }
    onSignInClick(){
         this.router.navigate(['sessions/signin']);
    }

    onLogoClick(){
        if(!!this.sendReceiveService.globalUserId)
      this.router.navigate(['dashboard'])
        else
            this.router.navigate(['/']);
    }

    setLang() {
        this.onLangChange.emit(this.currentLang);
    }
    changeTheme(theme) {
    }
    toggleNotific() {
        this.notificPanel.toggle();
    }
    toggleSidenav() {
        this.sidenav.toggle();
    }
    toggleCollapse() {
        let appBody = document.body;
    }

}
