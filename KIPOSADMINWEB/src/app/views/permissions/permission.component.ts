import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { PermissionService } from './permission.service';
import {Router} from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, RolePermissionModel } from '../../helpers/common.interface';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';



@Component({
    selector: 'permission-table',
    templateUrl: './permission.component.html',
    styleUrls: ['./permission.component.css']
})
export class PermissionComponent implements OnInit {
    status: boolean;
    displayedColumns = ['sno','PermissionName','Actions']
    dataSource: MatTableDataSource<RolePermissionModel>;
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
    PermissionForm:FormGroup;
    userId: number = 0;
    idOnUpdate: number =0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    currentPageLimit: number = 0;
    limit = MyAppHttp.LIMIT;
    pageLimitOptions = MyAppHttp.PAGE_LIMIT_OPTIONS

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: PermissionService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        private formBuilder:FormBuilder) {
            this.formErrors = {
                PermissionName: {}
               
            };
}

    
ngOnInit(){
    this.userId=this.sendReceiveService.globalUserId;
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'PermissionName',"Value":" "},
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
    this.getAllPermissions();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    this.PermissionForm = this.formBuilder.group({
        id: 0,
        'PermissionName':  [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]),this.duplicatePermissionName.bind(this)] 
    });
    
}
getAllPermissions(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllPermissions().subscribe((response) => { 
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
    this.idOnUpdate= id;

    this.service.getPermissionById(id)
        .subscribe(resp => {
            this.PermissionForm.patchValue({
                id: resp.PermissionId,
                PermissionName: resp.PermissionName
               
            });
        },  error => this.formErrors = error);
}

updatePermission(id){
        this.addPermissionFlag = true;
        
        this.processEditAction(id);
        this.title = "Update";
        this.PermissionForm = this.formBuilder.group({
            id:0,
            'PermissionName':[null, Validators.compose([Validators.required, Validators.minLength(3)]),this.duplicatePermissionName.bind(this)],
           
           });
         
    }
    duplicatePermissionName(){
        
    const q = new Promise((resolve, reject) => {
        this.service.duplicatePermission({
            'PermissionName':this.PermissionForm.controls['PermissionName'].value,
            'PermissionId': !!this.idOnUpdate ? this.idOnUpdate:0,
            'IsActive': 1,
            'CreatedBy': null,
            'CreatedDate': null,

      }).subscribe((duplicate) => {
            if (duplicate) {
                resolve({ 'duplicatePermissionName': true });

            } else {
                resolve(null);
            }
        }, () => { resolve({ 'duplicatePermissionName': true }); });
    });
    return q;

}

addPermission(){
    this.addPermissionFlag=true;
    this.title="Save"
}
onPermissionFormSubmit() {
    let var_id: string = this.PermissionForm.value.id;
    let var_PermissionName: string = this.PermissionForm.value.PermissionName;
  
    if (!this.PermissionForm.valid) {
        return;
    }
    if (this.title == "Save") { 
        
        this.service.savePermission({
            'PermissionName': var_PermissionName,
            'IsActive': 1,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
        }).subscribe((data) => {
            this.getAllPermissions();
            this.PermissionForm = this.formBuilder.group({
                  id: 0,
                'PermissionName': [null,(Validators.required),this.duplicatePermissionName.bind(this)]
             }) 
                    this.PermissionForm.reset();
                    this.status = true;
        }, error =>  error => {
            this.formErrors = error;
        });
        
    }
    else if (this.title == "Update") {
        this.idOnUpdate=0;
        
        this.service.updatePermission({
            "PermissionId":var_id,
            'PermissionName': var_PermissionName,
            'IsActive': 1,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
        }, var_id)
            .subscribe((data) => {
               this.duplicatePermissionName();
               this.title = "Save";
               this.PermissionForm = this.formBuilder.group({
                id: 0,
                'PermissionName':  [null, Validators.compose([Validators.required])],
              
            });
            this.getAllPermissions();
               this.PermissionForm.reset();
            }, error => {
                this.formErrors = error;
            });
            
    }
    this.addPermissionFlag=false;
}
deletePermission(id) {
    this.appInfoService.confirmationDialog().subscribe(result=>{ 
        if(!!result){ 
    this.service.deletePermission(id)
        .subscribe((data) => {
            this.getAllPermissions();
        }, error => {
            this.formErrors = error
        });
}
});
}
onCancel(){
    this.idOnUpdate=0;
    this.PermissionForm.reset();
    this.addPermissionFlag=false;
}
}
