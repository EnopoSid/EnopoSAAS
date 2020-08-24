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
import { cancelorderService } from './cancelorder.service';
import { TranslateService } from '@ngx-translate/core';
import * as $ from "jquery";
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { Subject } from 'rxjs';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { DatePipe } from '@angular/common';
@Component({
    selector: 'app-rolepermissions',
    templateUrl: './cancelorder.component.html'
})
export class CancelOrderComponent implements OnInit {
    displayedColumns = ['sno','CustomerName','MemberId','OrderId','PaymentTypeName','OrderDate','StoreName','EmailId','MobileNumber','HavingHere','ModifiedDate']
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
    editRole:boolean = true; 
    rolepermissions:any;
    PermissionId: string;
    role: string;
    permissionId:number;
    selectedRow : Number;
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

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: cancelorderService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
         public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        private formBuilder:FormBuilder,
        public exportService:ExportService,
        public datepipe:DatePipe,
        private route:ActivatedRoute,
        private _router: Router,) { 
        }
  
    ngOnInit() {
      this.userId=this.sendReceiveService.globalUserId;
      this.filterData={
        filterColumnNames:[
            {"Key":'sno',"Value":" "},
            {"Key":'CustomerName',"Value":" "},
            {"Key":'MemberId',"Value":" "},
            {"Key":'OrderId',"Value":""},
            {"Key":'PaymentTypeName',"Value":" "},
            {"Key":'OrderDate',"Value":" "},
            {"Key":'StoreName',"Value":" "},
            {"Key":'EmailId',"Value":" "},
            {"Key":'MobileNumber',"Value":" "},
            {"Key":'HavingHere',"Value":" "},
            {"Key":'ModifiedDate',"Value":" "},
          
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
      this.permissiontypeList();
        this.getAllCancelorders();
       


    }
    getAllCancelorders() {
        document.getElementById('preloader-div').style.display = 'block';
      this.service.getCancelorders().subscribe((response) => {
        this.temp=response;
          const orderData: any = [];              
          for (let i = 0; i < response.length; i++) {
              response[i].sno = i + 1;
              response[i].editMode = false;
              if(!!response[i].HavingHere){
                  response[i].HavingHere="Having Here"
              }else{
                  response[i].HavingHere="Take Away"
              }
              if(!!response[i].IsCouponApply){
                  response[i].IsCouponApply="Applied"
              }else{
                  response[i].IsCouponApply="NA"
              }

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


  permissiontypeList(){
    document.getElementById('preloader-div').style.display = 'block';
  this.service.getRefundOrderListItems().subscribe(resp => {
   this. permisssionList  = resp;
   console.log(this.permisssionList)
     document.getElementById('preloader-div').style.display = 'none';
  
  })
  }
  editPermission(row){
  this.rows.forEach(element => {
    if(element.ID == row.ID)
    element.editMode = true;
    else
    element.editMode = false;
  });

  var tempPermissionValue =  $.grep(this.permisssionList,function(e){return e.PermissionId == row.PermissionId})
  this.RolePermissionForm.controls['RefundStatusId'].setValue(tempPermissionValue.length  > 0 ? tempPermissionValue[0] : {});
    }
  cancelRolePermission(row){
    row.editMode = false;
   this.permissionType = false;
  }
  onChange(event): void{
    this.permissionname = event.value;
  }
  
  

  onGoBack(){
    this._router.navigate(['../'],{relativeTo: this.route}); 
  }
  updatePagination(){
   this.filterData.dataSource=this.filterData.dataSource;
   this.filterData.dataSource.paginator = this.paginator;
  }


  exportToPdf() {

    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
       
          var col = ['sno','Customer Name','Member Id','OrderId','PaymentType Name','Order Date','Store Name','EmailId','Mobile Number','Having  Here','Modified   Date'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].CustomerName,this.temp[key].MemberId,this.temp[key].OrderId,this.temp[key].PaymentTypeName,(this.datepipe.transform(this.temp[key].OrderDate, 'yyyy/MM/dd')),this.temp[key].StoreName,this.temp[key].EmailId,this.temp[key].MobileNumber,this.temp[key].HavingHere,this.temp[key].ModifiedDate];
                rows.push(temporary);
            }
            let reportname = "Cancelorders.pdf"
            this.exportService.exportAsPdf(col,rows,reportname);
        
     
       
    }
    else {

        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
}
exportToExcel() {
    if(this.temp.length!=0 ){
        var rows = [];

        
        for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].CustomerName,this.temp[key].MemberId,this.temp[key].OrderId,this.temp[key].PaymentTypeName,(this.datepipe.transform(this.temp[key].OrderDate, 'yyyy/MM/dd')),this.temp[key].StoreName,this.temp[key].EmailId,this.temp[key].MobileNumber,this.temp[key].HavingHere,this.temp[key].ModifiedDate];
            rows.push(temporary);
        }

        var createXLSLFormatObj = [];
        var xlsHeader = ['sno','Customer Name','Member Id','OrderId','PaymentType Name','Order Date','Store Name','EmailId','Mobile Number','Having  Here','Modified   Date'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.CustomerName +  "</td><td>" + value.MemberId +  "</td><td>" + value.OrderId +  "</td><td>" + value.PaymentTypeName +   "</td><td>" +value.orderData+"</td><td>" + value.StoreName +"</td><td>" + value.EmailId +"</td><td>" + value.MobileNumber+"</td><td>" + value.HavingHere +"</td><td>" + value.ModifiedDate + "</td></tr>");

        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "Cancelorders.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'Cancelorders': ws }, SheetNames: ['Cancelorders'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }







}