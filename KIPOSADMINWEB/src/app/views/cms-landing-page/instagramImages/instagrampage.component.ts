import { Component, OnInit, ViewChild, ChangeDetectorRef, ViewContainerRef, ViewEncapsulation, ElementRef } from '@angular/core';
import { MatDialog, MatDialogRef, MatTableDataSource, MatPaginator, MatSort, MatDatepicker } from '@angular/material';

import { Router } from '@angular/router';

import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';

import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';
import { ConfigurationModel, IPageLevelPermissions, InstagramModel } from 'src/app/helpers/common.interface';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { InstagramPageService } from './instagramImage.service';
import { environment } from 'src/environments/environment';
import { ComfirmComponent } from '../../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';
import { ConfigurationService } from '../../configurations/configuration.service';



@Component({
    selector: 'app-instagrampage',
    templateUrl: './instagrampage.component.html',
    encapsulation: ViewEncapsulation.None
})
export class InstagramPageComponent implements OnInit {
    appURL = environment.url;
    urlLink = environment.urlLink
    displayedColumns = ['sno', 'orderid', 'ulrpath', 'image', 'ImagePath', 'IsActive', 'Actions']
    dataSource: MatTableDataSource<InstagramModel>;
    gridData = [];
    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: false }) sort: MatSort;
    @ViewChild('myInput',{ static: false }) myInputVariable: ElementRef;
    instaImage;
    filterData;
    width:number;
    height:number;
    order1width; 
    order1height;
    orderAllwidth; 
    orderAllheight;
    status: boolean;
    title: string;
    orderid: number;
    @ViewChild('myTable', { static: false }) table: any;
    @ViewChild('tableWrapper', { static: false }) tableWrapper;
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    AddInstagramPageFlag: boolean = false;
    InstagramPageForm: FormGroup;
    userId: number = 0;
    submitted: boolean = false;
    idOnUpdate: number = 0;
    instaId: number;
    imageId = [{ id: 1, Value: '1' },
    { id: 2, Value: '2' },
    { id: 3, Value: '3' },
    { id: 4, Value: '4' },
    { id: 5, Value: '5' },
    { id: 6, Value: '6' },
    { id: 7, Value: '7' }];
    updateInsta;
    imageSize;
    showImageError:boolean=false;
    pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
    selectedFiles: FileList;
    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: InstagramPageService,private configurationService :ConfigurationService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        private formBuilder: FormBuilder) {

    }
    ngOnInit() {
        this.order1width= 600; 
        this.order1height=550;
        this.orderAllwidth=450; 
        this.orderAllheight=300;
          document.getElementById('preloader-div').style.display = 'block';
        this.userId = this.sendReceiveService.globalUserId;
        this.filterData = {
            filterColumnNames: [
                { "Key": 'sno', "Value": " " },
                { "Key": "orderid", "Value": "" },               
                { "Key": 'ulrpath', "Value": " " },
                { "Key": 'image', "Value": " " },
                { "Key": 'ImagePath', "Value": "" },
                { "Key": 'IsActive', "Value": "" },
            ],
            gridData: this.gridData,
            dataSource: this.dataSource,
            paginator: this.paginator,
            sort: this.sort
        };
        this.getImageSize();
        this.getAllInstagramPageImages();
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
            this.pagePermissions = pageLevelPermissions.response;
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
        this.InstagramPageForm = this.formBuilder.group({
            'url': [null, Validators.compose([Validators.required])],
            'orderid': ['', Validators.compose([Validators.required])],

        });
    }
    getImageSize() {

        this.configurationService.getAllConfigurations().subscribe(success => {
     var cartMinAmt = success.filter(function (e) {
              return e.Key == 'ImageSize'
            });
            this.imageSize = parseInt(cartMinAmt[0].Value) ;
          
        }, error => {
            this.sendReceiveService.showDialog('Please configure image size');
          document.getElementById('preloader-div').style.display = 'none';
        })
      }
    getAllInstagramPageImages() {
        document.getElementById('preloader-div').style.display = 'block';
        this.service.getViewInstagramImages().subscribe((response) => {
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
            document.getElementById('preloader-div').style.display = 'none';
        }, (error) => {
            document.getElementById('preloader-div').style.display = 'none';
        });
    }
    get f() { return this.InstagramPageForm.controls; }
    onInstagramPageFormSubmit() {
        let url: string = this.InstagramPageForm.value.url;
        let orderid: string = this.InstagramPageForm.value.orderid;
        let FileUpload = this.selectedFiles;

        if (!this.InstagramPageForm.valid) {
            this.submitted = true;
            return;
        }
        if(this.selectedFiles !=undefined && this.selectedFiles.length!=0 ){
          document.getElementById('preloader-div').style.display = 'block';
        if (this.title == "Save") {

            this.service.saveInstagramImages({
                'OrderId': parseInt(orderid),
                'URLPath': url,
                "IsActive": false,
                "IsCanview": true,
                'CreatedBy': this.userId,
                'ModifiedBy': this.userId,
                'CreatedDate': new Date(),
                'ModifiedDate': new Date(),
            }, FileUpload).subscribe((data) => {
                this.getAllInstagramPageImages();
                this.InstagramPageForm.reset();
                this.instaImage = "";
                this.AddInstagramPageFlag = false;
        
            }, error => {
                document.getElementById('preloader-div').style.display = 'none';
            });

        }
        else if (this.title == "Update") {
           let updateData = this.filterData.gridData.filter(data => {
                return data.id == this.instaId;
            })
            this.service.editInstagramImagesList({
                "Id": this.instaId,
                'OrderId': parseInt(orderid),
                'URLPath': url,
                'IsActive': this.updateInsta.IsActive,
                'IsCanview': this.updateInsta.IsCanview,
                'CreatedBy': this.userId,
                'ModifiedBy': this.userId,
                'CreatedDate': new Date(),
                'ModifiedDate': new Date(),
            }, FileUpload, this.instaId)
                .subscribe((data) => {
                    this.title = "Save";
                    this.getAllInstagramPageImages();
                    this.InstagramPageForm.reset();
                    this.instaImage = "";
                    this.AddInstagramPageFlag = false;
       
                }, error => {
                    this.sendReceiveService.showDialog('An error occured');
                    document.getElementById('preloader-div').style.display = 'none';;
                });

        }
    }else{
        this.sendReceiveService.showDialog('Please select Image size of ' +this.imageSize+ 'MB');
      }
    }
    
    activateRecord(event, id, orderid) {
       
            const dialogRef = this.dialog.open(ComfirmComponent, {
                width: '300px',
                height: "180px",
                data: "Do you want to change Status ?"

            });
            dialogRef.afterClosed().subscribe(result => {
                if (!!result) {
                    this.service.viewinstallDetails(id, orderid).subscribe((data) => {
                        this.getAllInstagramPageImages();
                    }, error => {

                    });
                }
                this.getAllInstagramPageImages();
            });
        
      
    }
    deleteImage(id,isActive) {
      if(isActive == 'true'){
        this.sendReceiveService.showDialog('Atleast one record need to be active');
      }else{
        const dialogRef = this.dialog.open(ComfirmComponent, {
            width: '300px',
            height: "180px",
            data: "Do you want to Delete?"

        });
        dialogRef.afterClosed().subscribe(result => {
            if (!!result) {
                this.service.deleteImage(id).subscribe((data) => {
                    this.getAllInstagramPageImages();
                }, error => {

                });
            }
        });
      }

        
    
    }
    updatePagination(){
        this.filterData.dataSource=this.filterData.dataSource;
        this.filterData.dataSource.paginator = this.paginator;
        }


    onCancel() {
        this.idOnUpdate = 0;
        this.instaImage = "";
        this.orderid = 0;
        this.InstagramPageForm.reset();
        this.AddInstagramPageFlag = false;

    }

    upload(event) {
        let orderid = this.InstagramPageForm.value.orderid;
        if (this.title == "Update") {
            orderid =  this.updateInsta.OrderId;
        }
if(orderid!= undefined && orderid!=""){
    const reader = new FileReader();
    const file = event.target.files[0];
   if (event.target.files && event.target.files[0]) {
       var FileSize = event.target.files[0].size / 1024 / 1024; // in MB
       if (FileSize > this.imageSize) {
           this.sendReceiveService.showDialog('File size exceeds '+this.imageSize+ 'MB');
           this.reset();
           this.instaImage="";
       }
        else{
               
           let image:any = event.target.files[0];
         
           let fr = new FileReader();
           fr.onload = () => { 
            var img:any = new Image();
        
            img.onload = () => {
                this.width = img.width;
                this.height = img.height;
            };
        
            img.src = fr.result;
           };
        
          fr.readAsDataURL(image);
          } 
    setTimeout(() => {
        if(orderid== 1){
           if( this.width <= this.order1width && this.height <= this.order1height ) {
               this.sendReceiveService.showDialog('For better visibility photo should be more than ' +this.order1width + " x " + this.order1height + " size");
                this.reset();
           this.instaImage="";
             }
        }else{
           if( this.width <= this.orderAllwidth && this.height <= this.orderAllheight ) {
               this.sendReceiveService.showDialog('For better visibility photo should be more than ' +this.orderAllwidth + " x " + this.orderAllheight + " size");
                this.reset();
           this.instaImage="";
             } 
        }
       this.selectedFiles = event.target.files;
           const file = event.target.files[0];
    reader.onload = e => this.instaImage = reader.result;
     
           reader.readAsDataURL(file);
           
    }, 500);
          
    
   }
}else{
    this.sendReceiveService.showDialog('Please select image serial number');
    this.reset();
}
        
 
    }
    reset() {
      this.myInputVariable.nativeElement.value = "";
    }
    onChangeproducts(events) {
        if (!!events) {
            this.orderid = events;
            this.reset();
            this.instaImage="";
        }
    }
    editImages(id: number) {
        this.instaId = id;
        this.AddInstagramPageFlag = true;
        this.processEditAction(id);
        this.title = "Update";
    }
    processEditAction(id) {
        this.service.getInstagramImagesListById(id)
            .subscribe(resp => {
               
                this.updateInsta = resp;
               this.InstagramPageForm.patchValue({
                    id: resp.Id,
                    url: resp.URLPath,
                    orderid: resp.OrderId,

                });
                this.instaImage = this.appURL + "/" + resp.ImagePath;
            })
    }
    addInstagramPageMember() {
        this.AddInstagramPageFlag = true;
        this.title = "Save"
    }
    goToLink(url: string) {
        window.open(this.urlLink + url, "_blank");
    }
}
