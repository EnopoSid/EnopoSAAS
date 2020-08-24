import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef, Input} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {LogoutService} from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { ComplaintsService } from './complaints.service';
import * as $ from 'jquery';
import { IPageLevelPermissions, ComplaintModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs/internal/Subject';
import { UsersService } from 'src/app/views/users/users.service';


@Component({
    selector: 'complaint-lists-table',
    templateUrl: './complaint-list.component.html',
     styleUrls: ['./complaint-list.component.css'],
     encapsulation: ViewEncapsulation.None
})
export class ComplaintListComponent implements OnInit {
    displayedColumns = ['sno','ComplaintNum','UserName','ComplaintTypeName','ServiceProviderName','ComplaintReceivedType','CreatedDate','ComplaintStatusName','Actions']
    dataSource: MatTableDataSource<ComplaintModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    id: number;
    formErrors: any;
    status: boolean;
    sample:number;
    title: string;
    @ViewChild('myTable', {static: false}) table: any;
    @ViewChild('tableWrapper', {static: false}) tableWrapper;
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    AddComplaintFlag:boolean=false;
    complaintForm:FormGroup;
    GetComplaintsForm: FormGroup;
    userId: number = 0;
    checkedComplaints: number[] = [];
    allComplaintStatuses = []; 
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    SearchFlag:boolean=false;
    roleList=[];
    departmentFlag:boolean=false;
    departmentList=[];
    ComplaintId;
    selectedRole:number=0;
    searchComplaintBy: {'id', 'name', 'checked'}[];
    isAdvanceSearchValid: boolean = true;
    isUserFromDeskTeam:boolean ;
    complaintStatusList=[];
    editFlagForSupervisor:boolean=false;
    mobileNumber : string;
    extentionNumber: string;
    isIncomingCall :boolean=false;
    roleId : number;
    calluserId : number;
    canNotAccessCall : boolean = true;
    callType : boolean = true;
    Callcategorys = [];

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: ComplaintsService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
       // private complaintStatusService: ComplaintStatusService,
        private formBuilder:FormBuilder,
        private route:ActivatedRoute,
        private UsersService:UsersService) {
            // this.formErrors = {
            //     menuName: {},
            //     menuUrl: {}
            // };
        }   

    
ngOnInit(){    
    this.service.getComplaintFormDropdownListItems().subscribe((responseConsumerData)=>{
        this.Callcategorys = responseConsumerData.CallCategory;
            });
    //    document.getElementById('preloader-div').style.display = 'block';
    this.userId = this.sendReceiveService.globalUserId;
    this.roleId = this.sendReceiveService.globalRoleId;
    this.extentionNumber = this.sendReceiveService.globalExtentionNumber;
    if(!this.extentionNumber ){
        this.service.getExtentionNumber(this.sendReceiveService.globalUserId).subscribe((response) => {
           
            this.extentionNumber = response;
            this.sendReceiveService.globalExtentionNumber = response;
        });
    }
    this.filterData={
        filterColumnNames:[
          {"Key":'sno',"Value":" "},
          {"Key":'ComplaintNum',"Value":" "},
          {"Key":'UserName',"Value":" "},
          {"Key":'ComplaintTypeName',"Value":" "},
          {"Key":'ServiceProviderName',"Value":" "},
          {"Key":'ComplaintReceivedTypeName',"Value":" "},
          {"Key":'CreatedDate',"Value":" "},
          {"Key":'ComplaintStatusName',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
    this.isUserFromDeskTeam = (MyAppHttp.ROLEIDS_WITHOUT_OPENCOMPLAINTS.indexOf(this.sendReceiveService.globalRoleId)>-1) ? false: true;
   this.searchComplaintBy =  JSON.parse(JSON.stringify(MyAppHttp.SEARCHCOMPLAINTSBYRADIOBUTTONDATA)) ;
    this.canNotAccessCall = (MyAppHttp.CanAnswerCall.indexOf(this.sendReceiveService.globalRoleId)>-1) ? false: true;
   if(this.canNotAccessCall){
      this.searchComplaintBy.splice(3, 1);
    }

    if(this.sendReceiveService.backflag1){
        if(this.sendReceiveService.isInCall == true){
            this.getcomplaintDetails(this.sendReceiveService.globalExtentionNumber);
            this.searchComplaintBy[3].checked = true;
            this.searchComplaintBy[1].checked = this.searchComplaintBy[0].checked = this.searchComplaintBy[2].checked = false; 
            //   document.getElementById('preloader-div').style.display = 'none';;
        }
        else
        {
        this.getAllComplaints(this.searchComplaintBy[1].id);
        this.searchComplaintBy[1].checked = true;
        this.searchComplaintBy[0].checked = this.searchComplaintBy[2].checked = false; 
        }

    }else{
        if(this.sendReceiveService.isInCall == true){
            this.getcomplaintDetails(this.sendReceiveService.globalExtentionNumber);
            this.searchComplaintBy[3].checked = true;
            this.searchComplaintBy[1].checked = this.searchComplaintBy[0].checked = this.searchComplaintBy[2].checked = false; 
            //   document.getElementById('preloader-div').style.display = 'none';;
        }
        else{
            this.getAllComplaints(this.searchComplaintBy[0].id);
        }
      
    }
    this.getAllComplaintStatuss();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    this.complaintForm = this.formBuilder.group({
        'complaintNum':  [null],
        'complaintStatusId':[null],
        'mobileNum':[null,Validators.compose([Validators.minLength(12),Validators.maxLength(12)])],
        'emailId':null,
        'Callcategorysid':null,
    });

    this.GetComplaintsForm = this.formBuilder.group({
        'GetComplaintsBy':['']
    });

    // if(this.sendReceiveService.globalRoleId==MyAppHttp.DESK_SUPERVISOR_ROLEID){
    //     this.editFlagForSupervisor=true;
    // }else{
    //     this.editFlagForSupervisor=false;
    // }
}
getAllComplaints(searchComplaintsFlag){
        this.service.getComplaintsByUserIdorRoleId(this.userId, this.sendReceiveService.globalRoleId, this.sendReceiveService.globalDeptId, searchComplaintsFlag).subscribe((response) => {
        let tempId = 1;
        let enquiryData:any=[] ;
        if(this.sendReceiveService.globalRoleId!=MyAppHttp.DESK_SUPERVISOR_ROLEID && searchComplaintsFlag==2){
            response.forEach(element => {
                element.sno = tempId;
                tempId ++;
                if(element.ComplaintReceivedTypeId!=2){
                    enquiryData.push(element);
                }
            });
        }else{
            response.forEach(element => {
                element.sno = tempId;
                tempId ++;   
                if(element.ComplaintReceivedTypeId==2){
                    this.editFlagForSupervisor=true;
                } else{
                    this.editFlagForSupervisor=false;
                }        
                enquiryData=response;
            });
        }
                this.filterData.gridData = enquiryData;
                this.dataSource = new MatTableDataSource(enquiryData);
                this.filterData.dataSource=this.dataSource;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
                //   document.getElementById('preloader-div').style.display = 'none';;
    }, (error) => {
        //   document.getElementById('preloader-div').style.display = 'none';;
    });
    this.extentionNumber = this.sendReceiveService.globalExtentionNumber
}

getAllComplaintStatuss(){
    // this.complaintStatusService.getAllComplaintStatus().subscribe(reponse=>{
    //     this.allComplaintStatuses = reponse;
    // });
}

actionAfterError() {
    this.dialogRef.afterClosed().subscribe(result => {
        this.appInfoService.setSecurity();
        this.sendReceiveService.setSequence(1);
        this.router.navigate(['/sessions/signin']);
    });
}

addComplaint(){
    this.AddComplaintFlag=true;
    this.title="Save"
}

onComplaintSearchRadioClick(event){
    this.sample=event;
    for(var i=0;i<this.searchComplaintBy.length;i++){
        if(event == this.searchComplaintBy[i].id){
            this.searchComplaintBy[i].checked = true;
        }
        else{
            this.searchComplaintBy[i].checked = false;
        }
    }

    if(event==this.searchComplaintBy[2].id){
        this.complaintForm.reset();
        this.filterData.dataSource=null;
        this.SearchFlag=true;
    }else{
        this.getAllComplaints(event);
        this.SearchFlag=false;
    }

   if(event==this.searchComplaintBy[1].id){
       this.sendReceiveService.backflag1=true;
   }else{
        if(event != this.searchComplaintBy[3].id)
        {
            this.sendReceiveService.backflag1=false;
           
        }
        else{
            this.getcomplaintDetails(this.sendReceiveService.globalExtentionNumber)
        }
       
    }
    this.filterData.dataSource =null;
}
getAdvancedSearchModel()
{
    this.isAdvanceSearchValid = true;
    let complaintSearchBy = { complaintNum: null,phoneNum: null,emailId: null,statusId: null,userId: null,fromdate: null, todate:null};

    complaintSearchBy.complaintNum = this.complaintForm.value.complaintNum;
    if(!!this.complaintForm.value.mobileNum){
    complaintSearchBy.phoneNum = this.encodePhoneNumber(this.complaintForm.value.mobileNum);
    //complaintSearchBy.phoneNum = (complaintSearchBy.phoneNum.toString().length >10) ? complaintSearchBy.phoneNum.toString().slice(-9) : complaintSearchBy.phoneNum;
    }
    complaintSearchBy.phoneNum = this.encodePhoneNumber(this.complaintForm.value.mobileNum);
    complaintSearchBy.emailId = this.complaintForm.value.emailId;
    complaintSearchBy.statusId = this.complaintForm.value.complaintStatusId;
    if(!!complaintSearchBy.complaintNum || !!complaintSearchBy.phoneNum || !!complaintSearchBy.emailId|| !!complaintSearchBy.statusId)
    {
        this.service.advancedSearch(complaintSearchBy).subscribe((response) => {
            let tempId = 1;
            response.forEach(element => {
                element.sno = tempId;
                tempId ++;
            });
            this.filterData.gridData = response;
            this.dataSource = new MatTableDataSource(response);
            this.filterData.dataSource=this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
        });
    }
    else
    {
        this.isAdvanceSearchValid= false;
    }
}

countrycode(event: any){
        
    let num = '' ;
    if( this.complaintForm.value.mobileNum.length < 3){
        num =  this.complaintForm.value.mobileNum;
    }
    else {
        if(this.complaintForm.value.mobileNum.indexOf('231') == 0  && this.complaintForm.value.mobileNum.length > 3){
          num =  this.complaintForm.value.mobileNum.substr(3,this.complaintForm.value.mobileNum.length);
        }
    }
    let mobile = this.UsersService.appendcountrycode(event,num);
 if(!num)
 {
     this.complaintForm.patchValue({
        mobileNum : "",
      }); 
 }else{
    this.complaintForm.patchValue({
        mobileNum :mobile,
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
canEdit(complaints)
{
let details = complaints;
 if(details.ComplaintStatusName != "Resolved" && this.roleId == 4){
    return true;
 }
 else{
    return false;
 }
   
}
// GetIncalldetails(){
//     if(this.roleId == 4 || this.roleId == 3){
//     this.searchComplaintBy[3].checked = true;
//     this.searchComplaintBy[0].checked = this.searchComplaintBy[1].checked = this.searchComplaintBy[2].checked = false;
//     if(!this.extentionNumber ){
//         this.service.getExtentionNumber(this.sendReceiveService.globalUserId).subscribe((response) => {
//             if(response == null){
//                 this.service.getextentionNumberbyIpBased(this.sendReceiveService.globalUserId).subscribe((response) => {
//                     this.extentionNumber = response;
//                 if(!this.extentionNumber){
//                 this.getcomplaintDetails(this.extentionNumber);
//                 }
//             });
//             }
//             else{
//                 this.sendReceiveService.globalExtentionNumber = response;
//                 this.extentionNumber = response;
//                 this.sendReceiveService.isInCall = false;
//             }
           
//         })
// }
// if(this.extentionNumber != null){
//     this.getcomplaintDetails(this.extentionNumber);
// }
//     }
// }

getcomplaintDetails(extentionNumber : string){

    if(this.roleId == 4 || this.roleId == 3){
this.service.getInCallDetails(extentionNumber).subscribe((response) => {
    if(response.complaints != null){
        this.calluserId = response.UserId;
    let tempId = 1;
       response.complaints.forEach(element => {
           element.sno = tempId;
           tempId ++;
       });
       response.complaints.forEach(element => {
        element.canEdit = this.canEdit(element);
    });
       this.filterData.gridData = response.complaints;
       this.dataSource = new MatTableDataSource(response.complaints);
       this.filterData.dataSource=this.dataSource;
       this.dataSource.paginator = this.paginator;
       this.dataSource.sort = this.sort;
       this.extentionNumber = response.ExtentionNumber;
       this.sendReceiveService.globalExtentionNumber = response.ExtentionNumber;
       this.sendReceiveService.globalPhoneNumber = response.PhoneNumber;
       this.sendReceiveService.isInCall = true;
       this.mobileNumber = response.PhoneNumber; 
       this.callType = response.CallType == "InCall"?true: false
       if(this.mobileNumber ){

       this.isIncomingCall = true;
       }
   }
    else{
        if(response.ExtentionNumber == null){
            this.calluserId = null;
            this.mobileNumber = null;
            this.sendReceiveService.globalExtentionNumber = null;
            this.sendReceiveService.globalPhoneNumber = null;
            this.extentionNumber = null;
            this.sendReceiveService.isInCall = false;
           this.dataSource = response.complaints;
           this.filterData.dataSource=this.dataSource;
           this.sendReceiveService.showDialog("Softphone is not Registered ")
           this.onComplaintSearchRadioClick(1);
          
        }
        else{
            this.calluserId = response.UserId;
       this.dataSource = response.complaints;
       this.filterData.dataSource=this.dataSource;
       if(!response.complaints ){
        this.extentionNumber = response.ExtentionNumber;
        this.sendReceiveService.globalExtentionNumber = response.ExtentionNumber;
        this.sendReceiveService.globalPhoneNumber = response.PhoneNumber;
        this.mobileNumber = response.PhoneNumber;
        this.isIncomingCall = false;
        if(response.PhoneNumber == null){
            this.sendReceiveService.isInCall = false;
            this.isIncomingCall = false;
            this.sendReceiveService.showDialog("Not In Call")
            this.onComplaintSearchRadioClick(1);
           
        }
        if(response.PhoneNumber != null && response.complaints == null){
            this.sendReceiveService.isInCall = true;
       this.isIncomingCall = true;
       this.sendReceiveService.showDialog("No Complaints Data Found")
       }
    }
        }
        //   document.getElementById('preloader-div').style.display = 'none';;
    }
}, (error) => {
   //   document.getElementById('preloader-div').style.display = 'none';;
});
}
}

Addcomplaints(){
    if(this.calluserId)
    {
        this.router.navigate(['/complaints/addcomplaint/'+ this.calluserId]);
      
    }
    else{
        this.router.navigate(['/complaints/addcomplaint']);
    }
}
radioChange(buttonid)
{
    if(this.mobileNumber){
    this.service.chagecallcategory(this.mobileNumber,buttonid.value).subscribe((response) => {
        let aa = response;
    });
}
}
selectedCallCategory(selectedcallcategoryId){
    if(selectedcallcategoryId){
    this.sendReceiveService.callcategoryId  = selectedcallcategoryId;
    this.radioChange(selectedcallcategoryId)
    }
     }
}