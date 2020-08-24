import { Injectable} from '@angular/core';
import { SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class CmsLandingPageService { 
    constructor(private sendReceiveService: SendReceiveService) { }
   
}