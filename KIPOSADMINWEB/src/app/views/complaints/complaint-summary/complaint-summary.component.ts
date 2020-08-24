import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../../services/common/appInfo.service';
import {SendReceiveService} from '../../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../../services/common/myAppHttp.service';
import { IPageLevelPermissions, ComplaintModel } from '../../../helpers/common.interface';
import { ComplaintsService } from '../complaints.service';
import * as $ from 'jquery'; 
import {ComplaintListComponent} from "./../../complaints/complaint-list.component"
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { UsersService } from 'src/app/views/users/users.service';
import { Subject } from 'rxjs';

@Component({
    selector: 'app-complaint-summary',
    templateUrl: './complaint-summary.component.html',
    styleUrls: ['./complaint-summary.component.css']
})
export class ComplaintSummaryComponent implements OnInit {
    displayedColumns = ['sno','AssignedByName','AssignedToName','AssignedToRole','DepartmentName','Comments','CreatedDate','ComplaintStatusName']
    dataSource: MatTableDataSource<ComplaintModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    formErrors: any;
    status: boolean;
    title: string;
    id:number;
    @ViewChild('myTable', {static: false}) table: any;
    @ViewChild('tableWrapper', {static: false}) tableWrapper;
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    userId: number =0;
    idOnUpdate:number=0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    complaintNumber:number;
    ComplaintDetailsForm:FormGroup;
    AssigningComplaintForm:FormGroup;
    StatusFlag:boolean=false;
    ComplaintId:number;
    roleList=[];
    selectedRole:number;
    departmentFlag:boolean=false;
    departmentList=[];
    auditDataCount:number =0;
    searchComplaintBy: {'id', 'name', 'checked'}[] = MyAppHttp.SEARCHCOMPLAINTSBYRADIOBUTTONDATA;
    allComplaintStatuses=[];
    isAssignedToMe:boolean = false;
    usersList = [];

