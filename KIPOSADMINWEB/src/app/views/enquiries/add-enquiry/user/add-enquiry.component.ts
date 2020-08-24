import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef, Input} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { EnquiryService } from '../../enquiry.service';
import * as $ from 'jquery';
import {LowerCasePipe} from '@angular/common';
import { IPageLevelPermissions, EnquiryModel } from '../../../../helpers/common.interface';
import MyAppHttp from '../../../../services/common/myAppHttp.service';
import { UsersService } from '../../../users/users.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';


@Component({
    selector: 'user-add-enquiry-component',
    templateUrl: './add-enquiry.component.html',
    styleUrls: ['./add-enquiry.component.css'],
    encapsulation: ViewEncapsulation.None
})

export class UserAddEnquiryComponent implements OnInit {
    displayedColumns = ['sno','FileName','Actions']
    dataSource: MatTableDataSource<EnquiryModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    regionId: any;
    EnquiryForm: FormGroup;
    title: string = "User";
    saveUser: boolean = false;
    updateUser: boolean = false;
    checkInput: boolean = false;
    rows = [];
    columns = [];
    temp = [];
    formStage:number=1;
    id: number = 0;
    userId: number = 0;
    formErrors = {
     firstName: {},
        lastName: {},
        postalAddress: {},
        residentialAdd:{},
        email: {},
        mobilenum: {},
        alternatenum: {},
        EnquiryDetails:{}   
    };
    isOnView=false;
    selectedFiles: FileList;
    selectedComplaintFiles :any = [];
    @Input() isFromConsumer: boolean;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    isVerified: boolean = false;
    enquriyNumber;

    constructor(private spinner: NgxSpinnerService,
                private router: Router,
                public service: EnquiryService,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                private UsersService:UsersService,
                public sendReceiveService: SendReceiveService,private formBuilder: FormBuilder,
                public translate: TranslateService,fb: FormBuilder,  private _router: Router,private route: ActivatedRoute,) {
    }
    ngOnInit() {
          document.getElementById('preloader-div').style.display = 'block';
        this.userId=this.sendReceiveService.globalUserId;
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'FileName',"Value":" "},
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
        this.id=+this.route.snapshot.params['id'];
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
            this.pagePermissions =pageLevelPermissions.response;

            if(!this.isFromConsumer){
                var temproute=this._router.url;
                if(temproute.indexOf('add') > -1) {
                    this.EnquiryForm.enable();
                    if(!this.pagePermissions.Add)
                        this.sendReceiveService.logoutService();
                }
    
                if(temproute.indexOf('update') > -1) {
                    this.EnquiryForm.enable();
                    if(!this.pagePermissions.Edit)
                        this.sendReceiveService.logoutService();
                }
    
                if(temproute.indexOf('view') > -1) {
                    this.EnquiryForm.disable();
                    this.isOnView=true;
                    if(!this.pagePermissions.View)
                        this.sendReceiveService.logoutService();
                }
            }
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
      
