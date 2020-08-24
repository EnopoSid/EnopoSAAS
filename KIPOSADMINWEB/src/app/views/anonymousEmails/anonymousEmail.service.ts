import { Injectable} from '@angular/core';
import { SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class AnonymousEmailService { 
    constructor(private sendReceiveService: SendReceiveService) { }
    getAllEmails() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/anonymouseemail",{});
       }
}