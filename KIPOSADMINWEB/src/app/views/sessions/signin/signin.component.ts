import {Component, OnInit, ViewChild, ViewEncapsulation} from '@angular/core';
import {MatProgressBar, MatButton} from '@angular/material';
import {CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router} from '@angular/router';
import {AuthService} from './../../../services/auth/auth.service';
import {MatDialog, MatDialogConfig, MatDialogRef} from '@angular/material';
import {TranslateService, TranslateModule} from '@ngx-translate/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {FormControl} from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import {LogoutService} from './../../../services/logout/logout.service';
import {AppInfoService} from './../../../services/common/appInfo.service';

@Component({
    selector: 'app-signin',
    templateUrl: './signin.component.html',
    styleUrls: ['./signin.component.css'],
    encapsulation: ViewEncapsulation.None
})

export class SigninComponent implements OnInit {
    @ViewChild(MatProgressBar, {static: false}) progressBar: MatProgressBar;
    @ViewChild(MatButton, {static: false}) submitButton: MatButton;
    public submitted: boolean = false;
    showErrorMsg = false;

    dialogRef: MatDialogRef<any>;

    signinData = {
        username: '',
        password: ''
    };

    signInForm: FormGroup;

    validation_messages = {
        'usernameFC': [
            {type: 'required', message: 'LOGIN.ERRORS.USERNAME.BLANK'},
            {type: 'pattern', message: 'LOGIN.ERRORS.USERNAME.PATTERN'},
            {type: 'minlength', message: 'LOGIN.ERRORS.USERNAME.MIN_LENGTH'},
            {type: 'maxlength', message: 'LOGIN.ERRORS.USERNAME.MAX_LENGTH'}
        ],
        'passwordFC': [
            {type: 'required', message: 'LOGIN.ERRORS.PASSWORD.BLANK'}
        ],
    };

    constructor(private router: Router,
                private spinner: NgxSpinnerService,
                public auth: AuthService,
                private formBuilder: FormBuilder,
                public translate: TranslateService,
                public dialog: MatDialog,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public sendReceiveService: SendReceiveService
            ) {


        this.signInForm = formBuilder.group({
            'usernameFC': ["", Validators.compose([Validators.required, Validators.maxLength(50), Validators.pattern(/^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$/)])],
            'passwordFC': ["", Validators.compose([Validators.required, Validators.maxLength(20), Validators.pattern(/^([a-zA-Z0-9#$@!%&*_.]+)$/)])]
        });
    }

    ngOnInit() {
    }

    signin() {
        document.getElementById('preloader-div').style.display = 'block';
        if (this.signInForm.invalid) {
            document.getElementById('preloader-div').style.display = 'none';
            this.submitted = true;
            this.showErrorMsg = false;
        } else {
            this.submitted = false;
            const passwordInClear = this.signinData.password;

            this.auth.authenticate(this.signinData.username, passwordInClear).subscribe((response) => {
                    this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,
                        'api/Common/GetTableKeyNameByKeyTypeAndId?type=users&id='+response.id+'', {}).subscribe(userName =>{
                            this.auth.setLoggedInUser(userName);
                        });
                        this.auth.setLoggedInUser(response.email);

                        this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,
                            'api/Common/GetTableKeyNameByKeyTypeAndId?type=roles&id='+response.roleId+'', {}).subscribe(roleName =>{
                                this.auth.setLoggedInRole(roleName);
                        });
                                this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,
                                    'api/Common/getIsPasswordChangedFlag?id='+response.id+'', {}).subscribe(res =>{
                                        this.auth.setLoggedInIsPasswordChangedFlag(res.IsPasswordChanged);
                                    });
                         
                        
                              document.getElementById('preloader-div').style.display = 'none';
                }, (error) => {
                      document.getElementById('preloader-div').style.display = 'none';
                    alert("Invalid Credentials");
                }, () => {

                }
            );
        }
    }
   

    actionAfterError() {
        this.dialogRef.afterClosed().subscribe(result => {
            this.appInfoService.setSecurity();
            this.sendReceiveService.setSequence(1);
            this.router.navigate(['/sessions/signin']);
        });
    }

}
