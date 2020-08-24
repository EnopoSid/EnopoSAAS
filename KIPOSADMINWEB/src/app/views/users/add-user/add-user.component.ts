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

@Component({
    selector: 'add-user-table',
    templateUrl: './add-user.component.html',
    encapsulation: ViewEncapsulation.None
})

export class GetAddUsersComponent implements OnInit {
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
    departmentList : any;
    globalDepartmentList : any;
    ZoneList:any;
    id: number = 0;
    isRoleSelected:boolean = false;
    userId: number = 0;
    isRegionSelected:boolean=false;
    storeList : any;
    @Input() isFromConsumer: boolean;

    userFormErrors = {
        firstName: {},
        lastName: {},
        postalAddress: {},
        email: {},
        password: {},
        mobilenum: {},
        telephoneNum: {},
    };
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    isOnView:boolean=false;
    
    constructor(private spinner: NgxSpinnerService,
                private router: Router,
                public service: UsersService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public UsersService:UsersService,
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
            this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
                if(!!this.service.isUserView)
                {
                    this.title = "View User - " +  resp;
                }
                else
                {
                    this.title = "Edit User - " +  resp;
                }
         })
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
                this.rolesList =  roles.filter(function (e) { return e.RoleName != 'pos MANAGER' || e.RoleName == 'POSUSer'});
            })
            this.service.getStoreDetails().subscribe((stores) => {
                this.storeList = stores; 
                this.allstores();
            })

        if(this.isFromConsumer){
            
            this.title="My Profile";
         
            this.UserForm = this.formBuilder.group({
                'id': 0,
                'firstName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
                'lastName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
                'postalAddress': [null, Validators.required],
                'email': [null, Validators.compose([Validators.required])],
                'mobilenum':[null,Validators.compose([Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.singapore_MobileNum)])],
                 'role': [null, Validators.required],
                 'store':[null, Validators.required],
                'password': [null]
            });
            this.service.getUsersListById(this.userId).subscribe(resp => {
                    this.UserForm.patchValue({
                        id: resp.UserId,
                        firstName: resp.FirstName,
                        lastName: resp.LastName,
                        email: resp.EmailId,
                        postalAddress: resp.PostalAddress,
                        password: resp.Password,
                        mobilenum: resp.PhoneNumber,
                         region:1,
                        role: resp.RoleId,
                         department:1,
                        zone: 1,
                        store:resp.StoreId
                    });
                    this.UserForm.disable();
                    
                }); 
                
        }

        else{
            if (!isNaN(this.id)) {
                this.UserForm = this.formBuilder.group({
                    'id': 0,
                    'firstName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
                    'lastName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
                    'postalAddress': [null, Validators.required],
                    'email': [null, Validators.compose([Validators.required])],
                    'mobilenum': [null,Validators.compose([Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.singapore_MobileNum)])],
                     'role': [null, Validators.required],
                     'store':[null, Validators.required],
                    'password': [null]
                });
                this.idOnUpdate=this.id;
                this.saveUser = false;
                this.updateUser = true;
                this.checkInput = true;
                this.service.getUsersListById(this.id).subscribe(resp => {
                        this.UserForm.patchValue({
                            id: resp.UserId,
                            firstName: resp.FirstName,
                            lastName: resp.LastName,
                            email: resp.EmailId,
                            postalAddress: resp.PostalAddress,
                            password: resp.Password,
                            mobilenum:resp.PhoneNumber,
                            region:1,
                             role: resp.RoleId,
                            department:1,
                            zone: 1,
                            store:resp.StoreId
                        });
                    });
            } else{
                this.UserForm = this.formBuilder.group({
                    id: 0,
                    'firstName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
                    'lastName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
                    'postalAddress': [null, Validators.required],
                    'email':  [null, Validators.compose([Validators.required]),this.emailValidation.bind(this)],
                    'password': [null, Validators.compose([Validators.required, Validators.minLength(8)])],
                    'mobilenum':[null,Validators.compose([Validators.required,Validators.minLength(8),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.singapore_MobileNum)])],
                    'store':[null, Validators.required],
                    'role': [null, Validators.required],
                });
               this.saveUser = true;
                this.updateUser = false;
                this.checkInput = false;
            }
        }

        this.service.roletypeMaster().subscribe((roles) => {
                this.rolesList =  roles.filter(function (e) { return e.RoleName != 'pos MANAGER' && e.RoleName != 'POSUSer'});
            })
            this.service.getStoreDetails()
            .subscribe((stores) => {
                this.storeList = stores;
                this.allstores();

            })
      
        if (this.service.isUserView) {
            this.UserForm.disable();
            this.isOnView=true;
        }
        else{
            this.isOnView=false;
        }
    }

