import { Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort, MatDialogRef, MatDialog } from '@angular/material';
import { OurStoryModel, IPageLevelPermissions, FeatureArticleModel } from 'src/app/helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { FeaturedArticlesComponentService } from './featuredArticles.service';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { ComfirmComponent } from '../../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-featuredArticles',
  templateUrl: './featuredArticles.component.html',
  encapsulation: ViewEncapsulation.None
})
export class FeaturedArticlesComponent implements OnInit {
  appURL = environment.url;
  displayedColumns = ['sno', 'ArticleTitleName', 'ArticlePostedBy','ArticleDescription','image', 'IsActive' ,'Actions']
  dataSource: MatTableDataSource<FeatureArticleModel>;
  gridData = [];
  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: false}) sort: MatSort;
  dialogRef: MatDialogRef<any>;
  filterData;
  pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
  constructor(private spinner: NgxSpinnerService,
    private router: Router,
    private route:ActivatedRoute,
    public service: FeaturedArticlesComponentService,
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
        {"Key":'ArticleTitleName',"Value":" "},
        {"Key":'ArticlePostedBy',"Value":" "},
        {"Key":'ArticleDescription',"Value":" "},
        {"Key":'image',"Value":" "},
        { "Key": 'IsActive', "Value": " " },
      ],
      gridData:  this.gridData,
      dataSource: this.dataSource,
      paginator:  this.paginator,
      sort:  this.sort
    };
  this.getAllFeaturedArticles();
  this.sendReceiveService.globalPageLevelPermission = new Subject;
  this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
      this.pagePermissions =pageLevelPermissions.response;  
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
  });
  }
  getAllFeaturedArticles(){
      document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllFeaturedArticles().subscribe((response) => {
       document.getElementById('preloader-div').style.display = 'none';           
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
  editStory(id: number){    
    this.service.getArticleListById(id)
    this.router.navigate(['landingpage/featuredArticles/addFeaturedArticles/'+id]);
    this.service.UserView(false);
}
deleteStory(id) {
  const dialogRef = this.dialog.open(ComfirmComponent, {
    width: '300px',
    height:"180px",
     data:"Do you want to Delete?"
    
  });
  dialogRef.afterClosed().subscribe(result => {
      if(!!result){ 
          this.service.deleteArticle(id).subscribe((data) => {
              this.getAllFeaturedArticles();
          }, error => {

          });
  }   
}); 
}
activateRecord(event,id){
  const dialogRef = this.dialog.open(ComfirmComponent, {
    width: '300px',
    height:"180px",
     data:"Do you want to change Status ?"
    
  });
  dialogRef.afterClosed().subscribe(result => {
    if(!!result){ 
        this.service.activateRecord(id, event).subscribe((data) => {
            this.getAllFeaturedArticles();
        }, error => {
  
        });
  }   
  this.getAllFeaturedArticles();
  }); 
}
}
