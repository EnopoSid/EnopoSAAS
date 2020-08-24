import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator,MatSort} from '@angular/material';
import { LogoutService} from '../../services/logout/logout.service';
import {Route, ActivatedRoute, Router} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import {IPageLevelPermissions, EnquiryModel } from '../../helpers/common.interface';
import {NgxSpinnerService} from 'ngx-spinner';
import {TranslateService} from '@ngx-translate/core';
import {Subject} from 'rxjs/internal/Subject';
import {ExportService } from '../../services/common/exportToExcel.service';
declare var jsPDF: any;  
import * as XLSX from 'xlsx';
import * as $ from "jquery";
import { DatePipe } from '@angular/common'
import { CorpMasterService } from './corp-master.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-corp-master',
  templateUrl: './corp-master.component.html',
  styleUrls: ['./corp-master.component.css']
})
export class CorpMasterComponent implements OnInit {

    displayedColumns =  ['sno','CompanyName','Domain','RegisteredDate','CouponCode','Actions']
    dataSource: MatTableDataSource<EnquiryModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    Domainform:FormGroup;
    formErrors: any;
    title: string;
    userId:number=0;
    idOnUpdate: number = 0;
    AddDiscountFlag:boolean=false;
    private currentComponentWidth: number;
    rows = [];
    columns = [];
    Discounts=[];
    temp = [];
    dialogRef: MatDialogRef<any>;
    usersFromServiceExists ;
    sample : string;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    constructor(private spinner: NgxSpinnerService,
        private router: Router,
        private route:ActivatedRoute,
        public service: CorpMasterService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public exportService:ExportService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,  
        private actRoute: ActivatedRoute,
        private activatedRoute: ActivatedRoute,
        public datepipe: DatePipe,
        private formBuilder:FormBuilder) {  
          this.formErrors = {
              region: {}
          }; 
}
ngOnInit() {
    this.userId = this.sendReceiveService.globalUserId;
    this.filterData={
        filterColumnNames:[
            {"Key":'sno',"Value":" "},
            {"Key":'CompanyName',"Value":" "},
            {"Key":'Domain',"Value":" "},
            {"Key":'RegisteredDate',"Value":" "},
            {"Key":'CouponCode',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
    this.getAllCorpMaster();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
    this.pagePermissions =pageLevelPermissions.response;  
   this.sendReceiveService.globalPageLevelPermission.unsubscribe();
});
this.Domainform = this.formBuilder.group({
  'CompanyName':  [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]), this.duplicateCompanyValue.bind(this)],
  'DoMain': ['',Validators.compose([ Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.Domain)]),this.duplicateDomainValue.bind(this)] ,
  'Discount':  [null,Validators.required],
});

}

handlePageChange (event: any): void {
}

openResume(filePath) {
    window.open(filePath);
}

updatePagination(){
    this.filterData.dataSource=this.filterData.dataSource;
    this.filterData.dataSource.paginator = this.paginator;
    }


getAllCorpMaster() {
    document.getElementById('preloader-div').style.display = 'block';
        this.service.getAllDomain().subscribe((response) => {
            this.temp = response;
            const memberData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                memberData.push(response[i]);
            }
            this.filterData.gridData = memberData;
            this.dataSource = new MatTableDataSource(memberData);
            this.filterData.dataSource=this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
              document.getElementById('preloader-div').style.display = 'none';
        }, (error) => {
              document.getElementById('preloader-div').style.display = 'none';
        });
}
onDomainSubmit() {
  let var_id: string = this.Domainform.value.id;
  let CompanyName: string = this.Domainform.value.CompanyName;
  let Domain: string = this.Domainform.value.DoMain;
  let Discount: string = this.Domainform.value.Discount;

  if (!this.Domainform.valid) {
      return;
  }
  
  if (this.title == "Save") {

      this.service.SaveDomain({
          'CompanyName': CompanyName,
          'Domain': Domain,
          'CouponCode': Discount,
          'StatusId': 1,
          'CreatedBy': this.userId,
      }).subscribe((data) => {
          this.Domainform.reset();
          this.AddDiscountFlag=false;
          this.getAllCorpMaster();

      }, error => error => {
          this.formErrors = error

      });
  }
  else if (this.title == "Update") { 
      this.idOnUpdate = 0;
      this.service.updateDomain({
          'Id': var_id,        
          'StatusId': 1,
        'CompanyName': CompanyName,
        'Domain': Domain,
        'CouponCode': Discount,
          'ModifiedBy': this.userId,
      })
          .subscribe((data) => {
              this.title = "Save";
              this.AddDiscountFlag=false;
              this.Domainform = this.formBuilder.group({
                  id: 0,
                  'CompanyName':  [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]), this.duplicateCompanyValue.bind(this)],
                  'DoMain': ['',Validators.compose([ Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.Domain)]),this.duplicateDomainValue.bind(this)],
                  'Discount':  [null,Validators.required],
              });
              this.Domainform.reset();
              this.getAllCorpMaster();

          }, error => {
              this.formErrors = error

          });
  }
}
processEditAction(id) {
  this.GetAllDiscounts();

  this.service.getAllDomainByid(id)
      .subscribe(resp => {
          this.Domainform.patchValue({
              id: resp[0].Id,
              CompanyName: resp[0].CompanyName,
              DoMain: resp[0].Domain,
              Discount:resp[0].CouponCode,

              
          });
      }
          , error => this.formErrors = error);
}
updateCorpMaster(id) {
    this.idOnUpdate = id;
  this.AddDiscountFlag = true;
  this.processEditAction(id);
  this.title = "Update";
  this.Domainform = this.formBuilder.group({
      id: 0,
      'CompanyName':  [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]), this.duplicateCompanyValue.bind(this)],
      'DoMain': ['',Validators.compose([ Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.Domain)]),this.duplicateDomainValue.bind(this)],
      'Discount':  [null,Validators.required],
  });
}
duplicateDomainValue() {
  const q = new Promise((resolve, reject) => {
      this.service.duplicateDomain({
          'CompanyName': this.Domainform.controls['CompanyName'].value,
          'Id': !!this.idOnUpdate ? this.idOnUpdate: 0,
          'Domain':this.Domainform.controls['DoMain'].value,
          'CouponCode':null,
          'RegisteredDate':null,
          'ModifiedBy': null,
          'ModifiedDate': null,
          'StatusId': 1,
          'CreatedBy': null,
          'CreatedDate': null,
      }).subscribe((duplicate) => {
          if (duplicate) {
              resolve({ 'duplicateDomainValue': true });
          } else {
              resolve(null);
          }
      }, () => { resolve({ 'duplicateDomainValue': true }); });

  });
  return q;
}

