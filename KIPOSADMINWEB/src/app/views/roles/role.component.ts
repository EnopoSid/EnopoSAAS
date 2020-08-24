import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { RoleService } from './role.service';
import { IPageLevelPermissions, RoleModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject, from } from 'rxjs';
import * as XLSX from 'xlsx';
import * as $  from 'jquery';
import { DatePipe } from '@angular/common';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
declare var jsPDF : any; 

@Component({
    selector: 'app-role',
    templateUrl: './role.component.html',
    styleUrls: ['./role.component.css']
})
export class RoleComponent implements OnInit {
    displayedColumns = ['sno','MenuName','Actions']
    dataSource: MatTableDataSource<RoleModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    EditRole: boolean=false;
    RoleId=0;
    cloneselected: boolean;
    updatedata: boolean;
    saveRole: boolean;
    formErrors: any;
    title: string;
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    AddRole:boolean=false;
    RoleForm:FormGroup;
    userId: number = 0;
    roleList = [];
    idOnUpdate:number=0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};

    constructor(
        private spinner: NgxSpinnerService,
        public service: RoleService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        public datepipe:DatePipe,
        public exportService:ExportService,
        private _router: Router,
        private router:ActivatedRoute,
        private formBuilder:FormBuilder) {
            this.formErrors = {
                roleName:{}
            };
        }
    
    ngOnInit(){
        this.userId=this.sendReceiveService.globalUserId;
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'RoleName',"Value":" "},
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
        this.getAllRoles();
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
            this.pagePermissions =pageLevelPermissions.response;  
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
        this.RoleForm = this.formBuilder.group({
            'id': 0,
            'roleName':  ['', Validators.compose([Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]),this.duplicateRole.bind(this)],
          }); 
          
    }
    onSelectedCheckbox(selectedValue){
        if(selectedValue==true){
            let control: FormControl = new FormControl(null, Validators.required);
            this.RoleForm.addControl('role', control);
          }
          else{
            this.RoleForm.removeControl('role');
          }
    }
    
    getAllRoles(){
        document.getElementById('preloader-div').style.display = 'block';
        this.service.getAllRoles().subscribe((response) => {
            this.temp=response;
            const roleData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                roleData.push(response[i]);
            }
            this.filterData.gridData = roleData;
            this.dataSource = new MatTableDataSource(roleData);
            this.roleList = roleData;
            this.filterData.dataSource=this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
              document.getElementById('preloader-div').style.display = 'none';
            
    },(error) => {
        document.getElementById('preloader-div').style.display = 'none';   
        }, () => {
            
        });
    }
    
    actionAfterError() {
        this.dialogRef.afterClosed().subscribe(result => {
            this.appInfoService.setSecurity();
            this.sendReceiveService.setSequence(1);
            this._router.navigate(['/sessions/signin']);
        });
    }
    
    onCheck(){
        this.cloneselected=true;
    }
    duplicateRole(){
        const q = new Promise((resolve, reject) => {
            this.service.duplicateRole({
                'ModifiedBy': null,
                'CreatedBy': null,
                'RoleName':this.RoleForm.controls['roleName'].value,
                'keyValue': null,
                'IsActive':MyAppHttp.ACTIVESTATUS,
                'RoleId': !!this.idOnUpdate ? this.idOnUpdate: 0,
          }).subscribe((duplicate) => {
                if (duplicate) {
                    resolve({ 'duplicateRole': true });
    
                } else {
                    resolve(null);
                }
            }, () => { resolve({ 'duplicateRole': true }); });
        });
        return q;
    
    }
    onRoleSubmit(){
        if (!this.RoleForm.valid) {
            return false;
        }
        let var_id: string = this.RoleForm.value.id;
        let RoleName: string = this.RoleForm.value.roleName;
        let var_roles :number = this.RoleForm.value.role;
        
        if (this.title == "Save") { 
          
          this.saveRole=false;
          this.updatedata = true;
          this.service.SaveRole({
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
            'RoleName':RoleName,
            'IsActive': 1,
            } ).subscribe((data)=>{
                if(this.cloneselected==true)
                {
                   this.RoleId=var_roles;
                }
                this.service.saveRolePermission(this.userId,this.RoleId, data.RoleId).subscribe(rolePermissionResponse =>{
                });
                  this.saveRole = false;
                  this.RoleForm = this.formBuilder.group({
                    id:0,
                    'roleName': ['', Validators.required]
                  });
                  this.getAllRoles();
                  this.cloneselected=false;
                  this.RoleForm.reset();
                  }, error =>  error => {
                      this.formErrors = error;
                    }   
                )
                this.AddRole=false;
                
        }else if (this.title == "Update") { 
                this.idOnUpdate=0;
                
                this.saveRole = false;
                this.service.UpdateRole({
                'CreatedBy': this.userId,
                'ModifiedBy': this.userId,
                'RoleName': RoleName,
                'IsActive':MyAppHttp.ACTIVESTATUS,
                'RoleId': var_id,
                },var_id)
                .subscribe((data) => {
                  this.title = "Save";
                  this.RoleForm = this.formBuilder.group({
                   id:0,
                  'roleName': ['', Validators.required,this.duplicateRole.bind(this)],
                  });
                  this.getAllRoles();
                 this.onCancel();
                  this.RoleForm.reset();
                }, error => {
                    this.formErrors = error
                  });
                  
          }
    }
    addRole(){
        this.AddRole=true;
        this.title="Save";
        this.EditRole=false;
    }

    onCancel(){
        this.idOnUpdate=0;
        this.AddRole=false;
        this.RoleForm.reset();
        this.cloneselected=false;
    }
    deleteRole(id){
        this.appInfoService.confirmationDialog().subscribe(result=>{
            if(!!result){
                this.service.deleteRole(id)
                .subscribe((data) => {
                    this.getAllRoles();
                }, error => {
                    this.formErrors = error
                });
                
            }
        });
    }
      
    updatePagination(){
        this.filterData.dataSource=this.filterData.dataSource;
        this.filterData.dataSource.paginator = this.paginator;
        }
    updateRole(ID){
        
        this.AddRole=true;
        this.EditRole=true;
        this.ProcessEditAction(ID);
        this.title = "Update";
        this.RoleForm = this.formBuilder.group({
          id:0,
          'roleName':  [null, Validators.compose([Validators.required]),this.duplicateRole.bind(this)], 
         });
        
      }
      ProcessEditAction(id:number){
        this.idOnUpdate=id;

        this.service.getRoleById(id).subscribe(resp => {
          this.updatedata = false;
          this.saveRole  =false;
          this.RoleForm.patchValue({ 
            id: resp.RoleId,
           roleName:resp.RoleName,
          }); 
      }
          , error => this.formErrors = error);
      }
    
      getRolePermissons(id :number){
        this.service.getRolePermissonsById(id)
         this._router.navigate([id],{relativeTo: this.router});
      }     


      exportToPdf() {
      
        if(this.temp.length!=0){
            var doc = new jsPDF();
            var rows = [];
           
              var col = ['sno','RoleName'];
                for(var key in this.temp){
                    var temporary = [(parseInt(key) +1), this.temp[key].RoleName];
                    rows.push(temporary);
                }
                let reportname = "Roles.pdf"
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
                var temporary = [(parseInt(key) +1), this.temp[key].RoleName];
                rows.push(temporary);
            }
        var createXLSLFormatObj = [];
            var xlsHeader =  ['sno','RoleName'];
            createXLSLFormatObj.push(xlsHeader);
         $.each(rows, function(index, value) {
                var innerRowData = [];
               $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.RoleName + "</td></tr>");
      
            
                $.each(value, function(ind, val) {
        
                    innerRowData.push(val);
                });
                createXLSLFormatObj.push(innerRowData);
            });
            var filename = "Roles.xlsx";
            var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
            const workbook: XLSX.WorkBook = { Sheets: { 'Roles': ws }, SheetNames: ['Roles'] };
            XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
        }
        else{
          this.sendReceiveService.showDialog('There is No Data Available to Export');
      }
      }
    
    
    



}