        if(!isNaN(this.id)){
            this.EnquiryForm = this.formBuilder.group({
                id: 0,
                'firstName': [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
                'lastName': [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
                'email':  [null, Validators.compose([Validators.required])],
                'mobilenum': [null, Validators.compose([Validators.required, Validators.minLength(12),Validators.maxLength(12)])],
                'alternatenum': [null, Validators.compose([ Validators.minLength(12),Validators.maxLength(12)])],
                'postalAddress':[null,Validators.required],
                'residentialAddress':[null],
               'EnquiryDetails':[null,Validators.required],
              'File':[null],
            }); 
            this.ProcessEditAction(this.id);
            this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
                this.title ="View Enquiry Form  - " + resp;
               });
            
        }else{
            this.EnquiryForm = this.formBuilder.group({
                id: 0,
                'firstName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
                'lastName': [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
                'email':  [null, Validators.compose([Validators.required])],
                'mobilenum': [null, Validators.compose([Validators.required, Validators.minLength(12),Validators.maxLength(12)])],
                'alternatenum': [null, Validators.compose([ Validators.minLength(12),Validators.maxLength(12)])],
                'postalAddress':[null,Validators.required],
                'residentialAddress':[null],
               'EnquiryDetails':[null,Validators.required],
               'File':[null],

        });
        this.title = "Add  Enquiry Form";
        }
         document.getElementById('preloader-div').style.display = 'none';;
    }
    keyPress(event: any) {
        const pattern = /[0-9\+\-\ ]/;
        let inputChar = String.fromCharCode(event.charCode);
        if (event.keyCode != 8 && !pattern.test(inputChar)) {
          event.preventDefault();
        }
    }
    ProcessEditAction(id){
        this.service.getEnquiryListById(id).subscribe(resp=>{
            this.isOnView=true;
            this.EnquiryForm.patchValue({
                firstName:resp.FirstName,
                lastName:resp.LastName,
                email:resp.EmailId,
                mobilenum:this.decodePhoneNumber(resp.PhoneNumber),
                alternatenum:this.decodePhoneNumber(resp.TelePhoneNumber),
                postalAddress:resp.PostalAddress,
                residentialAddress:resp.ResidentialAddress,
                EnquiryDetails:resp.EnquiryDetails,
            });
            this.fileArray(resp.Files);
             this.EnquiryForm.disable();
        },error=>this.formErrors=error);
    }
   
    onEnquiryFormSubmit(){
        this.EnquiryForm.enable();
        let var_id: number = this.EnquiryForm.value.id;
        let var_firstName: string = this.EnquiryForm.value.firstName;
        let var_lastName: string = this.EnquiryForm.value.lastName;
        let var_email: string = this.EnquiryForm.value.email;
        let var_mobilenumber: number = this.encodePhoneNumber(this.EnquiryForm.value.mobilenum);
        let var_alternatenumber: number = this.encodePhoneNumber(this.EnquiryForm.value.alternatenum);
        let var_postalAddress = this.EnquiryForm.value.postalAddress;
        let var_residentialAddress = this.EnquiryForm.value.residentialAddress;
        let var_EnquiryDetails = this.EnquiryForm.value.EnquiryDetails;
        let FileUpload = this.selectedFiles;
        let tempLoginUserId = !!this.userId ? this.userId : 0;
       var userData = {      
        FirstName: var_firstName,
        LastName: var_lastName, 
        EmailId: var_email,
        PhoneNumber: var_mobilenumber,
        PostalAddress: var_postalAddress, 
         TelePhoneNumber:var_alternatenumber,     
         ResidentialAddress:var_residentialAddress    }
         if (!this.EnquiryForm.valid) {
          return;
          }
            document.getElementById('preloader-div').style.display = 'block';
        if(this.isFromConsumer==true){
            this.service.saveConsumerEnquiry({
                'ModifiedBy': tempLoginUserId,
                'CreatedBy':tempLoginUserId,
                'IpAddress' : "192.168.0.10",
                 'EnquiryDetails'  : var_EnquiryDetails,
                 'IsActive'  :  1,
                   'User': userData
                },FileUpload,).subscribe((data) => {
                 if (this.EnquiryForm.valid) {
                    this.EnquiryForm.reset();
                }
                this.enquriyNumber=data.EnquiryNum;
                this._router.navigate(['/']);
                this.sendReceiveService.showDialog("Enquiry with Enquiry Number "+this.enquriyNumber +" has been registered successfully");
                 document.getElementById('preloader-div').style.display = 'none';;
            }, error =>{
                this.formErrors = error
                 document.getElementById('preloader-div').style.display = 'none'; 
            } 
        )   
        }
        else{
            this.service.saveEnquiryList({
                'ModifiedBy': tempLoginUserId,
                'CreatedBy': tempLoginUserId,
                'IpAddress' : "192.168.0.10",
                 'EnquiryDetails'  : var_EnquiryDetails,
                 'IsActive'  :  1,
                   'User': userData,
                   'Isincall': this.sendReceiveService.isInCall,
                   'IncomingPhoneNumber': this.sendReceiveService.globalPhoneNumber,
                },FileUpload,).subscribe((data) => {
                 if (this.EnquiryForm.valid) {
                    this.EnquiryForm.reset();
                }
                this.enquriyNumber=data.EnquiryNum;
                this._router.navigate(['../'],{relativeTo: this.route}); 
                this.sendReceiveService.showDialog("Enquiry with Enquiry Number "+this.enquriyNumber +" has been registered successfully");
                document.getElementById('preloader-div').style.display = 'none';;
            }, error => {
                this.formErrors = error;
                 document.getElementById('preloader-div').style.display = 'none';;
            });
        }
      }
        
