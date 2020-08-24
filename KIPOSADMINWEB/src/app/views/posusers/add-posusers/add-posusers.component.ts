import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef, Input} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef} from '@angular/material';
import { Router, ActivatedRoute } from '@angular/router';



import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IPageLevelPermissions } from '../../../helpers/common.interface';
import MyAppHttp from '../../../services/common/myAppHttp.service';
import { TopbarComponent } from '../../../components/common/topbar/topbar.component';

import { NgxSpinnerService } from 'ngx-spinner';
import { UsersService } from 'src/app/views/users/users.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';

import * as $ from 'jquery';
import { PosUsersService } from '../posusers.service';

@Component({
    selector: 'add-posuser-table',
    templateUrl: './add-posusers.component.html',
    encapsulation: ViewEncapsulation.None
})

export class GetAddPosUsersComponent implements OnInit {
    UserForm:FormGroup;
    RegionList: any;
   rolesList:any;
    regionId: any;
    pattern = "[a-zA-Z][a-zA-Z ]*";
    title: string = "User";
    saveUser: boolean = false;
    updateUser: boolean = false; 
    checkInput: boolean = false;
    idOnUpdate:number=0;
    storeList : any;
    globalstoreList : any;
    ZoneList:any;
    id: number = 0;
    userId: number = 0;
    isRegionSelected:boolean=false;
    @Input() isFromConsumer: boolean;
    customerGuid;
    POSUserName;

    userFormErrors = {
        firstName: {},
        lastName: {},
        email: {},
        password: {},
        mobilenum: {},
    };
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    isOnView:boolean=false;
    
    constructor(private spinner: NgxSpinnerService,
                private router: Router,
                public service: PosUsersService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public PosUsersService:PosUsersService,
                public sendReceiveService: SendReceiveService,private formBuilder: FormBuilder,
                public translate: TranslateService,
                fb: FormBuilder,  
                private _router: Router,
                private route: ActivatedRoute
            ) {
    }

    ngOnInit() {

        this.id = +this.route.snapshot.params['id'];
        this.userId=this.sendReceiveService.globalUserId;
        
        if(isNaN(this.id))
        {
            this.title = "Add User"
        }
        if(!!this.id){
                if(!!this.service.isUserView)
                {
                    this.title = "Edit User - ";
                }
                else
                {
                    this.title = "Edit User - ";
                }
        }
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
            this.pagePermissions =pageLevelPermissions.response;
            var temproute=this.router.url;
            if(temproute.indexOf('add') > -1) {
                this.UserForm.enable();
                if(!this.pagePermissions.Add)
                    this.sendReceiveService.logoutService();
            }

            if(temproute.indexOf('update') > -1) {
                this.UserForm.enable();
                if(!this.pagePermissions.Edit)
                    this.sendReceiveService.logoutService();
            }

            if(temproute.indexOf('view') > -1) {
                this.UserForm.disable();
                this.isOnView=true;
                if(!this.pagePermissions.View)
                    this.sendReceiveService.logoutService();
            }
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
        this.service.roletypeMaster()
            .subscribe((roles) => {
                this.rolesList = roles;
            })

            this.service.getStoreDetails()
            .subscribe((stores) => {
                this.storeList = stores;
            })
        if(this.isFromConsumer){
            
            this.title="My Profile";
         
            this.UserForm = this.formBuilder.group({
                'id': 0,
                'firstName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
                'lastName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
                'email': [null, Validators.compose([Validators.required])],
                'mobilenum': [null, Validators.compose([Validators.required, Validators.minLength(8),Validators.maxLength(10)])],
                'role': [null, Validators.required],
                 'store':[null, Validators.required],
                'password': [null]
            });
            this.service.getPosUsersListById(this.userId).subscribe(resp => {
                    this.UserForm.patchValue({
                        id: resp.ID,
                        firstName: resp.FirstName,
                        lastName: resp.LastName,
                        email: resp.EmailId,
                        password: resp.Password,
                        mobilenum: resp.MobileNumber,
                      role: resp.RoleId,
                        store:resp.StoreId
                    });
                    this.customerGuid=resp.CustomerGuid;
                    this.POSUserName = resp.FirstName;
                    this.UserForm.disable();
                    
                }); 
                
        }

        else{
            if (!isNaN(this.id)) {
                this.UserForm = this.formBuilder.group({
                    'id': 0,
                    'firstName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
                    'lastName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
                    'email': [null, Validators.compose([Validators.required])],
                    'mobilenum': [null, Validators.compose([Validators.required, Validators.minLength(8),Validators.maxLength(10)])],
                    'role': [null, Validators.required],
                     'store':[null, Validators.required],
                    'password': [null]
                });
                this.idOnUpdate=this.id;
                this.saveUser = false;
                this.updateUser = true;
                this.checkInput = true;
                this.service.getPosUsersListById(this.id).subscribe(resp => {
                        this.UserForm.patchValue({
                            id: resp.ID,
                            firstName: resp.FirstName,
                            lastName: resp.LastName,
                            email: resp.Email,
                            password: resp.Password,
                            mobilenum:resp.MobileNumber,
                           role: resp.RoleId,
                            store:resp.StoreId
                        });
                        this.customerGuid=resp.CustomerGuid;
                        this.POSUserName = resp.FirstName;
                    });
            } else{
                this.UserForm = this.formBuilder.group({
                    id: 0,
                    'firstName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
                    'lastName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
                    'email':  [null, Validators.compose([Validators.required]),this.emailValidation.bind(this)],
                    'password': [null, Validators.compose([Validators.required, Validators.minLength(8)])],
                    'mobilenum': [null, Validators.compose([Validators.required, Validators.minLength(8),Validators.maxLength(10)])],
                   'role': [null, Validators.required],
                    'store':[null, Validators.required],
                });
               this.saveUser = true;
                this.updateUser = false;
                this.checkInput = false;
            }
        }

        this.service.roletypeMaster().subscribe((roles) => {
                this.rolesList = roles; 
            })
            this.service.getStoreDetails().subscribe((stores) => {
                this.storeList = stores; 
            })
        if (this.service.isUserView) {
            this.UserForm.disable();
            this.isOnView=true;
        }
        else{
            this.isOnView=false;
        }
    }
  
