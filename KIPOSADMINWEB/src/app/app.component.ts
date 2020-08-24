import {Component, OnInit, HostListener, VERSION} from '@angular/core';
import {Title} from '@angular/platform-browser';
import { environment } from './../environments/environment';
import { PlatformLocation } from '@angular/common';
import { filter } from 'rxjs/operators';

import {
    Router,
    NavigationEnd,
    ActivatedRoute,
} from '@angular/router';

import {
    MatSnackBar
} from '@angular/material';


import {TranslateService} from '@ngx-translate/core';

import AppHttpRequests from './services/common/myAppHttp.service';


import { AppInfoService } from './services/common/appInfo.service';
import {RoutePartsService} from "./services/route-parts/route-parts.service";
import { NgxSpinnerService } from 'ngx-spinner';
import { SendReceiveService } from './services/common/sendReceive.service';





@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    pageTitle = '';
    appTitle = 'Kipos Admin';
    loading = true;
    idleState = 'Not started.';
    timedOut = false;

    version = VERSION;

    constructor(
                public translate: TranslateService,
                private spinner: NgxSpinnerService,
                public sendReceiveService: SendReceiveService,
                public title: Title,
                private router: Router,
                private activeRoute: ActivatedRoute,
                private routePartsService: RoutePartsService,
                public appInfoService: AppInfoService,
                public snackBar: MatSnackBar,
                public location: PlatformLocation,
              ) {

         translate.addLangs(["en"]);
        translate.setDefaultLang('en');

         let browserLang = translate.getBrowserLang();
         translate.use(browserLang.match(/en/) ? browserLang : 'en');


        //Go to splash on browser back
        this.location.onPopState((response) => {
            this.appInfoService.gotoSplash();
        });
    }

    //Go to splash on browser refresh
    @HostListener('window:beforeunload', ['$event'])
    doSomething($event) {
        setTimeout (() => {
            this.appInfoService.gotoSplash();
        }, 1000);
    }

    arrayOfKeys: any;

    ngOnInit() {

        this.changePageTitle();

        
    }

    /*
    ***** Tour Steps ****
    * You can supply tourSteps directly in hopscotch.startTour instead of
    * returning value by invoking tourSteps method,
    * but DOM query methods(querySelector, getElementsByTagName etc) will not work
    */
    tourSteps(): any {
        let self = this;
        return {
            id: 'hello-egret',
            showPrevButton: true,
            onEnd: function () {
                self.snackBar.open('Awesome! Now let\'s explore KIpos Admin Portal\'s cool features.', 'close', {duration: 5000});
            },
            onClose: function () {
                self.snackBar.open('You just closed Admin Portal Tour!', 'close', {duration: 3000});
            },
            steps: [
                {
                    title: 'Sidebar Controls',
                    content: 'Control left sidebar\'s display style.',
                    target: 'sidenavToggle', // Element ID
                    placement: 'bottom',
                    xOffset: 10
                },
                {
                    title: 'Available Themes',
                    content: 'Choose a color scheme.',
                    target: 'schemeToggle', // Element ID
                    placement: 'left',
                    xOffset: 20
                },
                {
                    title: 'Language',
                    content: 'Choose your language.',
                    target: document.querySelector('.topbar .mat-select'),
                    placement: 'left',
                    xOffset: 10,
                    yOffset: -5
                }
            ]
        }
    }

    changePageTitle() {
        this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe((routeChange) => {
            var routeParts = this.routePartsService.generateRouteParts(this.activeRoute.snapshot);
            if (!routeParts.length)
                return this.title.setTitle(this.appTitle);
            // Extract title from parts;
            this.pageTitle = routeParts
                .reverse()
                .map((part) => part.title)
                .reduce((partA, partI) => {
                    return `${partA} > ${partI}`
                });
            this.pageTitle += ` - ${this.appTitle}`;
            this.title.setTitle(this.pageTitle);
        });
    }
}
