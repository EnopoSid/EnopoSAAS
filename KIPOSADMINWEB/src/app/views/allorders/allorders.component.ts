import { Component, OnInit, ViewChild, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort, MatDialogRef, MatDialog } from '@angular/material';
import { EnquiryModel, IPageLevelPermissions } from 'src/app/helpers/common.interface';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { TranslateService } from '@ngx-translate/core';
import { DatePipe } from '@angular/common';
import { Subject } from 'rxjs';
import { ALLOrdersService } from './allorders.service';
import *  as $ from 'jquery'  ;
import * as XLSX from 'xlsx';
@Component({
  selector: 'app-allorders',
  templateUrl: './allorders.component.html',
  styleUrls: ['./allorders.component.css']
})
export class ALLOrdersComponent implements OnInit {
  displayedColumns = ['sno','OrderId','SiteName','CustomerName','EmailID','OrderDate','DeliveryDate','OrderAmount','StoreName','ShippingOption','AppliedKPoints','AppliedCouponName']
  dataSource: MatTableDataSource<EnquiryModel>;
  gridData =[];
  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: false}) sort: MatSort;
  messageFlag1:boolean=false;
  messageFlag2:boolean=false;
  filterData;
  private currentComponentWidth: number;
  rows = [];
  columns = [];
  temp = [];
  permisssionList:any;
  orderForm: FormGroup;
  dialogRef: MatDialogRef<any>;
  usersFromServiceExists ;
  sample : string;
  pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};

  constructor(private spinner: NgxSpinnerService,
              private router: Router,
              private route:ActivatedRoute,
              public service: ALLOrdersService,
              public ref: ChangeDetectorRef,
              public dialog: MatDialog,
              public viewContainerRef: ViewContainerRef,
              public logoutService: LogoutService,
              public appInfoService: AppInfoService,
              private formBuilder:FormBuilder,
              public sendReceiveService: SendReceiveService,
              public translate: TranslateService,  
              private actRoute: ActivatedRoute,
              public datepipe:DatePipe,
              private activatedRoute: ActivatedRoute,) {
  }



  ngOnInit() {
      this.rows = [];
      this.filterData={
          filterColumnNames:[
            {"Key":'sno',"Value":" "},
            {"Key":'OrderId',"Value":" "},
            {"Key":'SiteName',"Value":" "},
            {"Key":'CustomerName',"Value":" "},
            {"Key":'EmailID',"Value":" "},
            {"Key":'OrderDate',"Value":" "},
            {"Key":'DeliveryDate',"Value":" "},
            {"Key":'OrderAmount',"Value":" "},
            {"Key":'StoreName',"Value":" "},
            {"Key":'ShippingOption',"Value":" "},
            {"Key":'AppliedKPoints',"Value":" "},
            {"Key":'AppliedCouponName',"Value":" "}
                     
          ],
          gridData:  this.gridData,
          dataSource: this.dataSource,
          paginator:  this.paginator,
          sort:  this.sort
        };
        this.orderForm = this.formBuilder.group({
          'orderstatusId':  [null]
      });
      this.permissiontypeList();
      this.AllOrders();
      this.sendReceiveService.globalPageLevelPermission = new Subject;
      this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
      this.pagePermissions =pageLevelPermissions.response;  
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
  });
  }
  getAllOrders() {
    throw new Error("Method not implemented.");
  }
  

  handlePageChange (event: any): void {
  }

  openResume(filePath) {
      window.open(filePath);
  }


  AllOrders() {
    document.getElementById('preloader-div').style.display = 'block';
        this.service.Allorders().subscribe((response) => {
            this.temp=response;
            const orderData: any = [];              
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                response[i].editMode = false;
                if(!!response[i].PickUpInStore){
                    response[i].PickUpInStore="PickUp"
                }else{
                    response[i].PickUpInStore="Delivery"
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

updatePagination(){
   this.filterData.dataSource=this.filterData.dataSource;
   this.filterData.dataSource.paginator = this.paginator;
  }
  
permissiontypeList(){

  
    }
actionAfterError () {
    this.dialogRef.afterClosed().subscribe(result => {
        this.appInfoService.setSecurity();
        this.sendReceiveService.setSequence(1);
        this.router.navigate(['/sessions/signin']);
    });
}
editorderPermission(row){
    this.rows.forEach(element => {
      if(element.OrderId == row.OrderId)
      element.editMode = true;
      else
      element.editMode = false;
    });
}


exportToExcel() {
 
  if(this.temp.length!=0){
      
       var rows = [];
     
      for(var key in this.temp){
          var temporary = [(parseInt(key) +1), this.temp[key].OrderId,this.temp[key].SiteName,this.temp[key].CustomerName,this.temp[key].EmailID,(this.datepipe.transform(this.temp[key].OrderDate, 'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].deliverydate, 'yyyy/MM/dd')),this.temp[key].OrderAmount, this.temp[key].StoreName,this.temp[key].AppliedKPoints, this.temp[key].AppliedCouponName];
          rows.push(temporary);
      }
      var createXLSLFormatObj = [];
      var xlsHeader =['sno','OrderId','SiteName','CustomerName','EmailID','OrderDate','DeliveryDate','OrderAmount(GST included)','StoreName','AppliedKPoints','AppliedCouponName']
      createXLSLFormatObj.push(xlsHeader);
      $.each(rows, function(index, value) {
             var innerRowData = [];
            $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MemberId +  "</td><td>" + value.FullName +  "</td><td>" + value.Email +  "</td><td>" + value.MobileNumber +   "</td><td>" +value.CreatedDate+ "</td><td>" +value.EmailID+ "</td><td>" +value.phoneNumber+ "</td><td>" +value.StoreName+"</td><td>" +value.IsCouponApply+ "</td></tr>");

         
             $.each(value, function(ind, val) {
     
                 innerRowData.push(val);
             });
             createXLSLFormatObj.push(innerRowData);
         });
         var filename = "Allorder.xlsx";
         var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
         const workbook: XLSX.WorkBook = { Sheets: { 'Allorder': ws }, SheetNames: ['Allorder'] };
         XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
     }


  
  else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
}


}

