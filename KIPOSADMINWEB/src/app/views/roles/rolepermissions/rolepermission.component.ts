import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {LogoutService} from '../../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../../services/common/appInfo.service';
import {SendReceiveService} from '../../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../../services/common/myAppHttp.service';
import { IPageLevelPermissions, RolePermissionModel } from '../../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { RoleService } from 'src/app/views/roles/role.service';
import { TranslateService } from '@ngx-translate/core';
import * as $ from "jquery";
import { Subject } from 'rxjs';
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { DatePipe } from '@angular/common';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
@Component({
    selector: 'app-rolepermissions',
    templateUrl: './rolepermission.component.html'
})
export class RolePermissionComponent implements OnInit {
    displayedColumns = ['MenuName','SubMenuName','PermissionName','Actions']
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
    RoleForm:FormGroup;
    roleList = [];
    defaultValue = 'Completed';
    constructor(private router: Router,
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
        private formBuilder:FormBuilder,
        private route:ActivatedRoute,
        private _router: Router,) { 
        }
  
    ngOnInit() {
      this.userId=this.sendReceiveService.globalUserId;
      this.filterData={
        filterColumnNames:[
          {"Key":'MenuName',"Value":" "},
          {"Key":'SubMenuName',"Value":" "},
          {"Key":'PermissionName',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
        let paramId = +this.route.snapshot.params['id'];
        this.RolePermissionForm = this.formBuilder.group({
          'permission':['',Validators.required]
        }); 
        this.RoleForm = this.formBuilder.group({
          'id': 0,
          'role':  [null, Validators.required],
        });
        if(!paramId){
          this.service.getAllRoles().subscribe((response) => {
            const roleData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                roleData.push(response[i]);
            }
   
            this.roleList = roleData;
            this.id= roleData[0].RoleId;
            this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
              this.rolename = resp;
              this.editRolePermission(this.id);

          
            }) 
    },(error) => {
        });
        }else{
          this.service.getAllRoles().subscribe((response) => {
            const roleData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                roleData.push(response[i]);
            }
            this.roleList = roleData;
          this.id = paramId;
          this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
            this.rolename = resp;
            this.editRolePermission(this.id);

          })
          }) 
        }
       
             
        this.permissiontypeList();
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
            this.pagePermissions =pageLevelPermissions.response;  
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
   
      
    }
    editRolePermission(id) {
      this.rows = [];
      this.rolepermissions = [];
       this.permissionType = false
   
      this.service.getRolePermissonsById(this.id).subscribe((data ) =>{
        this.rolepermissions = data;
        for (let i = 0; i <this.rolepermissions.length; i++) {
          this.rolepermissions[i].sno = i + 1;
          this.rolepermissions[i].editMode = false;
          this.rows.push(this.rolepermissions[i]);
      }
      this.temp = this.rows;
      this.filterData.gridData = this.rows;
      this.dataSource = new MatTableDataSource(this.rows);
      this.filterData.dataSource=this.dataSource;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.rolepermissions = this.rows;
      })
    }

  permissiontypeList(){
  
  this.service.getPermissions().subscribe(resp => {
   this. permisssionList  = resp;
  
  })
  }
  editPermission(row){    
  this.rows.forEach(element => {
    if(element.RolePermissionId == row.RolePermissionId)
    element.editMode = true;
    else
    element.editMode = false;
  });

  var tempPermissionValue =  $.grep(this.permisssionList,function(e){return e.PermissionId == row.PermissionId})
  this.RolePermissionForm.controls['permission'].setValue(tempPermissionValue.length  > 0 ? tempPermissionValue[0] : {});
    }
  cancelRolePermission(row){
    row.editMode = false;
   this.permissionType = false;
  }
  onChange(event): void{
    this.permissionname = event.value;
  }
  
  updatePagination(){
    this.filterData.dataSource=this.filterData.dataSource;
    this.filterData.dataSource.paginator = this.paginator;
    }
    
  saveRolePermission(row){
    if (!this.RolePermissionForm.valid) {
      return false;
     }

    if(row.RolePermissionId){
      let requestData ={
        'RolePermissionId': row.RolePermissionId,
        'PermissionId': !!this.RolePermissionForm.value.permission ?  
        this.RolePermissionForm.value.permission.PermissionId : 0
      };
      this.service.updateRolePermission(row.RolePermissionId, requestData).subscribe(resp=> {
        this.editRolePermission(this.id);
      }, error=>{

      });
    }
  }
  onGoBack(){
    this._router.navigate(['../'],{relativeTo: this.route}); 
  }
  onRoleSubmit(){
    let var_roles :number = this.RoleForm.value.role;
    if (!this.RoleForm.valid) {
        return false;
    }
  
    this.sendReceiveService.getKeyValueName(var_roles).subscribe(resp => {
      this.rolename = resp;
    this.id =  var_roles;
    this.editRolePermission( this.id);
    })
}

exportToPdf() {

  if(this.temp.length!=0){
      var doc = new jsPDF();
      var rows = [];
     
        var col = ['sno','MenuName','SubMenuName','Permission'];
          for(var key in this.temp){
              var temporary = [(parseInt(key) +1), this.temp[key].MenuName,this.temp[key].SubMenuName,this.temp[key].PermissionName];
              rows.push(temporary);
          }
          let reportname = "RolePermission.pdf"
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
              var temporary = [(parseInt(key) +1), this.temp[key].MenuName,this.temp[key].SubMenuName,this.temp[key].PermissionName];
              rows.push(temporary);
          }
      var createXLSLFormatObj = [];
      var xlsHeader =  ['sno','MenuName','SubMenuName','Permission'];
      createXLSLFormatObj.push(xlsHeader);
   $.each(rows, function(index, value) {
          var innerRowData = [];
         $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MenuName +  "</td><td>" + value.SubMenuName +  "</td><td>" + value.PermissionName +   "</td></tr>");

      
          $.each(value, function(ind, val) {
  
              innerRowData.push(val);
          });
          createXLSLFormatObj.push(innerRowData);
      });
      var filename = "RolePermission.xlsx";
      var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
      const workbook: XLSX.WorkBook = { Sheets: { 'RolePermission': ws }, SheetNames: ['RolePermission'] };
      XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
  }
  else{
    this.sendReceiveService.showDialog('There is No Data Available to Export');
}
}



}