deleteDomain(id) {
    this.appInfoService.confirmationDialog().subscribe(result=>{
        if(!!result){
            this.service.Delete(id)
            .subscribe((data) => {
                this.getAllCorpMaster();
            }, error => {
                this.formErrors = error
            });
        
        }
    });
}

GetAllDiscounts(){
  document.getElementById('preloader-div').style.display = 'none';
  this.service.getAllDiscounts().subscribe((success)=>{
this.Discounts=success;
  }, (error) => {
  document.getElementById('preloader-div').style.display = 'none';
});
  
}
addMenu(){
  this.GetAllDiscounts();
this.AddDiscountFlag=true;
  this.title="Save";
   
  
}
onCancel(){
  this.Domainform.reset();
  this.AddDiscountFlag=false;
}
duplicateCompanyValue() {
  const q = new Promise((resolve, reject) => {
      this.service.duplicateCompany({
          'CompanyName': this.Domainform.controls['CompanyName'].value,
          'Id': !!this.idOnUpdate ? this.idOnUpdate: 0,
          'Domain':this.Domainform.controls['DoMain'].value,
          'CouponCode':null,
          'RegisteredDate':null,
          'ModifiedBy': null,
          'ModifiedDate': null,
          'StatusId': 1,
          'CreatedBy': null,
          'CreatedDate': null,
      }).subscribe((duplicate) => {
          if (duplicate) {
              resolve({ 'duplicateCompanyValue': true });
          } else {
              resolve(null);
          }
      }, () => { resolve({ 'duplicateCompanyValue': true }); });

  });
  return q;
}

exportToPdf() {
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
        var col =['sno','Names','ContactNo','EmailId','Comments','CreatedDate'];
        for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].Names,this.temp[key].ContactNo,this.temp[key].EmailId,this.temp[key].Comments,(this.datepipe.transform(this.temp[key].CreatedDate, 'yyyy-MM-dd'))];
            rows.push(temporary);
        }
        let reportname = "Contactus.pdf"
        this.exportService.exportAsPdf(col,rows,reportname);
    }
    else {
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
}
exportToExcel(event) {
    if(this.temp.length!=0){
        var rows = [];

        for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].CompanyName,this.temp[key].Domain,(this.datepipe.transform(this.temp[key].RegisteredDate, 'yyyy-MM-dd')),this.temp[key].CouponCode];
            rows.push(temporary);
        }
    
        var createXLSLFormatObj = [];
        var xlsHeader =   ['sno','CompanyName','Domain','RegisteredDate','CouponCode'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.CompanyName +  "</td><td>" + value.Domain +  "</td><td>" + value.RegisteredDate +  "</td><td>" + value.CouponCode + "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "CorpDomain.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'CorpDomain': ws }, SheetNames: ['CorpDomain'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }



    
    else{
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
  }
  tableFormatofData = "";
excelRowCount = 0;
borderClass='border: 1px solid black;border-collapse: collapse;'

  drawRow(rowData) {
    this.tableFormatofData = this.tableFormatofData + "<tr style='border: 1px solid black;border-collapse: collapse;'><td>" + this.excelRowCount + "</td><td>" + rowData.Names + "</td><td>" + rowData.ContactNo + "</td><td>" + rowData.EmailId + "</td><td>" + rowData.Comments + "</td><td>"+(this.datepipe.transform(rowData.CreatedDate, 'yyyy-MM-dd')) + "</td></tr>";
    this.excelRowCount++;
}
  drawTable(data) {
    this.excelRowCount = 1;
    for (var i = 0; i < data.length; i++) {
        if (i == 0) {
            this.tableFormatofData = "<html><head><style> #tblreportdata123 tr th, #tblreportdata123 tr td { text-align: center; }</style></head><body>";
            this.tableFormatofData = this.tableFormatofData + "<table id='tblreportdata123' class='hide'>";
            this.tableFormatofData = this.tableFormatofData + "<thead style='border: 1px solid black;border-collapse: collapse;'><tr style='border: 1px solid black;border-collapse: collapse;' ><th>S.No.</th><th>Names </th><th>ContactNo </th><th>EmailId </th><th>Comments </th><th>CreatedDate</th></tr></thead><tbody>";
        }
        this.drawRow(data[i]);
        if (i == data.length - 1) {
            this.tableFormatofData = this.tableFormatofData + "</tbody></table>";
            this.tableFormatofData = this.tableFormatofData + "</body></html>";
        }
    }
  
}
}