    constructor(private router: Router,
        private route:ActivatedRoute,
        private spinner: NgxSpinnerService,
        public service: ComplaintsService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public userservice : UsersService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        private formBuilder:FormBuilder,
) {
            this.formErrors = {
                menuName: {},
                menuUrl: {}
            };
        }

    
ngOnInit(){
    this.id = +this.route.snapshot.params['id'];
    this.userId=this.sendReceiveService.globalUserId;
    this.filterData={
        filterColumnNames:[
          {"Key":'sno',"Value":" "},
          {"Key":'AssignedByName',"Value":" "},
          {"Key":'AssignedToName',"Value":" "},
          {"Key":'AssignedToRole',"Value":" "},
          {"Key":'DepartmentName',"Value":" "},
          {"Key":'Comments',"Value":" "},
          {"Key":'CreatedDate',"Value":" "},
          {"Key":'ComplaintStatusName',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
    this.getComplaintSummary(this.id);
    this.getAllComplaintStatus();
    this.getAssignedToIdByComplaintId(this.id);
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    this.service.getComplaintNumber(this.id).subscribe((resp)=>{
        this.complaintNumber=resp;
    });

    this.AssigningComplaintForm = this.formBuilder.group({
        'assignedTo':[''],
        'assignedToUserId':[''],
        'complaintStatus': [''],
        'comments':['', Validators.compose([Validators.required])],
        'department':['']
    });

    this.ComplaintDetailsForm=this.formBuilder.group({
        'complaintNumber':[''],
        'complainantName': [''],
        'complaintType':[''],
        'serviceProvider':[''],
        'region':[''],
        'zone':[''],
        'complaintDetails':[''],
        'refNumber':['']
    });
    this.getComplaintDetails(this.id);
}
getComplaintSummary(complaintId){
    this.service.getComplaintSummary(complaintId).subscribe((response) => {
        const complaintSummaryData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                complaintSummaryData.push(response[i]);
            }
            this.auditDataCount=complaintSummaryData.length;
            this.filterData.gridData = complaintSummaryData;
            this.dataSource = new MatTableDataSource(complaintSummaryData);
            this.filterData.dataSource=this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
            //   document.getElementById('preloader-div').style.display = 'none';;
    }, (error) => {
        //   document.getElementById('preloader-div').style.display = 'none';;
    }, () => {

    });
}

getComplaintDetails(id){
    this.service.getComplaintById(id).subscribe((resp)=>{
        this.ComplaintDetailsForm.patchValue({
            complaintNumber:resp.ComplaintNum,
            complainantName:resp.FirstName+" "+resp.LastName,
            complaintType:resp.ComplaintTypeName,
            serviceProvider:resp.ServiceProviderName,
            region:resp.RegionName,
            zone:resp.ZoneName,
            complaintDetails:resp.ComplaintDetails,
            refNumber:resp.refNumber,
        });
        this.ComplaintDetailsForm.disable();
    });
}

updateStatus()
{
    this.StatusFlag=true;
    this.ComplaintId=this.id;
    let searchComplaintFlag = this.auditDataCount == 1 ? this.searchComplaintBy[1].id :  this.searchComplaintBy[0].id
     this.service.getRolesByHierarchy(this.sendReceiveService.globalRoleId, searchComplaintFlag).subscribe((resp)=>{
        this.roleList=resp;
    }, (error) => {
    });
}
actionAfterError() {
    this.dialogRef.afterClosed().subscribe(result => {
        this.appInfoService.setSecurity();
        this.sendReceiveService.setSequence(1);
        this.router.navigate(['/sessions/signin']);
    });
}
onChangeofRole(selectedRoleId){
    this.selectedRole=selectedRoleId;
    this.usersList = [];
    if(selectedRoleId==MyAppHttp.DEPARTMENT_OFFICER_ROLEID||(selectedRoleId==MyAppHttp.DEPARTMENT_MANAGER_ROLEID && this.sendReceiveService.globalRoleId==MyAppHttp.EXECUTIVE_ROLEID)){
        this.departmentFlag=true;
        this.service.getDepartments().subscribe((resp)=>{
            this.departmentList=resp;
        })
    }
    else{
        this.getusersDetailsByRoleIdorDepartment(selectedRoleId, 0)
        this.departmentFlag=false;
    }
}

onAssigningComplaintFormSubmit(){
    if(this.AssigningComplaintForm.invalid){ 
        $('input.ng-invalid textarea.ng-invalid').first().focus();
        return false;
    }
    let var_assignedToUserId: string = this.AssigningComplaintForm.value.assignedToUserId;
    let var_assigned: string = this.AssigningComplaintForm.value.assignedTo;
    let department;
    if(!!this.sendReceiveService.globalDeptId && MyAppHttp.DEPARTMENT_MANAGER_ROLEID != this.sendReceiveService.globalRoleId){
        department= this.sendReceiveService.globalDeptId;
    }
    if(!!this.AssigningComplaintForm.value.department){
        department= this.AssigningComplaintForm.value.department;
    }
    let var_status:string=this.AssigningComplaintForm.value.complaintStatus;
    let comments: string = this.AssigningComplaintForm.value.comments;
        this.service.changeComplaintStatus({
            'AssignedToUserId': var_assignedToUserId,
            'AssignedTo': var_assigned,
            'ComplaintId': this.ComplaintId,
            'IsActive': 1,
            'Comments': comments,
            'ComplaintStatusId':var_status,
            'DepartmentId': department,
        }).subscribe((data) => {
            this.getComplaintSummary(this.id);
            this.AssigningComplaintForm.reset();
            this.StatusFlag=false;
            this.getAssignedToIdByComplaintId(this.id);
            if(var_assigned!="")
            {
                this.router.navigate(['/complaints']);
            }
        }, error =>  error => {
            this.formErrors = error;
        });
}
getAllComplaintStatus(){
    this.service.getComplaintStatusList().subscribe(reponse=>{
        this.allComplaintStatuses = reponse;
    });
}

onCancel(){
    this.AssigningComplaintForm.reset();
    this.StatusFlag=false;
}
onGoBack(){
    this.router.navigate(['/complaints']);
}
getAssignedToIdByComplaintId(complaintId){
    this.isAssignedToMe = false;
    this.service.getAssignedToIdByComplaintId(complaintId).subscribe(response=>{
            this.isAssignedToMe = !!response ? 
            ( (response == this.sendReceiveService.globalRoleId || response == -1) ? true : false ) : false;
    })
}

onChangeofStatus(selectedStatus){
    if(selectedStatus==4){
        this.AssigningComplaintForm.get('assignedTo').disable();
    }
    else{
        this.AssigningComplaintForm.get('assignedTo').enable();
    }
}

getusersDetailsByRoleIdorDepartment(RoleId, deptId)
{
    this.userservice.getUsersByRoleIdOrDeptId(RoleId,deptId).subscribe(data => {
        this.usersList = data; 
    });
}
}