allstores(){
     var storesList =[];

    let storeeee={
        AddressId: 10,
CCAddress: "mail1@srijaytech.com,mail2@srijaytech.com,mail3@srijaytech.com",
ContactAddress: "phanendra.surathu@srijaytech.com",
ContactMobile: "9618452526",
Description: null,
DisplayOrder: 0,
Id: 0,
Name: "All Stores",
OpeningHours: "9.00 - 9.00",
PickupFee: 0,
StoreId: 0,
StorePickupPoint1: "1"

    }
   
   storesList=this.storeList;
   storesList.push(storeeee);
}



    onChangeRegion(selectedRegionId){
        this.isRegionSelected=true;
        this.regionId=selectedRegionId;
        this.service.getZonesByRegionId(this.regionId).subscribe((zone)=>{
          this.ZoneList=zone;
        })
    }

    onChangeRole(selectedRoleId){
        this.isRoleSelected = true;
        
        if (this.service.isUserView) {
            this.UserForm.disable();
        }
   
    }
 handlePageChange (event: any): void {

    }

    openResume(filePath) {
        window.open(filePath);
    }
  
    onUserFormSubmit() {
        let var_id: number = this.UserForm.value.id;
        let var_firstName: string = this.UserForm.value.firstName;
        let var_lastName: string = this.UserForm.value.lastName;
        let var_email: string = this.UserForm.value.email;
        let var_password: string = this.UserForm.value.password;
        let var_mobilenumber: number = this.UserForm.value.mobilenum;
        let var_postalAddress: number = this.UserForm.value.postalAddress;
        let var_store = this.UserForm.value.store;
        let var_role: number = this.UserForm.value.role;
        if (this.saveUser) {
            if (!this.UserForm.valid) {
                return;
            }
            this.service.saveUsersList({
                'ModifiedBy': this.userId,
                'CreatedBy': this.userId,
                'EmailId': var_email,
                'FirstName': var_firstName,
                'PostalAddress': var_postalAddress,
                'LastName': var_lastName,
                'PhoneNumber': var_mobilenumber,
                'Password': var_password,
                'RoleId': var_role,
                "StoreId" :var_store,
                'IsActive':MyAppHttp.ACTIVESTATUS,
                  }).subscribe((data) => {
                this._router.navigate(['../'],{relativeTo: this.route}); 
                if (this.UserForm.valid) {
                    this.UserForm.reset();
                }
              }, 
              error => this.userFormErrors = error
            )
        }
        else if (this.updateUser) {
            this.idOnUpdate=0;
            this.service.editUserList({
                'ModifiedBy': this.userId,
                'CreatedBy': this.userId,
                'EmailId': var_email,
                'UserId': var_id,
                'FirstName': var_firstName,
                'LastName': var_lastName,
                'PostalAddress': var_postalAddress,
                'Password': var_password,
                'RoleId': var_role,
                "StoreId" :var_store,
                'PhoneNumber': var_mobilenumber,
                 'IsActive':MyAppHttp.ACTIVESTATUS,
                 }, var_id)
                .subscribe((data) => {
                    this._router.navigate(['/users']); 
                }, error => this.userFormErrors = error)   
        }
    
}
onGoBack() {
    if(this.isFromConsumer)
    {
        this.router.navigate(['/dashboard']);
    }
    else{
    this.idOnUpdate=0;
    this._router.navigate(['/users']); 
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
            'PostalAddress':null,
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
countrycode(event: any){  
    let num = '' ;
    if( this.UserForm.value.mobilenum.length < 2){
        num =  this.UserForm.value.mobilenum;
    }
    else {
        if(this.UserForm.value.mobilenum.indexOf('91') == 0  && this.UserForm.value.mobilenum.length > 2){
          num =  this.UserForm.value.mobilenum.substr(2,this.UserForm.value.mobilenum.length);
        }
    }
    let mobile = this.UsersService.appendcountrycode(event,num);
 if(!num)
 {
     this.UserForm.patchValue({
        mobilenum : "",
      }); 
 }else{
    this.UserForm.patchValue({
        mobilenum :mobile,
      }); 
 }


}
countryscode(event: any){
    let num = '' ;
    if( this.UserForm.value.telephonenum.length < 2){
        num =  this.UserForm.value.telephonenum;
    }
    else {
        if(this.UserForm.value.telephonenum.indexOf('91') == 0  && this.UserForm.value.telephonenum.length > 2){
          num =  this.UserForm.value.telephonenum.substr(2,this.UserForm.value.telephonenum.length);
        }
    }
    let mobile = this.UsersService.appendcountrycode(event,num);
 if(!num)
 {
     this.UserForm.patchValue({
        telephonenum : "",
      }); 
 }else{
    this.UserForm.patchValue({
        telephonenum :mobile,
      }); 
 }
   }

encodePhoneNumber(phoneNumber: string)
{
    if(phoneNumber != null){
    let phonenumber =  this.UsersService.encodePhoneNumber(phoneNumber);
    return phonenumber;
    }
}
decodePhoneNumber(phoneNumber: string){
    if(phoneNumber != null){
    let phonenumber =  this.UsersService.decodePhoenNumber(phoneNumber);
    return phonenumber;
    }
}
_keyPress(event: any) {
    const pattern = /[0-9]/;
    let inputChar = String.fromCharCode(event.charCode);
    if (!pattern.test(inputChar)) {
        event.preventDefault();

    }
}
}