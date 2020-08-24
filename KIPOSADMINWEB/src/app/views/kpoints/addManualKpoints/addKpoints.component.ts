import { Component, OnInit, ViewEncapsulation,ViewChild, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { IPageLevelPermissions, RolePermissionModel } from 'src/app/helpers/common.interface';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { Subject } from 'rxjs';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';

import { environment } from 'src/environments/environment';

import { DatePipe } from '@angular/common';
import { KpointsService } from '../kpoints.service';


@Component({
  selector: 'app-addKpoints',
  templateUrl: './addKpoints.component.html',
  encapsulation: ViewEncapsulation.None
})

export class addManualKpoints implements OnInit {
  displayedColumns = ['sno','MemberId','ManualRewardPoints','CreatedDate','Actions']

  dataSource: MatTableDataSource<RolePermissionModel>;
  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: false}) sort: MatSort;
  submitted: boolean = false;
  appURL = environment.url;
  userId: number = 0;
  posPlanFormGroup: FormGroup;
  id: number = 0;
  filterData;
  gridData =[];
  formErrors: any;
  idOnUpdate: number = 0;
  title: string ;
  saveUser: boolean = false;
  AddKpointsFlag:boolean=false;
  EditRole: boolean=false;
  updateUser: boolean = false;
  checkInput: boolean = false;
  minDate;
  maxDate;
  endDate;
  rolename: string;
  RolePermissionForm: FormGroup;

  updateRespose: any;
  addStoryFormErrors = {
    MemberId: {},
    ManualRewardPoints: {},
    targetSalesAmt: {},

  };
  pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
  isOnView: boolean = false;


  constructor(private spinner: NgxSpinnerService,
    private router: Router,

    public ref: ChangeDetectorRef,
    public dialog: MatDialog,
    public viewContainerRef: ViewContainerRef,
    public logoutService: LogoutService,
    public appInfoService: AppInfoService,
    public service: KpointsService,
    public sendReceiveService: SendReceiveService, private formBuilder: FormBuilder,
    public translate: TranslateService,
    fb: FormBuilder, public datepipe: DatePipe,

    private route: ActivatedRoute
  ) {
  }

  ngOnInit() {
    this.maxDate = new Date();
    this.userId = this.sendReceiveService.globalUserId;
    this.filterData={
      filterColumnNames:[
          {"Key":'sno',"Value":" "},
          {"Key":'MemberId',"Value":" "},
          {"Key":'ManualRewardPoints',"Value":" "},
          {"Key":'CreatedDate',"Value":" "}


      ],
      gridData:  this.gridData,
      dataSource: this.dataSource,
      paginator:  this.paginator,
      sort:  this.sort
    };

    this.RolePermissionForm = this.formBuilder.group({
      'RefundStatusId':  [null]
  });
      this.id = 2 ;
      
      if(!isNaN(this.id))
      {
      this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
        this.rolename = resp;
      
      })
    }
    this.posPlanFormGroup = this.formBuilder.group({
      
      'MemberId':  [null, Validators.compose([Validators.required, Validators.minLength(3),Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.alphaNumericsWithoutSpaces)])],
       'ManualRewardPoints': ['',Validators.compose([ Validators.required,Validators.pattern('^[0-9](\.[0-9]+)?$')])]
  });
    this.getAllMembers();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
    this.pagePermissions =pageLevelPermissions.response;  
   this.sendReceiveService.globalPageLevelPermission.unsubscribe();
});

    if (isNaN(this.id)) {
      this.title = "Add Manual kpoints"
    }

   
 
}
  getAllMembers() {
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAddedKpoints().subscribe((response) => {
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
console.log(this.filterData);
          document.getElementById('preloader-div').style.display = 'none';
    }, (error) => {
          document.getElementById('preloader-div').style.display = 'none';
    });
}

    

     
   onCancel(){
    this.idOnUpdate=0;
    this.posPlanFormGroup.reset();
    this.AddKpointsFlag=false;
}
totalKpoints(){
  this.idOnUpdate = 0;
     this.router.navigate(['/kpoints']);
}
addKpoints(){
  this.AddKpointsFlag=true;
     this.title="Save";
    
 }
  
 onKpointsSubmit() {
  
  if (!this.posPlanFormGroup.valid) {
      return;
  }
  let MemberId: string = this.posPlanFormGroup.value.MemberId;
  let ManualRewardPoints: string = this.posPlanFormGroup.value.ManualRewardPoints;
  if (this.title == "Save") { 
      this.service.saveKpoints({
          'MemberId': MemberId,
          'ManualRewardPoints': ManualRewardPoints,
          'IsActive': 1,
          'CreatedBy': this.userId,
          'ModifiedBy': this.userId,
          'CreatedDate':Date.now,
      }).subscribe((data) => {
          this.getAllMembers();
          this.posPlanFormGroup.reset();
      }, error =>  error => {
          this.formErrors = error;
      });
      
  }
  this.AddKpointsFlag=false;
}





deleteKpoints(MemberId){
  this.appInfoService.confirmationDialog().subscribe(result=>{
    if(!!result){
        this.service.DeleteKpoints(MemberId)
        .subscribe((data) => {
            this.getAllMembers();
        }, error => {
            this.formErrors = error
        });
    
    }
});
}
}
















