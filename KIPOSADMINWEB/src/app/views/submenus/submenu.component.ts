
import { Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { MatDialogRef, MatDialog, MatDialogConfig, MatTableDataSource, MatPaginator, MatSort } from '@angular/material';
import { Router, ActivatedRoute } from '@angular/router';
import { AppInfoService } from '../../services/common/appInfo.service';
import { SendReceiveService } from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { SubMenuService } from './submenu.service';
import { IPageLevelPermissions, SubMenuModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';
import { MenuService } from '../menus/menu.service';
import * as $ from "jquery";
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { DatePipe } from '@angular/common';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { resolve } from 'url';

@Component({
    selector: 'app-submenu',
    templateUrl: './submenu.component.html',
    styleUrls: ['./submenu.component.css']
})
export class SubMenuComponent implements OnInit {
    id: any;
    addSubMenuFlag: boolean;
    displayedColumns = ['sno', 'SubMenuName', 'SubMenuUrl', 'Actions']
    dataSource: MatTableDataSource<SubMenuModel>;
    gridData = [];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1: boolean = false;
    messageFlag2: boolean = false;
    filterData;
    formErrors: any;
    title: string;
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    SubMenuForm: FormGroup;
    ID: number = 0;
    userId: number = 0;
    idOnUpdate: number = 0;
    pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
    menuname: string;
    currentPageLimit: number = 0;
    limit = MyAppHttp.LIMIT;
    pageLimitOptions = MyAppHttp.PAGE_LIMIT_OPTIONS
    RoleForm: FormGroup;
    roleList = []
    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public ref: ChangeDetectorRef,
        public service: SubMenuService, public menuservice: MenuService,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public datepipe:DatePipe,
        public exportService : ExportService,
        public translate: TranslateService,
        private route: ActivatedRoute,
        private formBuilder: FormBuilder) {
        this.formErrors = {
            subMenuName: {},
            subMenuUrl: {},
        };
    }
    ngOnInit() {
           document.getElementById('preloader-div').style.display = 'block';
        this.userId = this.sendReceiveService.globalUserId;
        this.filterData = {
            filterColumnNames: [
                { "Key": 'sno', "Value": " " },
                { "Key": 'SubMenuName', "Value": " " },
                { "Key": 'SubMenuUrl', "Value": " " },
            ],
            gridData: this.gridData,
            dataSource: this.dataSource,
            paginator: this.paginator,
            sort: this.sort
        };
    
        this.SubMenuForm = this.formBuilder.group({
            id: 0,
            'subMenuName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]), this.duplicateSubMenu.bind(this)],
            'subMenuUrl': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces)])],
        });
        this.RoleForm = this.formBuilder.group({
            'id': 0,
            'role': [null, Validators.required],
        });
        let paramId = +this.route.snapshot.params['id'];
        if (!paramId) {
            this.menuservice.getAllMenus().subscribe((response) => {
                const roleData: any = [];
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1;
                    roleData.push(response[i]);
                }

                this.roleList = roleData;
                this.id = roleData[0].MenuId;
                if(this.id){
                    this.getAllSubMenus();
                }
               
            })
        } else {
            this.menuservice.getAllMenus().subscribe((response) => {
                const roleData: any = [];
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1;
                    roleData.push(response[i]);
                }

                this.roleList = roleData;
                this.id = paramId;
                if(this.id){
                    this.getAllSubMenus();
                }
            })
        }

        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
            this.pagePermissions = pageLevelPermissions.response;
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
    }

    updatePagination(){
        this.filterData.dataSource=this.filterData.dataSource;
        this.filterData.dataSource.paginator = this.paginator;
        }

    private getAllSubMenus() {
        document.getElementById('preloader-div').style.display = 'block';
        this.addSubMenuFlag = false;
        this.service.getSubMenuByMenuId(this.id).subscribe((response) => {
            this.temp=response;
            const submenuData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                submenuData.push(response[i]);
            }
            this.filterData.gridData = submenuData;
            this.dataSource = new MatTableDataSource(submenuData);
            this.filterData.dataSource = this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;

            this.addSubMenuFlag = false;

            document.getElementById('preloader-div').style.display = 'none';

        }, (error) => {

               document.getElementById('preloader-div').style.display = 'none';
        });

    }

    onSubMenuSubmit() {

        let var_id: string = this.SubMenuForm.value.id;
        let subMenuName: string = this.SubMenuForm.value.subMenuName;
        let subMenuUrl: string = this.SubMenuForm.value.subMenuUrl;
        if (!this.SubMenuForm.valid) {
            return;
        }
        if (this.title == "Save") {

            this.service.saveSubMenu({
                'SubMenuName': subMenuName,
                'SubMenuUrl': subMenuUrl,
                'MenuId': this.id,
                'IsActive': MyAppHttp.ACTIVESTATUS,
                'CreatedBy': this.userId,
                'ModifiedBy': this.userId,
            }).subscribe((data) => {
                this.getAllSubMenus();
                this.SubMenuForm.reset();
            }, error => error => {
                this.formErrors = error

            });
        }
        else if (this.title == "Update") { // on edit menu form
            this.idOnUpdate = 0;

            this.service.updateSubMenu({
                'SubMenuId': var_id,
                'SubMenuName': subMenuName,
                'SubMenuUrl': subMenuUrl,
                'MenuId': this.id,
                'IsActive': MyAppHttp.ACTIVESTATUS,
                'CreatedBy': this.userId,
                'ModifiedBy': this.userId,
            }, var_id)
                .subscribe((data) => {
                    this.title = "Save";
                    this.SubMenuForm = this.formBuilder.group({
                        id: 0,
                        'subMenuName': [null, Validators.compose([Validators.required, Validators.minLength(3)]), this.duplicateSubMenu.bind(this)],
                        'subMenuUrl': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
                    });
                    this.getAllSubMenus();
                    this.SubMenuForm.reset();
                }, error => {
                    this.formErrors = error

                });
        }
    }
    onAddSubMenu() {
        this.addSubMenuFlag = true;
        this.title = "Save";
    }

    onGoBack() {
        this.router.navigate(['/menus']);
    }

    duplicateSubMenu() {
        const q = new Promise((resolve, reject) => {
            this.service.duplicateSubMenu({
                'SubMenuName': this.SubMenuForm.controls['subMenuName'].value,
                'SubMenuId': !!this.idOnUpdate ? this.idOnUpdate : 0,
                'MenuId': this.id,
                'ModifiedBy': null,
                'ModifiedDate': null,
                'IsActive': 1,
                'CreatedBy': null,
                'CreatedDate': null,
            }).subscribe((duplicate) => {
                if (duplicate) {
                    resolve({ 'duplicateSubMenu': true });
                } else {
                    resolve(null);
                }
            }, () => { resolve({ 'duplicateSubMenu': true }); });

        });
        return q;
    }
    processEditAction(id) {
        this.service.getSubMenuById(id)
            .subscribe(resp => {
                this.SubMenuForm.patchValue({
                    id: resp.SubMenuId,
                    subMenuName: resp.SubMenuName,
                    subMenuUrl: resp.SubMenuUrl,
                });
            }
                , error => this.formErrors = error);
    }
    updateSubMenu(id) {
        this.idOnUpdate = id;
        this.addSubMenuFlag = true;
        this.processEditAction(id);
        this.title = "Update";
        this.SubMenuForm = this.formBuilder.group({
            id: 0,
            'subMenuName': [null, Validators.compose([Validators.required]), this.duplicateSubMenu.bind(this)],
            'subMenuUrl': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
        });
    }

    deleteSubMenu(id) {
        this.appInfoService.confirmationDialog().subscribe(result => {
            if (!!result) {
                this.service.deleteSubMenu(id).subscribe((data) => {
                    this.getAllSubMenus();
                }, error => {

                });

            }
        });
    }
    onCancel() {
        this.idOnUpdate = 0;
        this.addSubMenuFlag = false;
        this.SubMenuForm.reset();
    }
    onRoleSubmit(){
        let var_roles :number = this.RoleForm.value.role;
        if (!this.RoleForm.valid) {
            return false;
        }
      
        this.sendReceiveService.getKeyValueName(var_roles).subscribe(resp => {
            this.menuname = resp;
        this.id =  var_roles;
        this.getAllSubMenus();
        })
    }


    
exportToPdf() {
  
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
       
          var col = ['sno','SubMenu Name','SubMenu Url'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].SubMenuName,this.temp[key].SubMenuUrl];
                rows.push(temporary);
            }
            let reportname = "SubMenu.pdf"
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
            var temporary = [(parseInt(key) +1), this.temp[key].SubMenuName,this.temp[key].SubMenuUrl];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','subMenu Name','SubMenu Url'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.SubMenuName +  "</td><td>" + value.SubMenuUrl + "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "SubMenu.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'SubMenu': ws }, SheetNames: ['SubMenu'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }
  
}