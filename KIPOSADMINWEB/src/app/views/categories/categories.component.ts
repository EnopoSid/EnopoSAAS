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
import { CategoriesService } from './categories.service';
import { ComfirmComponent } from '../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css']

})
export class CategoriesComponent implements OnInit {
  appURL = environment.url;
  displayedColumns = ['sno', 'categoryName', 'subCategoryName','IsOnline', 'IsPOS','IsBoth']
  dataSource: MatTableDataSource<BannerImageModel>;
  gridData = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  dialogRef: MatDialogRef<any>;
  filterData;
 CategoryData: any = [];

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
  SelectedPos:number;
  SelectedOnline:number;
  SelectedBoth:number;
  SelectedEvent:string;
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
        { "Key": 'categoryName', "Value": " " },
        { "Key": 'subCategoryName', "Value": " " },
        { "Key": 'IsOnline', "Value": " " },
        { "Key": 'IsPOS', "Value": " " },
        { "Key": 'IsBoth', "Value": " " }
      ],
      gridData: this.gridData,
      dataSource: this.dataSource,
      paginator: this.paginator,
      sort: this.sort
    };
    
    this.getAllCategories();
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

  

  getAllCategories() {
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllCategories().subscribe((response) => {
      const configurationData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
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

  activateRecord(event,select, catergory, subcategory) {
    document.getElementById('preloader-div').style.display = 'none';;
    if(event==true && select=='IsOnline'){
      var message ="Do you want to Enable for Online?"
  }
  else if(event==true && select=='IsPOS'){
      
     var message ="Do you want to Enable for Pos?"

  }
  else if(event==true && select=='IsBoth'){
      
    var message ="Do you want to Enable POS & Online?"

 }
 if(event==false && select=='IsOnline'){
  var message ="Do you want to Disable for Online?"
}
else if(event==false && select=='IsPOS'){
  
 var message ="Do you want to Disable for Pos?"

}
else if(event==false && select=='IsBoth'){
  
var message ="Do you want to Disable POS & Online?"

}
  
      const dialogRef = this.dialog.open(ComfirmComponent, {
        width: '300px',
        height: "180px",
        data: message

      });
      dialogRef.afterClosed().subscribe(result => {
        if (!!result) {
          document.getElementById('preloader-div').style.display = 'none';;
          if(select=='IsOnline'){
            if(event===true ){             
              this.SelectedOnline=1;
              this.SelectedBoth=0;
              this.SelectedPos=0; 
              this.SelectedEvent=select;


          }
         else{
           this.SelectedOnline=0;
           this.SelectedBoth=0;
           this.SelectedPos=0; 
           this.SelectedEvent=select;
         } 
          }
          else if(select=='IsBoth'){
            if(event===true ){             
              this.SelectedBoth=1;
              this.SelectedEvent=select;

              
          }
          else {
            this.SelectedBoth=0;
            this.SelectedEvent=select;

          }
          }


         else if(select=='IsPOS'){
           if(event===true){            
           this.SelectedPos=1;
           this.SelectedOnline=0;
           this.SelectedBoth=0;
           this.SelectedEvent=select;

         }
        else{
          this.SelectedPos=0;     
          this.SelectedBoth=0;
          this.SelectedOnline=0;
          this.SelectedEvent=select;

        } 
         }
        
        
        
          var obj={
           
            "categoryId":catergory,
        "ParentCategoryId":subcategory,
                   "IsPOS":this.SelectedPos,
              "IsOnline":this.SelectedOnline,
              "IsBoth":this.SelectedBoth,
              "selectedEvent":this.SelectedEvent,
              "CreatedBy":this.userId,
              "ModifiedBy":this.userId
          }
          this.service.activateRecord(obj).subscribe((data) => {
           this.getAllCategories();
          }, error => {
            document.getElementById('preloader-div').style.display = 'none';;

          });
        }else{
          this.getAllCategories(); 
        }
      });
  }
    
}
