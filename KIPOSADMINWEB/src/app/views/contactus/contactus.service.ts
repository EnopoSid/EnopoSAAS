import { Injectable} from '@angular/core';
import { SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class ContactUsService { 
    constructor(private sendReceiveService: SendReceiveService) { }
    getAllEmails() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/getContactUss",{});
       }
}