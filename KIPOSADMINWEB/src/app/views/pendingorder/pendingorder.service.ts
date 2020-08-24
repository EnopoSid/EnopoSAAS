import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { Observable } from 'rxjs';

@Injectable()
export class pendingorderService {
    HMACKey: string;
    userData:any;
    constructor(private sendReceiveService: SendReceiveService) { }
    interval;
    public setHMACKey(input) {
        this.HMACKey = input;
    }

    
    getPendingorders(){
        this.userData = JSON.parse(localStorage.getItem('userData'));
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/posorders/GetAllPOSandOnlineOrders?storeid='+this.userData.StoreId,{});
    }

    UpdateOrderStaus(id:number){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/posorders/UpdateOrderStatusInAdmin?orderid='+id,{},MyAppHttp.REQUEST_TYPES.UPDATE);
    }
    
    UpdateStatusOrderList(orderIds){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/posorders/UpdateOrderListStatusInAdmin',orderIds,MyAppHttp.REQUEST_TYPES.UPDATE);
    }

    CancelOrderStaus(id:number){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/posorders/UpdateOrderStatusInAdminCancelled?orderid='+id,{},MyAppHttp.REQUEST_TYPES.CANCEL);
    }
    getAllConfigurations() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Admin/Configuration/Get",{});
    }
}