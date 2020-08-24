import { Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort, MatDialogRef, MatDialog } from '@angular/material';
import { OurStoryModel, IPageLevelPermissions } from 'src/app/helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { ComfirmComponent } from '../../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';
import { OurBrandComponentService } from './ourBrands.service';
import { environment } from 'src/environments/environment';
@Component({
  selector: 'app-ourBrands',
  templateUrl: './ourBrands.component.html',
  encapsulation: ViewEncapsulation.None
})
export class OurBrandsComponent implements OnInit {
  appURL = environment.url;
  displayedColumns = ['sno', 'Id', 'TitleName','SubTitleName','image', 'ShortDescription','IsActive' ,'Actions']
  dataSource: MatTableDataSource<OurStoryModel>;
  gridData = [];
  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: false}) sort: MatSort;
  dialogRef: MatDialogRef<any>;
  filterData;
  pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
  constructor(private spinner: NgxSpinnerService,
    private router: Router,
    private route:ActivatedRoute,
    public service: OurBrandComponentService,
    public ref: ChangeDetectorRef,
    public dialog: MatDialog,
    public viewContainerRef: ViewContainerRef,
    public logoutService: LogoutService,
    public appInfoService: AppInfoService,
    public sendReceiveService: SendReceiveService,
    public translate: TranslateService,  
    private actRoute: ActivatedRoute,
    private activatedRoute: ActivatedRoute,
 ) {
}

  ngOnInit() {
    this.filterData={
      filterColumnNames:[
        {"Key":'sno',"Value":" "},
        {"Key":'Id',"Value":" "},
        {"Key":'TitleName',"Value":" "},
        {"Key":'SubTitleName',"Value":" "},
        {"Key":'image',"Value":" "},
        {"Key":'ShortDescription',"Value":" "},
        { "Key": 'IsActive', "Value": " " },
      ],
      gridData:  this.gridData,
      dataSource: this.dataSource,
      paginator:  this.paginator,
      sort:  this.sort
    };
  this.getAllBrands();
  this.sendReceiveService.globalPageLevelPermission = new Subject;
  this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
      this.pagePermissions =pageLevelPermissions.response;  
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
  });
  }
  getAllBrands(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllBrands().subscribe((response) => {
      const userData: any = [];
      for (let i = 0; i < response.length; i++) {
          response[i].sno = i + 1;
          response[i].IsActive = response[i].IsActive.toString();
          userData.push(response[i]);
      }
      this.filterData.gridData = userData;
      this.dataSource = new MatTableDataSource(userData);
      this.filterData.dataSource=this.dataSource;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
   document.getElementById('preloader-div').style.display = 'none';

  }, (error) => {

      document.getElementById('preloader-div').style.display = 'none';

  });
  }
  editBrand(id: number){
    this.service.getBrandListById(id)
    this.router.navigate(['landingpage/ourBrands/addourBrands/'+id]);
    this.service.UserView(false);
}

updatePagination(){
  this.filterData.dataSource=this.filterData.dataSource;
  this.filterData.dataSource.paginator = this.paginator;
  } 
  
deleteBrand(id) {
  const dialogRef = this.dialog.open(ComfirmComponent, {
    width: '300px',
    height:"180px",
     data:"Do you want to Delete?"
    
  });
  dialogRef.afterClosed().subscribe(result => {
      if(!!result){ 
          this.service.deleteBrand(id).subscribe((data) => {
              this.getAllBrands();
          }, error => {

          });
  }   
}); 
}
activateRecord(event,id){
if(event ==true){
  const dialogRef = this.dialog.open(ComfirmComponent, {
    width: '300px',
    height:"180px",
     data:"Do you want to change Status ?"
    
  });
  dialogRef.afterClosed().subscribe(result => {
    if(!!result){ 
        this.service.activateRecord(id,event).subscribe((data) => {
            this.getAllBrands();
        }, error => {
  
        });
  }   
  this.getAllBrands();
  }); 
}
else{
  const dialogRef = this.dialog.open(ComfirmComponent, {
    width: '300px',
    height:"180px",
     data:"Do you want to change Status ?"
    
  });
  dialogRef.afterClosed().subscribe(result => {
    if(!!result){ 
        this.service.activateRecord(id,event).subscribe((data) => {
            this.getAllBrands();
        }, error => {
  
        });
  }   
  this.getAllBrands();
  }); 
 }
}
}
