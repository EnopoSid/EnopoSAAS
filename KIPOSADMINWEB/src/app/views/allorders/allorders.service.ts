import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { Observable } from 'rxjs';

@Injectable()
export class ALLOrdersService {
    HMACKey: string;
    userData:any;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    
    Allorders(){
        
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/posorders/GetAllOrders',{},{});
    }

  
    
}