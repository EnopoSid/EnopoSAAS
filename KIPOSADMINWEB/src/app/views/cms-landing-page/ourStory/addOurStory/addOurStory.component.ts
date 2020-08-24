import { Component, OnInit, ViewEncapsulation, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog, getMatFormFieldDuplicatedHintError } from '@angular/material';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { OurStoryComponentService } from '../ourStory.service';
import { Subject, from } from 'rxjs';
import { IPageLevelPermissions } from 'src/app/helpers/common.interface';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';
import { AngularEditorModule, AngularEditorConfig } from '@kolkov/angular-editor';
import { environment } from 'src/environments/environment';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
@Component({
  selector: 'app-addOurStory',
  templateUrl: './addOurStory.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AddOurStoryComponent implements OnInit {
  updateText:any;
  submitted :boolean= false;
  appURL = environment.url;
  editImage;
  userId: number = 0;
  selectedFiles: FileList;
  AddStory: FormGroup;
  id: number = 0;
  idOnUpdate: number = 0;
  pattern = "[a-zA-Z][a-zA-Z ]*";
  title: string = "Our Story";
  saveUser: boolean = false;
  updateUser: boolean = false;
  checkInput: boolean = false;
  pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
  isOnView: boolean = false;

  subDescriptionEditor: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
   
    minHeight: '5rem',
    placeholder: 'Enter Sub Description',
    translate: 'no',
    defaultFontName: 'Arial',
    customClasses: [
      {
        name: "quote",
        class: "quote",
      },
      {
        name: 'redText',
        class: 'redText'
      },
      {
        name: "titleText",
        class: "titleText",
        tag: "h1",
      },
    ]
  };
  fullDescriptionEditor: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: '7rem',
    minHeight: '5rem',
    placeholder: 'Enter Full Description',
    translate: 'no',
    defaultFontName: 'Arial',
    customClasses: [
      {
        name: "quote",
        class: "quote",
      },
      {
        name: 'redText',
        class: 'redText'
      },
      {
        name: "titleText",
        class: "titleText",
        tag: "h1",
      },
    ]
  };
  constructor(private spinner: NgxSpinnerService,
    private router: Router,

    public ref: ChangeDetectorRef,
    public dialog: MatDialog,
    public viewContainerRef: ViewContainerRef,
    public logoutService: LogoutService,
    public appInfoService: AppInfoService,
    public service: OurStoryComponentService,
    public sendReceiveService: SendReceiveService, private formBuilder: FormBuilder,
    public translate: TranslateService,
    fb: FormBuilder,

    private route: ActivatedRoute
  ) {
  }

  ngOnInit() {
    this.userId = this.sendReceiveService.globalUserId;
    this.id = +this.route.snapshot.params['id'];
    if (isNaN(this.id)) {
      this.title = "Add Story"
    }
    if (!!this.id) {
      this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
        if (!!this.service.isOurStoryView) {
          this.title = "View Story - " + this.id;
        }
        else {
          this.title = "Edit Story - " + this.id;
        }
      })
    }
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
      this.pagePermissions = pageLevelPermissions.response;
      var temproute = this.router.url;
      if (temproute.indexOf('add') > -1) {
        this.AddStory.enable();
        if (!this.pagePermissions.Add)
          this.sendReceiveService.logoutService();
      }

      if (temproute.indexOf('update') > -1) {
        this.AddStory.enable();
        if (!this.pagePermissions.Edit)
          this.sendReceiveService.logoutService();
      }

      if (temproute.indexOf('view') > -1) {
        this.AddStory.disable();
        this.isOnView = true;
        if (!this.pagePermissions.View)
          this.sendReceiveService.logoutService();
      }
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });


    if (!isNaN(this.id)) {
      this.AddStory = this.formBuilder.group({
        'id': 0,
        'titleName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'subTitle': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'shortDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
        'fullDescription':[null, Validators.compose([Validators.required, Validators.minLength(3)])],
      });
      this.idOnUpdate = this.id;
      this.saveUser = false;
      this.updateUser = true;
      this.checkInput = true;
      this.service.getStoryListById(this.id).subscribe(resp => {     
        this.AddStory.patchValue({
          id: resp[0].Id,
          titleName: resp[0].TitleName,
          subTitle: resp[0].SubTitleName,
          shortDescription: resp[0].ShortDescription,
          fullDescription:resp[0].FullDescription
          
        });
        this.updateText =resp[0];
        this.editImage = this.appURL+resp[0].ImagePath;        
      });
    } else {
      this.AddStory = this.formBuilder.group({
        id: 0,
        'titleName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'subTitle': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'shortDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
        'fullDescription':[null, Validators.compose([Validators.required, Validators.minLength(3)])],
        'File':[null, Validators.required]
      });
      this.saveUser = true;
      this.updateUser = false;
      this.checkInput = false;
    }

    if (this.service.isOurStoryView) {
      this.AddStory.disable();
      this.isOnView = true;
    }
    else {
      this.isOnView = false;
    }

  }
  get f() { return this.AddStory.controls; }
  onAddStorySubmit() {   
    
    if (!this.AddStory.valid) {
      this.submitted =true;
      return;
    }
    this.submitted =false;
    let var_id: number = this.AddStory.value.id;
    let var_titleName: string = this.AddStory.value.titleName;
    let var_subTitle: string = this.AddStory.value.subTitle;
    let var_shortDescription: string = this.AddStory.value.shortDescription;
    let var_fullDescription: string = this.AddStory.value.fullDescription;
    let var_image = this.selectedFiles;
    
    if (this.saveUser) {
     
      this.service.saveStoryList({
        'CreatedDate': new Date(),
        'ModifiedDate': new Date(),
        "ModifiedBy": this.userId,
        "CreatedBy": this.userId,
        "IsActive": MyAppHttp.ACTIVESTATUS,
        'IsCanview':0,
        "TitleName": var_titleName,
        "SubTitleName": var_subTitle,
        "ShortDescription": var_shortDescription,
        "FullDescription":var_fullDescription,
      }, var_image).subscribe((data) => {
        this.editImage ="";
        this.router.navigate(['/landingpage/ourStory']);
      
      },
        error =>  {
          this.sendReceiveService.showDialog('An error occured');
        })
      
    }
    else if (this.updateUser) {
      this.idOnUpdate = 0;
      this.AddStory = this.formBuilder.group({
        id: 0,
        'titleName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'subTitle': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'shortDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
        'fullDescription':[null, Validators.compose([Validators.required, Validators.minLength(3)])],
      });
      this.service.editStoryList({
        'CreatedDate': new Date(),
        'ModifiedDate': new Date(),
        "ModifiedBy": this.userId,
        "CreatedBy": this.userId,
        'Id': var_id,
        "TitleName": var_titleName,
        "SubTitleName": var_subTitle,
        "ShortDescription": var_shortDescription,
        "FullDescription":var_fullDescription,
        'IsCanview':this.updateText.IsCanview,
        'IsActive':  this.updateText.IsActive === 'true' ? true : false,
      }, var_image, var_id)
        .subscribe((data) => {
          this.editImage ="";
          this.router.navigate(['/landingpage/ourStory']);
        }, error => {
          this.sendReceiveService.showDialog('An error occured');
        })
    }
  }
  onGoBack() {
    this.editImage ="";
    this.idOnUpdate = 0;
    this.router.navigate(['/landingpage/ourStory']);

  }
  upload(event) {

    this.selectedFiles = event.target.files;
    if (event.target.files && event.target.files[0]) {
      const file = event.target.files[0];

      const reader = new FileReader();
      reader.onload = e => this.editImage = reader.result;

      reader.readAsDataURL(file);
  }
  }
}
