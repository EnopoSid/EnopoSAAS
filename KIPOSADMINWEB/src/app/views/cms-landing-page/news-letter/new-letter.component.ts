import { Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort, MatDialogRef, MatDialog, Sort } from '@angular/material';
import { IPageLevelPermissions, FeatureArticleModel, NewsLetterModel } from 'src/app/helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';

import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { ComfirmComponent } from '../../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';
import { environment } from 'src/environments/environment';
import { NewsLetterComponentService } from './news-letter.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-news-letter',
  templateUrl: './news-letter.component.html',
  encapsulation: ViewEncapsulation.None
})
export class NewsLetterComponent implements OnInit {
  appURL = environment.url;
  displayedColumns = ['sno', 'Title', 'NewsLetter', 'CreatedDate', 'LastsendDate', 'Actions']
  dataSource: MatTableDataSource<NewsLetterModel>;
  gridData = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  dialogRef: MatDialogRef<any>;
  filterData;
  pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    public service: NewsLetterComponentService,
    public ref: ChangeDetectorRef,
    public dialog: MatDialog,
    public viewContainerRef: ViewContainerRef,
    public logoutService: LogoutService,
    public appInfoService: AppInfoService,
    public sendReceiveService: SendReceiveService,
    public translate: TranslateService,
    private datePipe: DatePipe,

  ) {
  }

  ngOnInit() {
    this.filterData = {
      filterColumnNames: [
        { "Key": 'sno', "Value": " " },
        { "Key": 'Letter', "Value": " " },
        { "Key": 'NewsLetter', "Value": " " },
        { "Key": 'CreatedDate', "Value": " " },
        { "Key": 'LastsendDate', "Value": " " },
      ],
      gridData: this.gridData,
      dataSource: this.dataSource,
      paginator: this.paginator,
      sort: this.sort
    };
    this.getAllNewsLetter();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
      this.pagePermissions = pageLevelPermissions.response;
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
  }
  getAllNewsLetter() {
      document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllNewsLetter().subscribe((response) => {
      console.log(JSON.stringify(response))
       document.getElementById('preloader-div').style.display = 'none';;
      const userData: any = [];
      for (let i = 0; i < response.length; i++) {
        response[i].sno = i + 1;
        response[i].CreatedDate = this.datePipe.transform(new Date(response[i].CreatedDate), "dd-MM-yyyy");
        response[i].LastsendDate = this.datePipe.transform(new Date(response[i].LastsendDate), "dd-MM-yyyy");
        userData.push(response[i]);
      }
      this.filterData.gridData = userData;
      this.dataSource = new MatTableDataSource(userData);
      this.filterData.dataSource = this.dataSource;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      const sortState: Sort = { active: 'CreatedDate', direction: 'asc' };
      this.sort.active = sortState.active;
      this.sort.direction = sortState.direction;
      this.sort.sortChange.emit(sortState);
       document.getElementById('preloader-div').style.display = 'none';;

    }, (error) => {
       document.getElementById('preloader-div').style.display = 'none';;
    });
  }
  editStory(id: number) {
    this.service.getNewsLetterListById(id)
    this.router.navigate(['landingpage/newsLetter/addNewsLetter/' + id]);
    this.service.UserView(false);
  }
  deleteStory(id) {
    const dialogRef = this.dialog.open(ComfirmComponent, {
      width: '300px',
      height: "180px",
      data: "Do you want to Delete?"

    });
    dialogRef.afterClosed().subscribe(result => {
      if (!!result) {
        this.service.deleteArticle(id).subscribe((data) => {
          this.getAllNewsLetter();
        }, error => {

        });
      }
    });
  }
  sendMessage(id) {
      document.getElementById('preloader-div').style.display = 'block';
    this.service.sendMessage(id).subscribe((response) => {
       document.getElementById('preloader-div').style.display = 'none';;
      if (response) {
        this.sendReceiveService.showDialog('Message Sent Successfully.');
      } else {
        this.sendReceiveService.showDialog('An error occured');
      }
      console.log('response: ' + JSON.stringify(response))
    }, error => {
       document.getElementById('preloader-div').style.display = 'none';;
      this.sendReceiveService.showDialog('An error occured');
    })

  }

  activateRecord(event, id) {
    const dialogRef = this.dialog.open(ComfirmComponent, {
      width: '300px',
      height: "180px",
      data: "Do you want to change Status ?"

    });
    dialogRef.afterClosed().subscribe(result => {
      if (!!result) {
        this.service.activateRecord(id, event).subscribe((data) => {
          this.getAllNewsLetter();
        }, error => {

        });
      }
      this.getAllNewsLetter();
    });

  }
}
