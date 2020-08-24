import { Component, OnInit, ViewEncapsulation, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';

import { Subject } from 'rxjs';
import { IPageLevelPermissions } from 'src/app/helpers/common.interface';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { environment } from 'src/environments/environment';
import { FeaturedArticlesComponentService } from '../featuredArticles.service';

@Component({
  selector: 'app-addFeaturedArticles',
  templateUrl: './addFeaturedArticles.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AddFeaturedArticlesComponent implements OnInit {
  updateText: any;
  submitted: boolean = false;
  appURL = environment.url;
  editImage;
  userId: number = 0;
  selectedFiles: FileList;
  featuredArticlesFormGroup: FormGroup;
  id: number = 0;
  idOnUpdate: number = 0;
  pattern = "[a-zA-Z][a-zA-Z ]*";
  title: string = "Featured Articles";
  saveUser: boolean = false;
  updateUser: boolean = false;
  checkInput: boolean = false;
  addStoryFormErrors = {

    titleName: {},
    postedby: {},
    shortDescription: {},
    fullDescription: {},
    File: {}
  };
  pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
  isOnView: boolean = false;

  subDescriptionEditor: AngularEditorConfig = {
    editable: true,
    spellcheck: true,

    minHeight: '5rem',
    placeholder: 'Enter Short Description',
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
    public service: FeaturedArticlesComponentService,
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
      this.title = "Add Featured Articles"
    }
    if (!!this.id) {
      this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
        if (!!this.service.isOurStoryView) {
          this.title = "View Featured Articles - " + this.id;
        }
        else {
          this.title = "Edit Featured Articles - " + this.id;
        }
      })
    }
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
      this.pagePermissions = pageLevelPermissions.response;
      var temproute = this.router.url;
      if (temproute.indexOf('add') > -1) {
        this.featuredArticlesFormGroup.enable();
        if (!this.pagePermissions.Add)
          this.sendReceiveService.logoutService();
      }

      if (temproute.indexOf('update') > -1) {
        this.featuredArticlesFormGroup.enable();
        if (!this.pagePermissions.Edit)
          this.sendReceiveService.logoutService();
      }

      if (temproute.indexOf('view') > -1) {
        this.featuredArticlesFormGroup.disable();
        this.isOnView = true;
        if (!this.pagePermissions.View)
          this.sendReceiveService.logoutService();
      }
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });


    if (!isNaN(this.id)) {
      this.featuredArticlesFormGroup = this.formBuilder.group({
        'id': 0,
        'titleName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'postedby': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'shortDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
        'fullDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
      });
      this.idOnUpdate = this.id;
      this.saveUser = false;
      this.updateUser = true;
      this.checkInput = true;
      this.service.getArticleListById(this.id).subscribe(resp => {
        this.featuredArticlesFormGroup.patchValue({
          id: resp[0].ArticleId,
          isCanView: resp[0].IsCanView,
          titleName: resp[0].ArticleTitleName,
          postedby: resp[0].ArticlePostedBy,
          shortDescription: resp[0].ArticleDescription,
          fullDescription: resp[0].FullDescription,
        });
        this.updateText = resp[0];
        this.editImage = this.appURL + resp[0].ImagePath;
      });
    } else {
      this.featuredArticlesFormGroup = this.formBuilder.group({
        id: 0,
        'titleName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'postedby': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'shortDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
        'fullDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
        'File': [null, Validators.required]
      });
      this.saveUser = true;
      this.updateUser = false;
      this.checkInput = false;
    }

    if (this.service.isOurStoryView) {
      this.featuredArticlesFormGroup.disable();
      this.isOnView = true;
    }
    else {
      this.isOnView = false;
    }

  }
  get f() { return this.featuredArticlesFormGroup.controls; }
  onFeaturedArticlesSubmit() {

    if (!this.featuredArticlesFormGroup.valid) {
      this.submitted = true;
      return false;
    }
    this.submitted= false;
    let var_id: number = this.featuredArticlesFormGroup.value.id;
    let var_titleName: string = this.featuredArticlesFormGroup.value.titleName;
    let var_postedby: string = this.featuredArticlesFormGroup.value.postedby;
    let var_shortDescription: string = this.featuredArticlesFormGroup.value.shortDescription;
    let var_fullDescription: string = this.featuredArticlesFormGroup.value.fullDescription;
    let var_image = this.selectedFiles;



    if (this.saveUser) {

      this.service.saveArticleList({
        'CreatedDate': new Date(),
        'ModifiedDate': new Date(),
        "ModifiedBy": this.userId,
        "CreatedBy": this.userId,
        "IsActive": MyAppHttp.ACTIVESTATUS,
        "IsCanView": true,
        "ArticleTitleName": var_titleName,
        "ArticlePostedBy": var_postedby,
        "ArticleDescription": var_shortDescription,
        "FullDescription": var_fullDescription,        
      }, var_image).subscribe((data) => {
        this.editImage = "";
        this.router.navigate(['/landingpage/featuredArticles']);

      },
        error => this.addStoryFormErrors = error
      )
    }
    else if (this.updateUser) {
      this.idOnUpdate = 0;
      this.featuredArticlesFormGroup = this.formBuilder.group({
        id: 0,
        'titleName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'postedby': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'shortDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
        'fullDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
      });
      this.service.editArticleList({
        'CreatedDate': new Date(),
        'ModifiedDate': new Date(),
        "ModifiedBy": this.userId,
        "CreatedBy": this.userId,
        'ArticleId': var_id,
        "ArticleTitleName": var_titleName,
        "ArticlePostedBy": var_postedby,
        "ArticleDescription": var_shortDescription,
        "FullDescription": var_fullDescription,
        'IsActive': this.updateText.IsActive,
        'isCanView': this.updateText.IsCanView,
      }, var_image, var_id)
        .subscribe((data) => {
          this.editImage = "";
          this.router.navigate(['/landingpage/featuredArticles']);
        }, error => {
          this.sendReceiveService.showDialog('An error occured');
        })
    }
  }
  onGoBack() {
    this.editImage = "";
    this.idOnUpdate = 0;
    this.router.navigate(['/landingpage/featuredArticles']);

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