        onGoBack(){
            if(this.isFromConsumer==true){
                this._router.navigate(['/']);
            }else
            this._router.navigate(['/enquiries']);
        }
        onSave(){
            if(this.EnquiryForm.invalid){
               
                $('input.ng-invalid Mat-select.ng-invalid').first().focus();
                return false;
            }
      this.EnquiryForm.disable();
      this.formStage=2;
      }
      onEditClick(){
        this.EnquiryForm.enable();
        this.formStage =1 ;
      }
      selectFile(event) {
        this.selectedFiles = event.target.files;
      }
      resolved(captchaResponse: string) {
        this.isVerified = true;
    }

      fileArray(Files : any)
      {
          let Sno = 1; 
          for(let i = 0;i<Files.length;i++){ 
              var FileTemp: any= {}; 
              FileTemp.FileId  = Sno;
              FileTemp.Id = Files[i].FileId,
              FileTemp.FilePath  = Files[i].FilePath;
              FileTemp.FileName  = Files[i].FilePath.substring(Files[i].FilePath.lastIndexOf('\\')+1).split('.')[0];
              this.selectedComplaintFiles.push(FileTemp)
                Sno++;
          }   
            this.filterData.gridData = this.selectedComplaintFiles;
              this.dataSource = new MatTableDataSource(this.selectedComplaintFiles);
              this.filterData.dataSource=this.dataSource;
              this.dataSource.paginator = this.paginator;
              this.dataSource.sort = this.sort;
      }
  
      downloadImage(downloadLink) {
          this.service.downloadFile(downloadLink.Id)
          .subscribe(data => {
          var link = document.createElement('a');
          link.download =  downloadLink.FilePath.substring(downloadLink.FilePath.lastIndexOf('\\')+1) || LowerCasePipe;
          link.href = ('data:application/octet-stream;base64,' + data);
          link.click();
      })
      }
      encodePhoneNumber(phoneNumber: string)
      {
          let phonenumber =  this.UsersService.encodePhoneNumber(phoneNumber);
          return phonenumber;
      }
      decodePhoneNumber(phoneNumber: string){
        let phonenumber =  this.UsersService.decodePhoenNumber(phoneNumber);
         return phonenumber;
      }
      countrycode(event: any){
        
        let num = '' ;
        if( this.EnquiryForm.value.mobilenum.length < 3){
            num =  this.EnquiryForm.value.mobilenum;
        }
        else {
            if(this.EnquiryForm.value.mobilenum.indexOf('231') == 0  && this.EnquiryForm.value.mobilenum.length > 3){
              num =  this.EnquiryForm.value.mobilenum.substr(3,this.EnquiryForm.value.mobilenum.length);
            }
        }
        let mobile = this.UsersService.appendcountrycode(event,num);
     if(!num)
     {
         this.EnquiryForm.patchValue({
            mobilenum : "",
          }); 
     }else{
        this.EnquiryForm.patchValue({
            mobilenum :mobile,
          }); 
     }
    
  
    }
    countryscode(event: any){
     let num = '' ;
     if( this.EnquiryForm.value.alternatenum.length < 3){
         num =  this.EnquiryForm.value.alternatenum;
     }
     else {
         if(this.EnquiryForm.value.alternatenum.indexOf('231') == 0  && this.EnquiryForm.value.alternatenum.length > 3){
           num =  this.EnquiryForm.value.alternatenum.substr(3,this.EnquiryForm.value.alternatenum.length);
         }
     }
     let mobile = this.UsersService.appendcountrycode(event,num);
  if(!num)
  {
      this.EnquiryForm.patchValue({
        alternatenum : "",
       }); 
  }else{
     this.EnquiryForm.patchValue({
        alternatenum :mobile,
       }); 
  }
    }
}
