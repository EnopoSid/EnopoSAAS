import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {LogoutService} from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, RolePermissionModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { KpointsService } from './kpoints.service';
import { TranslateService } from '@ngx-translate/core';
import * as $ from "jquery";
import { Subject } from 'rxjs';
import * as XLSX from 'xlsx';
declare var jsPDF: any;  
import { DatePipe } from '@angular/common'
import { ComfirmComponent } from '../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';
import { AppComfirmComponent } from '../app-dialogs/app-confirm/app-confirm.component';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { ThemeService } from 'ng2-charts';
@Component({
    selector: 'app-rolepermissions',
    templateUrl: './kpoints.component.html'
})
export class KpointsComponent implements OnInit {
    displayedColumns = ['sno','CustomerNo','CustomerName','EmailID','SubscribedPlan','TotalEarnedKPoints','TotalManualKPoints','CurrentAvailableKPoints','Actions']
    dataSource: MatTableDataSource<RolePermissionModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    permission: any;
    id:number = 0;
    saveRole:boolean = false;
    @ViewChild('myTable', {static: false}) table: any;
    @ViewChild('tableWrapper', {static: false}) tableWrapper;
    dialogRef: MatDialogRef<any>;
    editRole:boolean = true; 
    rolepermissions:any;
    PermissionId: string;
    role: string;
    permissionId:number;
    selectedRow : Number;
    orderId:number;
    permisssionList:any;
    status:boolean ;
    public permissionname ;
    permissionRole :boolean = true;
    userId: number = 0;
    permissionType = false;
    RolePermissionForm: FormGroup;
    rows = [];
    temp = [];
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    rolename: string;
    memberDetails: any = {};
    showMemberDetails:boolean=false;
    AddKpointsFlag:boolean=false;
    posPlanFormGroup: FormGroup;
    excelSalesData: any = [];  
    formErrors: any;
    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: KpointsService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
         public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        private formBuilder:FormBuilder,
        private route:ActivatedRoute,
        public datepipe:DatePipe,
        private _router: Router,
        public exportService:ExportService,) { 
        }
  
    ngOnInit() {


      
      this.userId=this.sendReceiveService.globalUserId;
      this.filterData={
        filterColumnNames:[
            {"Key":'sno',"Value":" "},
            {"Key":'CustomerNo',"Value":" "},
            {"Key":'CustomerName',"Value":" "},
            {"Key":'EmailID',"Value":" "},
            {"Key":'SubscribedPlan',"Value":" "},
            {"Key":'TotalEarnedKPoints',"Value":""},
            {"Key":'TotalManualKPoints',"Value":" "},
            {"Key":'CurrentAvailableKPoints',"Value":" "},
            
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
     
      this.RolePermissionForm = this.formBuilder.group({
        'RefundStatusId':  [null]
    });
        this.id = 2 ;
        if(!isNaN(this.id))
        {
        this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
          this.rolename = resp;
        
        })
      }
      
        
        this.getAllKpoints();
       
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
            this.pagePermissions =pageLevelPermissions.response;  
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
        this.posPlanFormGroup = this.formBuilder.group({
      
         
           'ManualRewardPoints': ['',Validators.compose([ Validators.required])],
           'comment':[],
          });
          
    }
  
    updatePagination(){
     this.filterData.dataSource=this.filterData.dataSource;
     this.filterData.dataSource.paginator = this.paginator;
    }

    getAllKpoints() {
      document.getElementById('preloader-div').style.display = 'block';
  this.service.getKpoints().subscribe((response) => {
    this.temp=response;
      const orderData: any = [];     
      this.excelSalesData = response;        
      for (let i = 0; i < response.length; i++) {
          response[i].sno = i + 1;
          response[i].editMode = false;
          orderData.push(response[i]);
          this.rows.push(response[i]);
      }
      this.filterData.gridData = orderData;
      this.dataSource = new MatTableDataSource(orderData);
      this.filterData.dataSource=this.dataSource;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    
        document.getElementById('preloader-div').style.display = 'none';
  }, (error) => {
        document.getElementById('preloader-div').style.display = 'none';
  });
}

 onGoBack(){
    this._router.navigate(['../'],{relativeTo: this.route}); 
  }
  addKpoints(){
    this.AddKpointsFlag=true;
       
      
   }
  addKpointsToMember(Id) {
    this.service.getIsLoyalityMemberDetails(Id).subscribe(success => {
      this.memberDetails = success;
      this.showMemberDetails = true;
    })
  }
    onCancel(){
      
      this.posPlanFormGroup.reset();
      this.AddKpointsFlag=false;
  }
  onCancelAdd(){
    this.posPlanFormGroup.reset();
    this.showMemberDetails = false;
  }
  _keyPress(event: any) {
    const pattern = /[0-9.]/;
    let inputChar = String.fromCharCode(event.charCode);
    if (!pattern.test(inputChar)) {
        event.preventDefault();

    }
}
  onKpointsSubmit() {
    document.getElementById('preloader-div').style.display = 'block';
    if (!this.posPlanFormGroup.valid) {
        return;
    }
    let MemberId: string = this.memberDetails.MemberId;
    let ManualRewardPoints: any = this.posPlanFormGroup.value.ManualRewardPoints;
    let comment: any = this.posPlanFormGroup.value.comment;
    this.posPlanFormGroup.reset();
        this.service.saveKpoints({
            'MemberId': MemberId,
            'ManualRewardPoints': ManualRewardPoints,
            'IsActive': 1,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
            'CreatedDate':Date.now,
            "comments":comment
        }).subscribe((data) => {
          document.getElementById('preloader-div').style.display = 'none';
            this.getAllKpoints();
            this.posPlanFormGroup.reset();
            
            this.showMemberDetails = false;
          }, error =>  error => {
            this.formErrors = error;
        });
        
    

    this.AddKpointsFlag=false;
  }
  exportToPdf() {
        if(this.temp.length!=0){
            var doc = new jsPDF();
            var rows = [];
            var col = ['sno','MemberId','Customer Name','EmailID','Subscribed Plan','Earned KPoints','Manual KPoints','Available KPoints'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].CustomerNo,this.temp[key].CustomerName,this.temp[key].EmailID, this.temp[key].SubscribedPlan,this.temp[key].TotalEarnedKPoints, this.temp[key].TotalManualKPoints, this.temp[key].CurrentAvailableKPoints];
                rows.push(temporary);
            }
            let reportname = "Kpoints.pdf"
            this.exportService.exportAsPdf(col,rows,reportname);
        }
        else {
            this.sendReceiveService.showDialog('There is No Data Available to Export');
        }
    }
  exportToExcel() {
    if (this.excelSalesData.length > 0) {
       var rows = [];
       for(var key in this.temp){
        var temporary = [(parseInt(key) +1), this.temp[key].CustomerNo,this.temp[key].CustomerName,this.temp[key].EmailID, this.temp[key].SubscribedPlan,this.temp[key].TotalEarnedKPoints, this.temp[key].TotalManualKPoints, this.temp[key].CurrentAvailableKPoints];
        rows.push(temporary);
    }
            var createXLSLFormatObj = [];
            var xlsHeader = ['sno','MemberId','Customer Name','EmailID','Subscribed Plan','Earned KPoints','Manual KPoints','Available KPoints'];
            createXLSLFormatObj.push(xlsHeader);
         $.each(rows, function(index, value) {
                var innerRowData = [];
                $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MemberId +  "</td><td>" + value.CustomerName +  "</td><td>" + value.Email +  "</td><td>" + value.SubscribedPlan +  "</td><td>" + value.TotalEarnedKPoints +  "</td><td>" +value.TotalManualKPoints + "</td><td>"+value.CurrentAvailableKPoints +"</td></tr>");
                $.each(value, function(ind, val) {
        
                    innerRowData.push(val);
                });
                createXLSLFormatObj.push(innerRowData);

            });      
            
            var filename = "Kpoints.xlsx"; 
            var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
            const workbook: XLSX.WorkBook = { Sheets: { 'Kpoints': ws }, SheetNames: ['Kpoints'] };
            XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
        
           
    } else {
      alert("No recordes found");
     
    }


  }
}