import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef} from '@angular/material';
import {ChangePasswordService} from './change-password.service';
import {Router} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { error } from 'util';



@Component({
    selector: 'change-password-table',
    templateUrl: './change-password.component.html',
    styleUrls: ['./change-password.component.css']
})

export class ChangePasswordComponent implements OnInit {
    @ViewChild('myTable', {static: false}) table: any;
    @ViewChild('tableWrapper', {static: false}) tableWrapper;
    public submitted: boolean = false;
    errorMessage = '';
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    showErrorMsg = false;

    changePasswordData:any = {
        oldPassword: '',
        newPassword: '',
        confirmNewPassword: '',
        userId:0
    };

    changePasswordForm: FormGroup;

    validation_messages = {
        'oldPasswordFC': [
            {type: 'required', message: 'CHANGE_PASSWORD.ERRORS.OLDPASSWORD.BLANK'}
        ],
        'newPasswordFC': [
            {type: 'required', message: 'CHANGE_PASSWORD.ERRORS.NEWPASSWORD.BLANK'},
            {type: 'pattern', message: 'CHANGE_PASSWORD.ERRORS.NEWPASSWORD.PATTERN'},
            {type: 'minlength', message: 'CHANGE_PASSWORD.ERRORS.NEWPASSWORD.MIN_LENGTH'},
            {type: 'maxlength', message: 'CHANGE_PASSWORD.ERRORS.NEWPASSWORD.MAX_LENGTH'}
        ],
        'confirmNewPasswordFC': [
            {type: 'required', message: 'CHANGE_PASSWORD.ERRORS.CONFIRMPASSWORD.BLANK'},
            {type: 'pattern', message: 'CHANGE_PASSWORD.ERRORS.CONFIRMPASSWORD.PATTERN'},
            {type: 'minlength', message: 'CHANGE_PASSWORD.ERRORS.CONFIRMPASSWORD.MIN_LENGTH'},
            {type: 'maxlength', message: 'CHANGE_PASSWORD.ERRORS.CONFIRMPASSWORD.MAX_LENGTH'}
        ],
    };

    constructor(
        private router: Router,
                private spinner: NgxSpinnerService,
                public changePwdSvc: ChangePasswordService,
                private formBuilder: FormBuilder,
                public dialog: MatDialog,
                public appInfoService: AppInfoService,
                public sendReceiveService: SendReceiveService,
                public translate: TranslateService
            ) {
    }

    ngOnInit() {
        this.changePasswordForm =this.formBuilder.group({
            'oldPassword': [null,(Validators.required)],
            'newPassword':[null,(Validators.required,Validators.minLength(8),Validators.maxLength(20),Validators.pattern(/^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d][A-Za-z\d!@#$%^&*()_+]{8,15}$/))],
            'confirmNewPassword': ["", Validators.compose([Validators.required, Validators.minLength(8), Validators.maxLength(20), Validators.pattern(/^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d][A-Za-z\d!@#$%^&*()_+]{8,15}$/)])]
        });
    }

    handlePageChange(event: any): void {
    }

    actionAfterError() {
        this.dialogRef.afterClosed().subscribe(result => {
            this.appInfoService.setSecurity();
            this.sendReceiveService.setSequence(1);
            this.router.navigate(['/sessions/signin']);
        });
    }

    changePassword() {         
        this.changePasswordData.oldPassword=this.changePasswordForm.value.oldPassword;
            this.changePasswordData.newPassword=this.changePasswordForm.value.newPassword;
            this.changePasswordData.confirmNewPassword=this.changePasswordForm.value.confirmNewPassword;
            this.changePasswordData.userId=this.sendReceiveService.globalUserId;

        if (this.changePasswordForm.invalid) {
            this.submitted = true;
            this.showErrorMsg = false;
        }else if(this.changePasswordData.newPassword!==this.changePasswordData.confirmNewPassword){
            alert('the new password and confirmed password must be same');
        }else {
            this.submitted = false;
            this.changePwdSvc.changePassword(this.changePasswordData).subscribe((response) => {
                if(response){
                    this.sendReceiveService.showDialog('Your Password Has been Changed Successfully. Please Re-Login');
                    this.router.navigate(['/sessions/signin']);
                }
            },(error) => {
                
            });

        }
    }
}
