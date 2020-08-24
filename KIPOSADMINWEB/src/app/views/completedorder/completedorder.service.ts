import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { Observable } from 'rxjs';

@Injectable()
export class completedorderService {
    HMACKey: string;
    userData:any;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    
    getCompletedorder(){
        this.userData = JSON.parse(localStorage.getItem('userData'));
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/posorders/GetAllPOSandOnlineOrdersComplete?storeid='+this.userData.StoreId,{});
    }

  
    
}