import { Component, OnInit } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Router} from '@angular/router';
import MyAppHttp from '../../../services/common/myAppHttp.service';


@Component({
  selector: 'footer',
  templateUrl: './footer.component.html'
})
export class FooterComponent {
  menuItems:any[];
  appVersion: string;
  subMenuItems: any[];

  constructor() {}
  ngOnInit() {
  }


}
