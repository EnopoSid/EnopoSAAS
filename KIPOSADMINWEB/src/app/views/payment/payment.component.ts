import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { paymentService } from './payment.service';
import {Router} from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, PaymentModel } from '../../helpers/common.interface';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';
import { NavigationService } from 'src/app/services/navigation/navigation.service';
import * as $ from "jquery";
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { DatePipe } from '@angular/common';
import { ExportService } from 'src/app/services/common/exportToExcel.service';



@Component({
    selector: 'payment-table',
    templateUrl: './payment.component.html',
    styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {
    status: boolean;
    displayedColumns = ['sno','PaymentTypeId','PaymentTypeName','Actions']
    dataSource: MatTableDataSource<PaymentModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    formErrors: any;
    title: string;
    rows = [];
    columns = [];
    temp = []; 
    dialogRef: MatDialogRef<any>;
    addPermissionFlag:boolean=false;
    AddMenuFlag:boolean=false;
    MenuForm:FormGroup;
    userId: number = 0;
    idOnUpdate: number =0;
    id;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    currentPageLimit: number = 0;
    limit = MyAppHttp.LIMIT;
    pageLimitOptions = MyAppHttp.PAGE_LIMIT_OPTIONS

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: paymentService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public  navservice  :NavigationService,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public exportService:ExportService,
        public datepipe:DatePipe,
        public translate: TranslateService,
        private formBuilder:FormBuilder) {
            this.formErrors = {
                PaymentTypeName: {},
            };
}

    
ngOnInit(){
   
    this.userId=this.sendReceiveService.globalUserId;
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'PaymentTypeId',"Value":" "},
              {"Key":'PaymentTypeName',"Value":" "},
               ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
         
    this.getAllPayment();
    this.MenuForm = this.formBuilder.group({
        id: 0,
        'menuName':  [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]),this.duplicateMenuName.bind(this)],
        'menuUrl': ['',Validators.compose([ Validators.required])]
    });
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
}

updatePagination(){
    this.filterData.dataSource=this.filterData.dataSource;
    this.filterData.dataSource.paginator = this.paginator;
    }
    
getAllPayment(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllPayments().subscribe((response) => { 
        this.temp=response;
        const menuData: any = [];
        for (let i = 0; i < response.length; i++) {
            response[i].sno = i + 1;
            menuData.push(response[i]);
        }
        this.filterData.gridData = menuData;
        this.dataSource = new MatTableDataSource(menuData);
        this.filterData.dataSource=this.dataSource;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
          document.getElementById('preloader-div').style.display = 'none';
    }, (error) => {
        console.log(error);
        document.getElementById('preloader-div').style.display = 'none';
    });
}



addPermission(){
    this.addPermissionFlag=true;
    this.title="Save"
}
processEditAction(id){
    this.idOnUpdate=id;
    this.service.getMainMenuById(id)
        .subscribe(resp => {
            this.id =resp.Id;
            this.MenuForm.patchValue({
                id: resp.Id,
                menuName: resp.PaymentTypeName,
                menuUrl: resp.PaymentTypeId,
            });
        },  error => this.formErrors = error);
}
updateMainmenu(id){
    this.AddMenuFlag = true;
    this.processEditAction(id);
    this.title = "Update";
    this.MenuForm = this.formBuilder.group({
        id:0,
        'menuName':  [null, Validators.compose([Validators.required, Validators.minLength(3)]),this.duplicateMenuName.bind(this)],
        'menuUrl' : ['', Validators.required]
       });
       this.navservice.getAllMenuItems;
}

addMenu(){
this.AddMenuFlag=true;
this.title="Save";

}
MenuFormSubmit() {
    let var_id: string = this.MenuForm.value.id;
    let var_menu: string = this.MenuForm.value.menuName;
    let var_menu_url: string = this.MenuForm.value.menuUrl;
    if (!this.MenuForm.valid) {
        return;
    }
    if (this.title == "Save") { 
        this.service.saveMainMenu({
           "PaymentTypeId": var_menu_url,
            "PaymentTypeName": var_menu,
            "IsActive": true,
            "CreatedDate": "",
            "ModifiedDate": "",
            "CreatedBy": this.userId,
            "ModifiedBy": this.userId,



        }).subscribe((data) => {
           this.getAllPayment();
            this.MenuForm.reset();
        }, error =>  error => {
            this.formErrors = error;
        });
        
    }
    else if (this.title == "Update") { // on edit menu form
        this.idOnUpdate=0;
        this.service.updateMainMenu({
            "PaymentTypeId": var_menu_url,
            "PaymentTypeName": var_menu,
            "IsActive": true,
            "CreatedDate": "",
            "ModifiedDate": "",
            "CreatedBy": this.userId,
            "ModifiedBy": this.userId,
        }, this.id)
            .subscribe((data) => {
               this.title = "Save";
               this.MenuForm = this.formBuilder.group({
                id: 0,
                'menuName':  [null, Validators.compose([Validators.required, Validators.minLength(3)]),this.duplicateMenuName.bind(this)],
               'menuUrl' : ['', Validators.required]
            });
            this.getAllPayment();
               this.MenuForm.reset();
            }, error => {
                this.formErrors = error;
            });
            
    } 
    this.AddMenuFlag=false;
}
deleteMainmenu(id) {
    this.appInfoService.confirmationDialog().subscribe(result=>{
        if(!!result){
            this.service.deleteMainMenu(id)
            .subscribe((data) => {
                this.getAllPayment();
            }, error => {
                this.formErrors = error
            });
        
        }
    });
}
duplicateMenuName(){
    let var_menu: string = this.MenuForm.value.menuName;
    const q = new Promise((resolve, reject) => {
        this.service.duplicateMenuName({
            "PaymentTypeId": !!this.idOnUpdate ? this.idOnUpdate: 0,
            "PaymentTypeName": this.MenuForm.controls['menuName'].value,
            "IsActive": true,
            "CreatedDate": "",
            "ModifiedDate": "",
            "CreatedBy": null,
            "ModifiedBy": null,
      }).subscribe((duplicate) => {
            if (duplicate) {
                resolve({ 'duplicateMenuName': true });
            } else {
                resolve(null);
            }
        }, () => { resolve({ 'duplicateMenuName': true }); });
    });
    return q;
}
onCancel(){
    this.idOnUpdate=0;
    this.MenuForm.reset();
    this.AddMenuFlag=false;
}
exportToPdf() {
  
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
       
          var col = ['sno','PaymentID','Payment Options'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].PaymentTypeId,this.temp[key].PaymentTypeName];
                rows.push(temporary);
            }
            let reportname = "PaymentOptions.pdf"
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
            var temporary = [(parseInt(key) +1), this.temp[key].PaymentTypeId,this.temp[key].PaymentTypeName];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','PaymentID','Payment Options'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.PaymentTypeId+  "</td><td>" + value.PaymentTypeName +  "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "PaymentOptions.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'PaymentOptions': ws }, SheetNames: ['PaymentOptions'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }




}
