import { Injectable} from '@angular/core';
import { SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class CareerService { 
    constructor(private sendReceiveService: SendReceiveService) { }
    getAllCareerList() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/getCareers",{});
       }
       downloadFile(FielId)
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/DownloadFile?id='+FielId,{})
    }
}