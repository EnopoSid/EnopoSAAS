import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef, Input, Output, EventEmitter} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {LogoutService} from '../../../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../../../services/common/appInfo.service';
import {SendReceiveService} from '../../../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../../../services/common/myAppHttp.service';
import { ComplaintsService } from './../../complaints.service';
import {LowerCasePipe} from '@angular/common';
import { UsersService } from '../../../users/users.service';
import * as $ from 'jquery';
import { IPageLevelPermissions, ComplaintModel } from '../../../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { ExportService } from '../../../../services/common/exportToExcel.service';
import { Http } from '@angular/http';



@Component({
    selector: 'user-add-complaint-component',
    templateUrl: './add-complaint.component.html',
    styleUrls: ['./add-complaint.component.css'],
    encapsulation: ViewEncapsulation.None
})
export class UserAddComplaintComponent implements OnInit {
    displayedColumns = ['sno','FileName','Actions']
    dataSource: MatTableDataSource<ComplaintModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    SampleFlag: boolean=false;
    selectedContacted: boolean=false;
    selected: boolean=false;
    saveUser:boolean = false;
    updateUser:boolean = false;
    checkInput:boolean = false;
    regionList = [];
    zoneList=[];
    serviceProviderList=[];
    serviceCategoryList=[];
    complaintTypeList = [];
    communicationTypes = [];
    active :boolean;
    dublicateemail :boolean;
    id: number = 0;
    AddComplaintForm :FormGroup;
    formErrors: any;
    title: string;
    @ViewChild('myTable', {static: false}) table: any;
    @ViewChild('tableWrapper', {static: false}) tableWrapper;
    userId: number = 0;
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    AddComplaintFlag:boolean=false;
    isOthersSelectedForServiceProvider: boolean = false;
    isOthersSelectedForCategory: boolean=false;
    isOthersSelectedForComplaintType:boolean=false;
    isRegionSelected:boolean =false;
    regionId;
    ComplaintReceivedTypeId:number;
    ComplaintStatusId : number;
    ComplaintUserID : number;
    formStage:number=1;
    userid:number;
    isOnView=false;
    selectedFiles: FileList;
    selectedComplaintFiles :any = [];
    @Input() isFromConsumer: boolean;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    isVerified: boolean = false;
    ComplaintNumber;
    header = "Add Complaint Form" ;
    IsReportedToServiceProviderList=[{id:0,Value:'No',Checked:true},
                    {id:1,Value:'Yes',Checked:false}];
    HaveContactedWithusBeforeList=[{id:0,Value:'No',Checked:true},
                    {id:1,Value:'Yes',Checked:false}]
    complaintfromcall: number;
    incall:boolean;
    constructor(
        private spinner: NgxSpinnerService,
        public service: ComplaintsService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        private formBuilder:FormBuilder,
        private _router: Router, 
        private UsersService:UsersService,
        private exprotService:ExportService,
        private route:ActivatedRoute,
        public _http:Http
    ) {
            this.formErrors = {
                firstName: {},
                lastName: {}
            };
}

    
ngOnInit(){    
    this.userId = this.sendReceiveService.globalUserId;
    this.incall = this.sendReceiveService.isInCall;
    if(this.incall){
        this.complaintfromcall = 1
    }
    this.filterData={
        filterColumnNames:[
          {"Key":'sno',"Value":" "},
          {"Key":'FileName',"Value":" "},
          {"Key":'Actions',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
      if(this.route.snapshot.url[0].path == 'addcomplaint'){
        if(this.route.snapshot.params['id']){
        this.userid =  this.route.snapshot.params['id'];
       

        }
      }
      if(this.route.snapshot.url[0].path != 'addcomplaint'){
        this.id = +this.route.snapshot.params['id'];
      }

    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;
        if(!this.isFromConsumer){
            var temproute=this._router.url;
            if(temproute.indexOf('add') > -1) {
                this.AddComplaintForm.enable();
                this.title="Save";
                if(!this.pagePermissions.Add)
                    this.sendReceiveService.logoutService();
            }

            if(temproute.indexOf('update') > -1) {
                this.AddComplaintForm.enable();
                this.isOnView=false;
                this.title="Update";
                this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
                    this.header ="Edit Complaint Form  - " + resp;
                   });
                if(!this.pagePermissions.Edit)
                    this.sendReceiveService.logoutService();
            }

            if(temproute.indexOf('view') > -1) {
                this.AddComplaintForm.disable();
                this.isOnView=true;
                this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
                    this.header ="View Complaint Form  - " + resp;
                   });
                if(!this.pagePermissions.View)
                    this.sendReceiveService.logoutService();
            }
        }
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    if (!isNaN(this.id)) {
        this.AddComplaintForm = this.formBuilder.group({
            id: 0,
            'firstName': [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
            'lastName': [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
            'email':  [null, Validators.compose([Validators.required])],
            'mobilenum': [null, Validators.compose([Validators.required, Validators.minLength(12),Validators.maxLength(12)])],
            'alternatenum': [null, Validators.compose([ Validators.minLength(12),Validators.maxLength(12)])],
            'postalAddress':[null,Validators.required],
            'residentialAddress':[null],
            'region': [null, Validators.required],
            'location':[null],
            'zone':[null,Validators.required],
            'serviceProvider': [null, Validators.required],
            'serviceCategory':[null, Validators.required],
            'complaintType':[null,Validators.required],
            'complaintDetails':[null,Validators.required],
            'IsReportedToServiceProvider':[null],
            'referenceNumber':[null],
            'agentName':[null],
            'HaveContactedWithusBefore':[null],
            'feedback':[null,Validators.required],
            'File':[null],
      });
        this.saveUser = false;
        this.updateUser = true;
            this.checkInput = true;  
            let urlarray = this.route.snapshot.url;
           if(this.route.snapshot.url[0].path != 'addcomplaint'){
            this.processEditAction(this.id);
           }
           if(this.userid ){
            this.GetUserDetails(this.userid);
           }
    }else{
        this.AddComplaintForm = this.formBuilder.group({
            id: 0,
            'firstName': [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
            'lastName': [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
            'email':  [null, Validators.compose([Validators.required])],
            'mobilenum': [null, Validators.compose([Validators.required, Validators.minLength(12),Validators.maxLength(12)])],
            'alternatenum': [null, Validators.compose([ Validators.minLength(12),Validators.maxLength(12)])],
            'postalAddress':[null,Validators.required],
            'residentialAddress':[null],
            'region': [null, Validators.required],
            'location':[null],
            'zone':[null,Validators.required],
            'serviceProvider': [null, Validators.required],
            'serviceCategory':[null, Validators.required],
            'complaintType':[null,Validators.required],
            'complaintDetails':[null,Validators.required],
            'IsReportedToServiceProvider':[null],
            'referenceNumber':[null],
            'agentName':[null],
            'HaveContactedWithusBefore':[null],
            'feedback':[null,Validators.required],
            'File':[null],
      });
      this.title="Save"
      this.header ="Add Complaint Form "; 
    } 
    if(this.isFromConsumer==true){
        this.service.getConsumerComplaintFormDropdownListItems().subscribe((responseConsumerData)=>{
            this.regionList = responseConsumerData.Regions;
    this.serviceProviderList = responseConsumerData.ServiceProviders;
    this.serviceCategoryList = responseConsumerData.ServiceCategories;
    this.complaintTypeList = responseConsumerData.ComplaintTypes;
    this.communicationTypes = responseConsumerData.Communications;
        })
    }else{
        this.service.getComplaintFormDropdownListItems().subscribe((responseData)=>{
            this.regionList = responseData.Regions;
            this.serviceProviderList = responseData.ServiceProviders;
            this.serviceCategoryList = responseData.ServiceCategories;
            this.complaintTypeList = responseData.ComplaintTypes;
            this.communicationTypes = responseData.Communications;
          })
    }
    if(this.sendReceiveService.globalPhoneNumber){
        this.AddComplaintForm.patchValue({
            mobilenum:this.decodePhoneNumber(this.sendReceiveService.globalPhoneNumber),
        });
        this.AddComplaintForm.enable();
            this.formStage =1 ;
    }
}

keyPress(event: any) {
    const pattern = /[0-9\+\-\ ]/;
    let inputChar = String.fromCharCode(event.charCode);
    if (event.keyCode != 8 && !pattern.test(inputChar)) {
      event.preventDefault();
    }
}

processEditAction(id){

    this.service.getComplaintById(id)
        .subscribe(resp => {
            this.isOnView=true;
            this.ComplaintNumber=resp.ComplaintNum;
            this.ComplaintReceivedTypeId = resp.ComplaintReceivedTypeId;
            this.ComplaintStatusId = resp.ComplaintStatusId;
            this.ComplaintUserID = resp.UserId;
            this.AddComplaintForm.patchValue({
                firstName:resp.FirstName,
                lastName:resp.LastName,
                email:resp.EmailId,
                mobilenum:this.decodePhoneNumber(resp.PhoneNumber),
                alternatenum:this.decodePhoneNumber(resp.TelePhoneNumber),
                postalAddress:resp.PostalAddress,
                region:resp.RegionId,
                zone: resp.ZoneId,
                complaintType:resp.ComplaintTypeId,
                complaintTypeIfOthers:resp.ComplaintTypeNameIfOthers,
                serviceProvider:resp.ServiceproviderId,
                serviceProviderIfOthers:resp.ServiceProviderNameIfOthers,
                serviceCategory:resp.ServiceCategoryId,
                categoryIfOthers:resp.ServiceCategoryNameIfOthers,
                id:resp.ComplaintId,
                complaintDetails:resp.ComplaintDetails,
                IsReportedToServiceProvider: !!resp.IsReportedToServiceProvider==false?0:1,
                serviceProviderDetails:resp.ServiceProviderReportDetails,
                referenceNumber:resp.ServiceProviderComplaintRefNum,
                agentName:resp.CustomerCareAgentName,
                HaveContactedWithusBefore:!!resp.HaveContactedWithusBefore==false?0:1,
                PreviousContactedDetails:resp.PreviousContactedDetails,
                feedback:resp.CommunicationId,
                
            });
            this.fileArray(resp.Files);
            this.AddComplaintForm.enable();
            this.formStage =1 ;
        },  error => this.formErrors = error);
}

onChangeServiceProvider (selectedServiceProviderId){
    this.isOthersSelectedForServiceProvider = (selectedServiceProviderId == 19 ? true: false) ;
    if(this.isOthersSelectedForServiceProvider == true){
        let control: FormControl = new FormControl(null, Validators.required);
        this.AddComplaintForm.addControl('serviceProviderIfOthers', control);
    }
    else {
        this.AddComplaintForm.removeControl('serviceProviderIfOthers');
    }
}
onChangeCategory(selectedCategoryId){
    this.isOthersSelectedForCategory = (selectedCategoryId == 6 ? true: false) ;
    if(this.isOthersSelectedForCategory == true){
        let control: FormControl = new FormControl(null, Validators.required);
        this.AddComplaintForm.addControl('categoryIfOthers', control);
    }
    else {
        this.AddComplaintForm.removeControl('categoryIfOthers');
    }
}
onChangeComplaintType(selectedComplaintTypeId){
    this.isOthersSelectedForComplaintType=(selectedComplaintTypeId==18 ? true:false);
    if(this.isOthersSelectedForComplaintType == true){
        let control: FormControl = new FormControl(null, Validators.required);
        this.AddComplaintForm.addControl('complaintTypeIfOthers', control);
    }
    else {
        this.AddComplaintForm.removeControl('complaintTypeIfOthers');
    }
}
onSelected(value){
  this.selected=(value==1?true:false);
    if(this.selected == true){
        let control: FormControl = new FormControl(null, Validators.required);
        this.AddComplaintForm.addControl('serviceProviderDetails', control);
        this.IsReportedToServiceProviderList[1].Checked=true;
        this.IsReportedToServiceProviderList[0].Checked=false;
    }
    else {
        this.AddComplaintForm.removeControl('serviceProviderDetails');
        this.IsReportedToServiceProviderList[1].Checked=false;
        this.IsReportedToServiceProviderList[0].Checked=true;
    }
}
onSelectedContacted(value){
    this.selectedContacted=(value==1?true:false);

    if(this.selectedContacted == true){
        let control: FormControl = new FormControl(null, Validators.required);
        this.AddComplaintForm.addControl('PreviousContactedDetails', control);
        this.HaveContactedWithusBeforeList[1].Checked=true;
        this.HaveContactedWithusBeforeList[0].Checked=false;
    }
    else {
        this.AddComplaintForm.removeControl('PreviousContactedDetails');
        this.HaveContactedWithusBeforeList[1].Checked=false;
        this.HaveContactedWithusBeforeList[0].Checked=true;
    }
} 
  
actionAfterError() {
    this.dialogRef.afterClosed().subscribe(result => {
        this.appInfoService.setSecurity();
        this.sendReceiveService.setSequence(1);
        this._router.navigate(['/sessions/signin']);
    });
}

onAddComplaintSubmit(){
    this.AddComplaintForm.enable();
    let var_id: number = this.AddComplaintForm.value.id;
    let var_firstName: string = this.AddComplaintForm.value.firstName;
    let var_lastName: string = this.AddComplaintForm.value.lastName;
    let var_email: string = this.AddComplaintForm.value.email;
    let var_mobilenumber: number = this.encodePhoneNumber(this.AddComplaintForm.value.mobilenum);
    let var_alternatenumber: number = this.encodePhoneNumber(this.AddComplaintForm.value.alternatenum);
    let var_postalAddress = this.AddComplaintForm.value.postalAddress;
    let residentialAddress = this.AddComplaintForm.value.residentialAddress;
    let var_region = this.AddComplaintForm.value.region;
    let var_zone: number = this.AddComplaintForm.value.zone;
    let location = this.AddComplaintForm.value.location;
    let var_serviceProvider = this.AddComplaintForm.value.serviceProvider;
    let var_serviceCategory = this.AddComplaintForm.value.serviceCategory;
    let var_complaintType = this.AddComplaintForm.value.complaintType;
    let var_complaintDetails = this.AddComplaintForm.value.complaintDetails;
    let referenceNumber = this.AddComplaintForm.value.referenceNumber;
    let agentName = this.AddComplaintForm.value.agentName;
    let feedback = this.AddComplaintForm.value.feedback;
    let FileUpload = this.selectedFiles;
    var userData = {
      FirstName: var_firstName,
      LastName: var_lastName,
      EmailId: var_email,
      PhoneNumber: var_mobilenumber,
      PostalAddress: var_postalAddress,
      TelePhoneNumber:var_alternatenumber,
      ResidentialAddress:residentialAddress
    }

    if(this.title=="Save"){        
        if (!this.AddComplaintForm.valid) {
            return;
            }
            let tempLoginUserId = !!this.userId ? this.userId : 0;
      
          if(this.isFromConsumer==true){
              this.service.addConsumerComplaints({
                  'ModifiedBy':  tempLoginUserId,
                  'CreatedBy': tempLoginUserId,
                  'UserId': tempLoginUserId,
                  'ComplaintTypeId':var_complaintType,
                  'ComplaintDetails':var_complaintDetails,
                  'IsReportedToServiceProvider': !!this.AddComplaintForm.value.IsReportedToServiceProvider ? true: false,
                  'ServiceProviderReportDetails':this.AddComplaintForm.value.serviceProviderDetails,
                  'ServiceProviderComplaintRefNum':referenceNumber,
                  'CustomerCareAgentName':agentName,
                  'HaveContactedWithusBefore':  !!this.AddComplaintForm.value.HaveContactedWithusBefore ? true : false,
                  'PreviousContactedDetails':this.AddComplaintForm.value.PreviousContactedDetails,
                  'CommunicationId':feedback,
                  'ComplaintReceivedTypeId':5,
                  'ComplaintStatusId':MyAppHttp.ACTIVESTATUS,
                  'ServiceCategoryId':var_serviceCategory,
                  'SpecificLocation':this.AddComplaintForm.value.location,
                  'ZoneId': var_zone,
                  'ServiceProviderId':var_serviceProvider,
                  'IpAddress':'192.168.0.10',
                  'ServiceProviderNameIfOthers':this.AddComplaintForm.value.serviceProviderIfOthers,
                  'ServiceCategoryNameIfOthers':this.AddComplaintForm.value.categoryIfOthers,
                  'ComplaintTypeNameIfOthers':this.AddComplaintForm.value.complaintTypeIfOthers,
                  'User': userData
              },FileUpload,).subscribe((data) => {
                  this._router.navigate(['/']);
                  this.ComplaintNumber=data.ComplaintNum;
                  this.sendReceiveService.showDialog("Complaint with Complaint Number "+ this.ComplaintNumber+" has been registered successfully");
              }, error => {this.formErrors = error
            })  
          }
          else{
              this.service.addComplaints({
                  'ModifiedBy':  tempLoginUserId,
                  'CreatedBy': tempLoginUserId,
                  'UserId': tempLoginUserId,
                  'ComplaintTypeId':var_complaintType,
                  'ComplaintDetails':var_complaintDetails,
                  'IsReportedToServiceProvider': !!this.AddComplaintForm.value.IsReportedToServiceProvider ? true: false,
                  'ServiceProviderReportDetails':this.AddComplaintForm.value.serviceProviderDetails,
                  'ServiceProviderComplaintRefNum':referenceNumber,
                  'CustomerCareAgentName':agentName,
                  'HaveContactedWithusBefore':  !!this.AddComplaintForm.value.HaveContactedWithusBefore ? true : false,
                  'PreviousContactedDetails':this.AddComplaintForm.value.PreviousContactedDetails,
                  'CommunicationId':feedback,
                  'ComplaintReceivedTypeId':!this.complaintfromcall?3:this.complaintfromcall,
                  'ComplaintStatusId':1,
                  'ServiceCategoryId':var_serviceCategory,
                  'SpecificLocation':this.AddComplaintForm.value.location,
                  'ZoneId': var_zone,
                  'ServiceProviderId':var_serviceProvider,
                  'IpAddress':'192.168.0.10',
                  'ServiceProviderNameIfOthers':this.AddComplaintForm.value.serviceProviderIfOthers,
                  'ServiceCategoryNameIfOthers':this.AddComplaintForm.value.categoryIfOthers,
                  'ComplaintTypeNameIfOthers':this.AddComplaintForm.value.complaintTypeIfOthers,
                  'User': userData,
                  'Isincall': this.sendReceiveService.isInCall,
                  'IncomingPhoneNumber': this.sendReceiveService.globalPhoneNumber,
              },FileUpload,).subscribe((data) => {
                  if(this.sendReceiveService.isInCall == false){
                      this._router.navigate(['../'], { relativeTo: this.route });
                  }
                      if(this.sendReceiveService.isInCall == true){
                        this._router.navigate(['/complaints']);
                      }
                      this.ComplaintNumber=data.ComplaintNum;
                      this.sendReceiveService.showDialog("Complaint with Complaint Number "+ this.ComplaintNumber+" has been registered successfully");
              }, error => {this.formErrors = error
            })
          } 
    }else if(this.title=="Update"){
        let tempLoginUserId =  this.ComplaintUserID ;
        this.service.updateComplaint({
        'ComplaintId':var_id,
        'ComplaintNum':this.ComplaintNumber,
        'ModifiedBy':  tempLoginUserId,
        'CreatedBy': tempLoginUserId,
        'UserId': tempLoginUserId,
        'ComplaintTypeId':var_complaintType,
        'ComplaintDetails':var_complaintDetails,
        'IsReportedToServiceProvider': !!this.AddComplaintForm.value.IsReportedToServiceProvider ? true: false,
        'ServiceProviderReportDetails':this.AddComplaintForm.value.serviceProviderDetails,
        'ServiceProviderComplaintRefNum':referenceNumber,
        'CustomerCareAgentName':agentName,
        'HaveContactedWithusBefore':  !!this.AddComplaintForm.value.HaveContactedWithusBefore ? true : false,
        'PreviousContactedDetails':this.AddComplaintForm.value.PreviousContactedDetails,
        'CommunicationId':feedback,
        'ComplaintReceivedTypeId':this.ComplaintReceivedTypeId,
        'ComplaintStatusId':this.ComplaintStatusId,
        'ServiceCategoryId':var_serviceCategory,
        'SpecificLocation':this.AddComplaintForm.value.location,
        'ZoneId': var_zone,
        'ServiceProviderId':var_serviceProvider,
        'IpAddress':'192.168.0.10',
        'ServiceProviderNameIfOthers':this.AddComplaintForm.value.serviceProviderIfOthers,
        'ServiceCategoryNameIfOthers':this.AddComplaintForm.value.categoryIfOthers,
        'ComplaintTypeNameIfOthers':this.AddComplaintForm.value.complaintTypeIfOthers,
        'User': userData
    },FileUpload,var_id).subscribe(res=>{
        if(this.sendReceiveService.isInCall == true){
            this._router.navigate(['/complaints']);
        }
        else{
            this._router.navigate(['../'], { relativeTo: this.route });
        }
        this.sendReceiveService.showDialog("Complaint with Complaint Number "+ this.ComplaintNumber+" has been Updated successfully");
    })
    }

}
selectFile(event) {
    this.selectedFiles = event.target.files;
}
  
onSave(){
    if(this.AddComplaintForm.invalid){
        $('input.ng-invalid Mat-select.ng-invalid').first().focus();
        return false;
    }
  this.AddComplaintForm.disable();
  this.formStage=2;
}
onEditClick(){
    this.AddComplaintForm.enable();
    this.formStage =1 ;
}
onChangeRegion(selectedRegionId){
    this.isRegionSelected = true;
    this.regionId=selectedRegionId;
    if(this.isFromConsumer==true){
        this.service.getConsumerZonesByRegionId(this.regionId).subscribe((zone)=>{
            this.zoneList=zone;
    })
    }
    else{
        this.service.getZonesByRegionId(this.regionId).subscribe((zone)=>{
            this.zoneList=zone;
          })
    }
}
onGoBack(){
    if(this.isFromConsumer==true){
        this._router.navigate(['/']);
    }
    else{
       this._router.navigate(['/complaints']);
    }
   
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
        let theFile;
        this.service.downloadFile(downloadLink.Id)
        .subscribe(data => {
        var link = document.createElement('a');
        link.download =  downloadLink.FilePath.substring(downloadLink.FilePath.lastIndexOf('\\')+1) || LowerCasePipe;
        link.href = ('data:application/octet-stream;base64,' + data);
        link.click();




    })
    }


    onComplaintsummary(){
        this._router.navigateByUrl("/complaints/complaintsummary/"+ this.id);
    }
    countrycode(event: any){
        
        let num = '' ;
        if( this.AddComplaintForm.value.mobilenum.length < 3){
            num =  this.AddComplaintForm.value.mobilenum;
        }
        else {
            if(this.AddComplaintForm.value.mobilenum.indexOf('231') == 0  && this.AddComplaintForm.value.mobilenum.length > 3){
              num =  this.AddComplaintForm.value.mobilenum.substr(3,this.AddComplaintForm.value.mobilenum.length);
            }
        }
        let mobile = this.UsersService.appendcountrycode(event,num);
     if(!num)
     {
         this.AddComplaintForm.patchValue({
            mobilenum : "",
          }); 
     }else{
        this.AddComplaintForm.patchValue({
            mobilenum :mobile,
          }); 
     }
    
  
    }
    countryscode(event: any){
        let num = '' ;
        if( this.AddComplaintForm.value.alternatenum.length < 3){
            num =  this.AddComplaintForm.value.alternatenum;
        }
        else {
            if(this.AddComplaintForm.value.alternatenum.indexOf('231') == 0  && this.AddComplaintForm.value.alternatenum.length > 3){
              num =  this.AddComplaintForm.value.alternatenum.substr(3,this.AddComplaintForm.value.alternatenum.length);
            }
        }
        let mobile = this.UsersService.appendcountrycode(event,num);
     if(!num)
     {
         this.AddComplaintForm.patchValue({
           alternatenum : "",
          }); 
     }else{
        this.AddComplaintForm.patchValue({
           alternatenum :mobile,
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

    
GetUserDetails(userid){ 
    this.service.getusersfromComplaint(userid)
        .subscribe(resp => {
            this.AddComplaintForm.patchValue({
                firstName:resp.FirstName,
                lastName:resp.LastName,
                email:resp.EmailId,
                mobilenum:this.decodePhoneNumber(resp.PhoneNumber),
                alternatenum:this.decodePhoneNumber(resp.TelePhoneNumber),
                postalAddress:resp.PostalAddress,
            });
            this.AddComplaintForm.enable();
            this.formStage =1 ;
        },  error => this.formErrors = error);
}
}
