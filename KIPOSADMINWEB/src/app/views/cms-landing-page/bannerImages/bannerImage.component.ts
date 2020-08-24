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
import { BannerImageComponentService } from './bannerImage.service';
import { environment } from 'src/environments/environment';
import { ComfirmComponent } from '../../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';
@Component({
  selector: 'app-bannerImage',
  templateUrl: './bannerImage.component.html',
  encapsulation: ViewEncapsulation.None
})
export class BannerImageComponent implements OnInit {
  appURL = environment.url;
  displayedColumns = ['sno', 'Id', 'IsFromPageType', 'image', 'ImagePath', 'IsActive', 'Actions']
  dataSource: MatTableDataSource<BannerImageModel>;
  gridData = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  dialogRef: MatDialogRef<any>;
  filterData;
  BrandImageForm: FormGroup;
  AddBrandImageFlag: boolean = false;
  userId: number = 0;
  updateText;
  pageName: string = "LandingPage";
  title: string;
  checked = true;
  editImage;
  imageId: any;
  selectedFiles: FileList;
  selectedImage;
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
    public service: BannerImageComponentService,
  ) {
    this.pageName = "LandingPage";
  }

  ngOnInit() {
      document.getElementById('preloader-div').style.display = 'block';
    this.userId = this.sendReceiveService.globalUserId;
    this.filterData = {
      filterColumnNames: [
        { "Key": 'sno', "Value": " " },
        { "Key": 'Id', "Value": " " },
        { "Key": 'IsFromPageType', "Value": " " },
        { "Key": 'image', "Value": " " },
        { "Key": 'ImagePath', "Value": " " },
        { "Key": 'IsActive', "Value": "" },

      ],
      gridData: this.gridData,
      dataSource: this.dataSource,
      paginator: this.paginator,
      sort: this.sort
    };
    this.getAllBrandImage();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
      this.pagePermissions = pageLevelPermissions.response;
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    this.BrandImageForm = this.formBuilder.group({
      'ImagePath': [null, Validators.required],
    });

  }

  getAllBrandImage() {
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllBrandImage(this.pageName).subscribe((response) => {
      const configurationData: any = [];
      for (let i = 0; i < response.length; i++) {
        response[i].sno = i + 1;
        response[i].IsActive = response[i].IsActive.toString();
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
  onBrandImageFormSubmit() {

    let FileUpload = this.selectedFiles;
    if (!this.BrandImageForm.valid) {
      return;
    }
    console.log(this.BrandImageForm.value)
    if (this.title == "Save") {
      this.service.saveBrandImageList({
        "IsFromPageType": this.pageName,  
        "IsActive":false,
        "iscanview":true      
      }, FileUpload).subscribe((data) => {
        this.getAllBrandImage();
        this.BrandImageForm.reset();
        this.editImage = "";
        this.AddBrandImageFlag = false;
      }, error => {
        this.brandImageFormErrors = error;
      });

    }
    else if (this.title == "Update") {
      let var_id: string = this.BrandImageForm.value.id;// on edit menu form
      this.service.editBrandImageList({
        "id": this.imageId,
        "IsFromPageType": this.pageName,        
        "IsCanview":this.updateText.IsCanview,
        'IsActive': this.updateText.IsActive,
      }, FileUpload, this.imageId)
        .subscribe((data) => {
          this.title = "Save";

          this.getAllBrandImage();
          this.BrandImageForm.reset();
          this.editImage = "";
          this.AddBrandImageFlag = false;
        }, error => {
          this.brandImageFormErrors = error;
        });

    }

  }
  selectFile(event) {
    this.selectedFiles = event.target.files;
    if (event.target.files && event.target.files[0]) {
      const file = event.target.files[0];

      const reader = new FileReader();
      reader.onload = e => this.editImage = reader.result;

      reader.readAsDataURL(file);
    }
  }
  addBrandImage() {
    this.AddBrandImageFlag = true;
    this.title = "Save"
  }
  onCancel() {
    this.editImage = "";
    this.BrandImageForm.reset();
    this.AddBrandImageFlag = false;
  }

  updatePagination(){
    this.filterData.dataSource=this.filterData.dataSource;
    this.filterData.dataSource.paginator = this.paginator;
    }

  clkRadioBtn(page) {

    this.AddBrandImageFlag = false;
    this.BrandImageForm.reset();

    this.editImage = "";
    console.log(page);
    if (page == 0) {
      this.pageName = "LandingPage";

    } else if (page == 1) {
      this.pageName = "HomePage";

    }
    this.getAllBrandImage();
  }
  editImages(id: number) {
    this.imageId = id;
    this.AddBrandImageFlag = true;
    this.processEditAction(id);
    this.title = "Update";



  }
  processEditAction(id) {
    this.service.getAllBrandImageListById(id, this.pageName)
      .subscribe(resp => {
        this.BrandImageForm.patchValue({
          id: resp[0].Id,
          File: resp[0].ImagePath,

        });
        this.updateText = resp[0];
        this.editImage = this.appURL + "/" + resp[0].ImagePath;
      })
  }
  deleteImage(id) {
    const dialogRef = this.dialog.open(ComfirmComponent, {
      width: '300px',
      height: "180px",
      data: "Do you want to Delete?"

    });
    dialogRef.afterClosed().subscribe(result => {
      if (!!result) {
        this.service.deleteBrandImage(id, this.pageName).subscribe((data) => {
          this.getAllBrandImage();
        }, error => {

        });
      }
    });
  }
  activateRecord(event, id) {

    if (event == true) {
      const dialogRef = this.dialog.open(ComfirmComponent, {
        width: '300px',
        height: "180px",
        data: "Do you want to change status?"

      });
      dialogRef.afterClosed().subscribe(result => {
        if (!!result) {
          this.service.activateRecord(id, this.pageName).subscribe((data) => {
            this.getAllBrandImage();
          }, error => {

          });
        }
        this.getAllBrandImage();
      });
    }
    else {
      this.sendReceiveService.showDialog('Atleast one status is to be Active');
      this.getAllBrandImage();
    }
  }
}
