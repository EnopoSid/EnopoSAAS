import { Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort, MatDialogRef, MatDialog } from '@angular/material';
import { IPageLevelPermissions, BannerImageModel } from 'src/app/helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';

import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';
import { environment } from 'src/environments/environment';
import { ComfirmComponent } from '../../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';
import { CategoriesService } from './categories.service';
@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  encapsulation: ViewEncapsulation.None
})
export class CategoriesComponent implements OnInit {
  appURL = environment.url;
  displayedColumns = ['sno', 'CategoryName', 'SiteName', 'Actions']
  dataSource: MatTableDataSource<BannerImageModel>;
  gridData = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  dialogRef: MatDialogRef<any>;
  filterData;
  SiteForm:FormGroup;
  userId: number = 0;
  updateText;
  pageName: string = "LandingPage";
  title: string;
  checked = true;
  siteList=[
    {'id':1,'siteName':'Online'},
    {'id':2,'siteName':'POS'}
  ];
  IsPos:boolean=false;
  IsOnline:boolean=false;
  brandImageFormErrors = {

    ImagePath: {}

  };
  pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
  constructor(private spinner: NgxSpinnerService,
    private router: Router,
    private route: ActivatedRoute,
    public ref: ChangeDetectorRef,
    public dialog: MatDialog,
    public viewContainerRef: ViewContainerRef,
    public logoutService: LogoutService,
    public appInfoService: AppInfoService,
    public sendReceiveService: SendReceiveService,
    public translate: TranslateService,
    private actRoute: ActivatedRoute,
    private formBuilder: FormBuilder,
    private activatedRoute: ActivatedRoute,
    public service: CategoriesService,
  ) {
    this.pageName = "LandingPage";
  }

  ngOnInit() {
    this.userId = this.sendReceiveService.globalUserId;
    this.filterData = {
      filterColumnNames: [
        { "Key": 'sno', "Value": " " },
        { "Key": 'CategoryName', "Value": " " },
        { "Key": 'SiteName', "Value": " " }
      ],
      gridData: this.gridData,
      dataSource: this.dataSource,
      paginator: this.paginator,
      sort: this.sort
    };
    
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
      this.pagePermissions = pageLevelPermissions.response;
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    this.SiteForm = this.formBuilder.group({
      'id': 0,
      'SiteName': [null, Validators.required],
  });

  }

  onSiteFormSubmit(){
   var selectedSiteId=this.SiteForm.value.SiteName;
   if(selectedSiteId==1){
    this.IsPos=false;
    this.IsOnline=true;
   }else if(selectedSiteId==2){
    this.IsOnline=false;
    this.IsPos=true;
   }
   this.getAllCategories();
  }

  getAllCategories() {
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllCategories().subscribe((response) => {
      const configurationData: any = [];
      for (let i = 0; i < response.length; i++) {
        response[i].sno = i + 1;
        if(!!this.IsPos){
          response[i].SiteName="POS";
          response[i].IsActive = response[i].IsPOS;
        }else if(!!this.IsOnline){
          response[i].SiteName="Online";
          response[i].IsActive = response[i].IsOnline;
        }
        
        configurationData.push(response[i]);
      }      
      this.filterData.gridData = configurationData;
      this.dataSource = new MatTableDataSource(configurationData);
      this.filterData.dataSource = this.dataSource;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
       document.getElementById('preloader-div').style.display = 'none';;

    }, (error) => {
       document.getElementById('preloader-div').style.display = 'none';;
    });
  }

  updatePagination(){
    this.filterData.dataSource=this.filterData.dataSource;
    this.filterData.dataSource.paginator = this.paginator;
    }

  activateRecord(event, id) {
      const dialogRef = this.dialog.open(ComfirmComponent, {
        width: '300px',
        height: "180px",
        data: "Do you want to change status?"

      });
      dialogRef.afterClosed().subscribe(result => {
        if (!!result) {
          var obj={
            "CategoryId":id,
            "IsFromOnline":this.IsOnline,
            "IsFromPOS":this.IsPos,
            "StatusFlag":event
          }
          this.service.activateRecord(obj).subscribe((data) => {
           this.getAllCategories();
          }, error => {

          });
        }
      });
  }
}
