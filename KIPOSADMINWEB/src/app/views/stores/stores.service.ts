import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class storesService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllPlans() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"Api/Client/GetPickupAddress",{});
    }
    getPickupAddress() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'GetAllStorePickUpPointsAdmin', 
        {});
    }
    
    activateRecord(storeObj,id){
        let apiuRL ="UpdateStorePickUpPoint";
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,apiuRL+"?Id="+id,storeObj);
     }  
}