import { Component, OnInit } from '@angular/core';  
import { ViewChild } from '@angular/core';
import { MatDialogRef, MatDialog, MatTableDataSource, MatPaginator, MatSort } from '@angular/material';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { IPageLevelPermissions, MenuModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { MenuService } from '../../views/menus/menu.service';
import { ChangeDetectorRef } from '@angular/core';
import { ViewContainerRef } from '@angular/core';
import { AppInfoService } from '../../services/common/appInfo.service';
import { SendReceiveService } from '../../services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { FormBuilder } from '@angular/forms';
import { Validators } from '@angular/forms';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';
import { NavigationService } from '../../services/navigation/navigation.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import * as $ from 'jquery';
import * as XLSX from  'xlsx';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { DatePipe } from '@angular/common';
declare var jsPDF:any;


  
@Component({
  selector: 'app-menus',  
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenusComponent implements OnInit {
    displayedColumns = ['sno','MenuName','MenuUrl','Actions']
    dataSource: MatTableDataSource<MenuModel>;
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
    AddMenuFlag:boolean=false;
    MenuForm:FormGroup;
    userId: number = 0;
    idOnUpdate:number=0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    public customPatterns = {'0': { pattern: new RegExp('\[a-zA-Z\]')}};

  constructor(
      private router: Router,
      private spinner: NgxSpinnerService,
      public service: MenuService,
      public  navservice  :NavigationService,
      public ref: ChangeDetectorRef,
      public dialog: MatDialog,
      public viewContainerRef: ViewContainerRef,
      public logoutService: LogoutService,
      public appInfoService: AppInfoService,
      public exportService:ExportService,
      public datepipe:DatePipe,
      public sendReceiveService: SendReceiveService,
      public translate: TranslateService,
      private formBuilder:FormBuilder 
    ) {
          this.formErrors = {
              menuName: {},
              menuUrl: {}  
          };
}

  ngOnInit() {
    this.userId=this.sendReceiveService.globalUserId;
    this.filterData={  
        filterColumnNames:[
          {"Key":'sno',"Value":" "},
          {"Key":'MenuName',"Value":" "},
          {"Key":'MenuUrl',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
    this.getAllMenus();
    this.MenuForm = this.formBuilder.group({
        id: 0,
        'menuName':  [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]),this.duplicateMenuName.bind(this)],
        'menuUrl': ['',Validators.compose([ Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])]
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


  getAllMenus(){ 
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllMenus().subscribe((response) => {
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
          document.getElementById('preloader-div').style.display = 'none';
    }, () => {
        
    });
}

actionAfterError() {
    this.dialogRef.afterClosed().subscribe(result => {
        this.appInfoService.setSecurity();
        this.sendReceiveService.setSequence(1);
        this.router.navigate(['/sessions/signin']);
    });
}

processEditAction(id){
    this.idOnUpdate=id;
    this.service.getMainMenuById(id)
        .subscribe(resp => {
            this.MenuForm.patchValue({
                id: resp.MenuId,
                menuName: resp.MenuName,
                menuUrl: resp.MenuUrl,
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
            'MenuName': var_menu,
            'MenuUrl': var_menu_url,
            'IsActive': 1,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
            'MenuColor': '#2b84ee',
        }).subscribe((data) => {
            this.getAllMenus();
            this.MenuForm.reset();
        }, error =>  error => {
            this.formErrors = error;
        });
        
    }
    else if (this.title == "Update") { // on edit menu form
        this.idOnUpdate=0;
        this.service.updateMainMenu({
            "MenuId":var_id,
            'MenuName': var_menu,
            'MenuUrl': var_menu_url,
            'IsActive': 1,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
            'MenuColor': '#2b84ee',
        }, var_id)
            .subscribe((data) => {
               this.title = "Save";
               this.MenuForm = this.formBuilder.group({
                id: 0,
                'menuName':  [null, Validators.compose([Validators.required, Validators.minLength(3)]),this.duplicateMenuName.bind(this)],
               'menuUrl' : ['', Validators.required]
            });
               this.getAllMenus();
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
                this.getAllMenus();
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
            'MenuId':!!this.idOnUpdate ? this.idOnUpdate: 0,
            'MenuName':this.MenuForm.controls['menuName'].value,
            'MenuUrl': null,
            'IsActive': 1,
            'CreatedBy': null,
            'ModifiedBy': null,  
            'MenuColor': null,
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
       
          var col = ['SNO','Menu Name','Menu URL'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].MenuName,this.temp[key].MenuUrl];
                rows.push(temporary);
            }
            let reportname = "Menu.pdf"
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
                var temporary = [(parseInt(key) +1), this.temp[key].MenuName,this.temp[key].MenuUrl];
                rows.push(temporary);
            }
    
        var createXLSLFormatObj = [];
        var xlsHeader = ['SNO','Menu Name','Menu URL'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MenuName+  "</td><td>" + value.MenuUrl + "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "Menu.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'Menu': ws }, SheetNames: ['Menu'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }
  




}

