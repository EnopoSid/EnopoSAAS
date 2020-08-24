import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { CommunicationService } from './communication.service';
import {Router, NavigationEnd, ActivatedRouteSnapshot, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, CommunicationStatusModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';

import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';


@Component({
    selector: 'communication-table',
    templateUrl: './communication.component.html',
    styleUrls: ['./communication.component.css']
})
export class CommunicationComponent implements OnInit {
    displayedColumns = ['sno','CommunicationName','Actions']
    dataSource: MatTableDataSource<CommunicationStatusModel>;
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
    AddCommunicationFlag:boolean=false;
    CommunicationForm:FormGroup;
    userId: number = 0;
    idOnUpdate: number =0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    currentPageLimit: number = 0;
    limit = MyAppHttp.LIMIT;
    pageLimitOptions = MyAppHttp.PAGE_LIMIT_OPTIONS



    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: CommunicationService,
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
                communicationName: {},
            };
        }
 
    
ngOnInit(){
    this.userId=this.sendReceiveService.globalUserId;
    this.filterData={  
        filterColumnNames:[
          {"Key":'sno',"Value":" "},
          {"Key":'CommunicationName',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
    this.getAllCommunications();
    this.CommunicationForm = this.formBuilder.group({
        id: 0,
        'communicationName':  [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]),this.duplicateCommunicationName.bind(this)],
    });
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
}
getAllCommunications(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllCommunicationTypes().subscribe((response) => {
        const getAllCommunicationsData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                getAllCommunicationsData.push(response[i]);
            }
            this.filterData.gridData = getAllCommunicationsData;
            this.dataSource = new MatTableDataSource(getAllCommunicationsData);
            this.filterData.dataSource=this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;            
                document.getElementById('preloader-div').style.display = 'none';;
    }, (error) => {
            document.getElementById('preloader-div').style.display = 'none';;
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

    this.service.getCommunicationTypeById(id)
        .subscribe(resp => {
            this.CommunicationForm.patchValue({
                id: resp.CommunicationId,
                communicationName: resp.CommunicationName,
            });
        },  error => this.formErrors = error);
}

updateCommunication(id){
        this.AddCommunicationFlag = true;
        this.processEditAction(id);
        this.title = "Update";
        this.CommunicationForm = this.formBuilder.group({
            id:0,
            'communicationName':  [null, Validators.compose([Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]),this.duplicateCommunicationName.bind(this)],
           });
         
    }
    duplicateCommunicationName(){
    const q = new Promise((resolve, reject) => {
        this.service.duplicateCommunicationType({
            'ModifiedBy': null,
            'CreatedBy': null,
            'CommunicationName':this.CommunicationForm.controls['communicationName'].value,
            'CommunicationId': !!this.idOnUpdate ? this.idOnUpdate: 0,
            'IsActive':MyAppHttp.ACTIVESTATUS,
      }).subscribe((duplicate) => {
            if (duplicate) {
                resolve({ 'duplicateCommunicationName': true });

            } else {
                resolve(null);
            }
        }, () => { resolve({ 'duplicateCommunicationName': true }); });
    });
    return q;

}

addCommunication(){
    this.AddCommunicationFlag=true;
    this.title="Save"
}
onCommunicationFormSubmit() {
    let var_id: string = this.CommunicationForm.value.id;
    let communicationName: string = this.CommunicationForm.value.communicationName;
    if (!this.CommunicationForm.valid) {
        return;
    }
    if (this.title == "Save") { 
        this.service.saveCommunicationType({
            'CommunicationName': communicationName,
            'IsActive': MyAppHttp.ACTIVESTATUS,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
        }).subscribe((data) => {
            this.getAllCommunications();
            this.CommunicationForm.reset();
        }, error =>  error => {
            this.formErrors = error;
        });
        
    }
    else if (this.title == "Update") {
        this.idOnUpdate=0;
        this.service.updateCommunicationType({
            "CommunicationId":var_id,
            'CommunicationName': communicationName,
            'IsActive': MyAppHttp.ACTIVESTATUS,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
        }, var_id)
            .subscribe((data) => {
               this.title = "Save";
               this.CommunicationForm = this.formBuilder.group({
                id: 0,
                'communicationName':  [null, Validators.compose([Validators.required]),this.duplicateCommunicationName.bind(this)],
            });
               this.getAllCommunications();
               this.CommunicationForm.reset();
            }, error => {
                this.formErrors = error;
            });
            
    }
    this.AddCommunicationFlag=false;
}
deleteCommunication(id) {
    this.appInfoService.confirmationDialog().subscribe(result=>{ 
        if(!!result){ 
            this.service.deleteCommunicationType(id)
        .subscribe((data) => {
            this.getAllCommunications();
        }, error => {
            this.formErrors = error
        });
        
    }   
});
}

onCancel(){
    this.idOnUpdate=0;
    this.CommunicationForm.reset();
    this.AddCommunicationFlag=false;
}
}

