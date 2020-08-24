import { Injectable} from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';

@Injectable()
export class NewsLetterMailService { 
    constructor(private sendReceiveService: SendReceiveService) { }
    getAllEmails() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/NewsLetter/GetNewsLetter",{});
       }
}