    onUserFormSubmit() {
          document.getElementById('preloader-div').style.display = 'block';
        let var_id: number = this.UserForm.value.id;
        let var_firstName: string = this.UserForm.value.firstName;
        let var_lastName: string = this.UserForm.value.lastName;
        let var_email: string = this.UserForm.value.email;
        let var_password: string = this.UserForm.value.password;
        let var_mobilenumber: number = this.UserForm.value.mobilenum;
       let var_role: number = this.UserForm.value.role;
        let var_store = this.UserForm.value.store;
    
        if (this.saveUser) {
            if (!this.UserForm.valid) {
                return;
            }
            this.service.AddPosUserNOP({
            "UserName":var_firstName,
    	    "EmailId": var_email ,
    	    "MobileNumber":var_mobilenumber,
    	    "Password": var_password,
            "FirstName":var_firstName,
            "LastName":var_lastName,
           "CustomerGUID":"00000000-0000-0000-0000-000000000000"
                  }).subscribe((data) => {
                    this.service.savePosUsersList({
                        'FirstName': var_firstName,
                        "Email":var_email,
                        "MobileNumber":var_mobilenumber,
                       "Password":var_password,
                       "CustomerGuid":data.CustomerGuid,
                       "CustomerId":data.CustomerId,
                      "RoleId": var_role,
                       "IsPasswordChanged" :false,
                       'LastName': var_lastName,
                       "StoreId" :var_store
                          }).subscribe((response) => {
                        this._router.navigate(['../'],{relativeTo: this.route}); 
                        if (this.UserForm.valid) {
                            this.UserForm.reset();
                        }
                         document.getElementById('preloader-div').style.display = 'none';;
                      }, 
                      error => {
                          this.userFormErrors = error;
                           document.getElementById('preloader-div').style.display = 'none';;
                    }
                     
                    )
              }, 
              error =>{
                this.userFormErrors = error;
               var valiadationMessage =   error.error.ErrorMessage.replace("IsValidRegistration = false","");
               alert(valiadationMessage);
                document.getElementById('preloader-div').style.display = 'none';;
               } 
            )
        }
        else if (this.updateUser) {
            this.idOnUpdate=0;
            this.service.editPosUserInNOP({
                "CustomerGUID":this.customerGuid,
                "FirstName": var_firstName,
                "LastName": var_lastName,
                "Email": var_email,
                "MobileNumber":var_mobilenumber
                 })
                .subscribe((data) => {
                    this.service.editPosUserList({
                        'ModifiedBy': this.userId,
                        'CreatedBy': this.userId,
                        'Email': var_email,
                        'ID': var_id,
                        'FirstName': var_firstName,
                        'LastName': var_lastName,
                        'Password': var_password,
                       'RoleId': var_role,
                        "StoreId" :var_store,
                       "MobileNumber":var_mobilenumber,
                         'IsActive':MyAppHttp.ACTIVESTATUS,
                         }, var_id)
                        .subscribe((response) => {
                            this._router.navigate(['/posusers']); 
                             document.getElementById('preloader-div').style.display = 'none';;
                        }, error => {this.userFormErrors = error;
                             document.getElementById('preloader-div').style.display = 'none';;
                        })   
                }, error => {this.userFormErrors = error;
                     document.getElementById('preloader-div').style.display = 'none';;})  
        }
    
}
onGoBack() {
    if(this.isFromConsumer)
    {
        this.router.navigate(['/dashboard']);
    }
    else{
    this.idOnUpdate=0;
    this._router.navigate(['/posusers']); 
    }
}

emailValidation() {
    let var_email: string = this.UserForm.value.email;
    const q = new Promise((resolve, reject) => {
        this.service.duplicateEmail({
            'ModifiedBy': null,
            'CreatedBy': null,
            'EmailId':this.UserForm.controls['email'].value,
            'FirstName': null,
            'LastName':null,
            'PhoneNumber':null,
            'Password':null,
            'RoleId': null,
            'IsActive':MyAppHttp.ACTIVESTATUS,
        }).subscribe((duplicate) => {
            if (duplicate) {
                resolve({ 'emailValidation': true });

            } else {
                resolve(null);
            }
        }, () => { resolve({ 'emailValidation': true }); });
    });
    return q;

}
}