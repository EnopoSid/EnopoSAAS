import {Component, OnInit, ViewChild, ViewEncapsulation} from '@angular/core';
import {MatProgressBar, MatButton} from '@angular/material';
import {CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router} from '@angular/router';
import {AuthService} from './../../../services/auth/auth.service';
import {MatDialog, MatDialogConfig, MatDialogRef} from '@angular/material';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {FormControl} from '@angular/forms';
import {LogoutService} from './../../../services/logout/logout.service';
import {AppInfoService} from './../../../services/common/appInfo.service';
import {SendReceiveService} from './../../../services/common/sendReceive.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-forgot-password',
    templateUrl: './forgot-password.component.html',
    styleUrls: ['./forgot-password.component.css'],
    encapsulation: ViewEncapsulation.None
})
export class ForgotPasswordComponent implements OnInit {
    userEmail;
    @ViewChild(MatButton, {static: false}) submitButton: MatButton;
    public submitted: boolean = false;
    showErrorMsg = false;
    errorMessage = '';
    dialogRef: MatDialogRef<any>;
    forgotPasswordForm: FormGroup;

    validation_messages = {
        'userEmailFC': [
            {type: 'required', message: 'LOGIN.ERRORS.USERNAME.BLANK'},
            {type: 'pattern', message: 'LOGIN.ERRORS.USERNAME.PATTERN'},
            {type: 'minlength', message: 'LOGIN.ERRORS.USERNAME.MIN_LENGTH'},
            {type: 'maxlength', message: 'LOGIN.ERRORS.USERNAME.MAX_LENGTH'}
        ]
    };

    constructor(private router: Router,
                private spinner: NgxSpinnerService,
                public auth: AuthService,
                private formBuilder: FormBuilder,
                public translate: TranslateService,
                public dialog: MatDialog,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public sendReceiveService: SendReceiveService) {

        this.forgotPasswordForm = formBuilder.group({
            'userEmailFC': ["", Validators.compose([Validators.required,  Validators.maxLength(50), Validators.pattern(/^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$/)])]
        });
    }

    ngOnInit() {
    }

    actionAfterError() {
        this.dialogRef.afterClosed().subscribe(result => {
            this.appInfoService.setSecurity();
            this.sendReceiveService.setSequence(1);
            this.router.navigate(['/sessions/signin']);
        });
    }

    submitEmailToGetPWd(){
        this.submitButton.disabled = true;
        if (this.forgotPasswordForm.invalid) {
                   this.submitted = true;
                    this.showErrorMsg = false;
               } else {
                this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/ForgotPassword/ValidateEmail?EmailId='+this.userEmail+'',{}).subscribe((resp)=>{
                    if(!!resp){
                        this.submitted = false;
                        this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/ForgotPassword/Post?Email='+this.userEmail+'',{}).subscribe((success)=>{
                            if(!!success){
                                this.sendReceiveService.showDialog("Your Request Has been Sent Successfully ")
                                this.router.navigate(['/sessions/signin']);
                            }else{
                                this.sendReceiveService.showDialog("Please enter a registered and valid email");
                            }
                        });
                    }else{
                        this.sendReceiveService.showDialog("Please enter a registered and valid email");
                    }
                })

               }
    }

}
