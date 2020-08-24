import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { DepartmentService } from './department.service';
import {Router, NavigationEnd, ActivatedRouteSnapshot, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, DepartmentModel } from '../../helpers/common.interface';
import { TranslateService } from '@ngx-translate/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';

@Component({
    selector: 'department-table',
    templateUrl: './department.component.html',
})
export class DepartmentComponent implements OnInit {
    displayedColumns = ['sno','DepartmentName','Actions']
    dataSource: MatTableDataSource<DepartmentModel>;
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
    AddDepartmentFlag:boolean=false;
    DepartmentForm:FormGroup;
    userId: number = 0;
    idOnUpdate: number =0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: DepartmentService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        private formBuilder:FormBuilder
        ) {
            this.formErrors = {
                departmentName: {},
            };
        }

    
ngOnInit(){
    this.userId=this.sendReceiveService.globalUserId;
    this.filterData={  
        filterColumnNames:[
          {"Key":'sno',"Value":" "},
          {"Key":'DepartmentName',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
    this.getAllDepartments();
    this.DepartmentForm = this.formBuilder.group({
        id: 0,
        'departmentName':  [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]),this.duplicateDepartmentName.bind(this)],
    });
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });

}
getAllDepartments(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllDepartments().subscribe((response) => {
        const departmentData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                departmentData.push(response[i]);
            }
            this.filterData.gridData = departmentData;
            this.dataSource = new MatTableDataSource(departmentData);
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

    this.service.getDepartmentById(id)
        .subscribe(resp => {
            this.DepartmentForm.patchValue({
                id: resp.DepartmentId,
                departmentName: resp.DepartmentName,
            });
        },  error => this.formErrors = error);
}

updateDepartment(id){
        this.AddDepartmentFlag = true;
        
        this.processEditAction(id);
        this.title = "Update";
        this.DepartmentForm = this.formBuilder.group({
            id:0,
            'departmentName':  [null, Validators.compose([Validators.required]),this.duplicateDepartmentName.bind(this)],
           });
         
    }
    duplicateDepartmentName(){
    const q = new Promise((resolve, reject) => {
        this.service.duplicateDepartment({
            'ModifiedBy': null,
            'CreatedBy': null,
            'DepartmentName':this.DepartmentForm.controls['departmentName'].value,
            'DepartmentId': !!this.idOnUpdate ? this.idOnUpdate: 0,
            'IsActive':MyAppHttp.ACTIVESTATUS,
      }).subscribe((duplicate) => {
            if (duplicate) {
                resolve({ 'duplicateDepartmentName': true });

            } else {
                resolve(null);
            }
        }, () => { resolve({ 'duplicateDepartmentName': true }); });
    });
    return q;

}

addDepartment(){
    this.AddDepartmentFlag=true;
    this.title="Save"
}
onDepartmentFormSubmit() {
    let var_id: string = this.DepartmentForm.value.id;
    let departmentName: string = this.DepartmentForm.value.departmentName;
    if (!this.DepartmentForm.valid) {
        return;
    }
    if (this.title == "Save") { 
        
        this.service.saveDepartment({
            'DepartmentName': departmentName,
            'IsActive': 1,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
        }).subscribe((data) => {
            this.getAllDepartments();
            this.DepartmentForm.reset();
        }, error =>  error => {
            this.formErrors = error;
        });
        
    }
    else if (this.title == "Update") {
        this.idOnUpdate=0;
        
        this.service.updateDepartment({
            "DepartmentId":var_id,
            'DepartmentName': departmentName,
            'IsActive': 1,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
        }, var_id)
            .subscribe((data) => {
               this.title = "Save";
               this.DepartmentForm = this.formBuilder.group({
                id: 0,
                'departmentName':  [null, Validators.compose([Validators.required]),this.duplicateDepartmentName.bind(this)],
            });
               this.getAllDepartments();
               this.DepartmentForm.reset();
            }, error => {
                this.formErrors = error;
            });
            
    }
    this.AddDepartmentFlag=false;
}
deleteDepartment(id) {
    this.appInfoService.confirmationDialog().subscribe(result=>{ 
        if(!!result){ 
            this.service.deleteDepartment(id)
        .subscribe((data) => {
            this.getAllDepartments();
        }, error => {
            this.formErrors = error;
        });
        
    }   
});
}

onCancel(){
    this.idOnUpdate=0;
    this.DepartmentForm.reset();
    this.AddDepartmentFlag=false;
}
}

