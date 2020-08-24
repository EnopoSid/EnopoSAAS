import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class plansService {
    HMACKey: string;
    isUserView:boolean;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }
    UserView(userView : boolean)
    {
       this.isUserView = userView
    }
    getAllPlans() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Plans/Get",{});
    }

    getPlansById(id: number) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Plans/GetbyId?id="+id,{});
    }

    
    updatePlans(Plans,id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Plans/Put?id='+id,Plans,MyAppHttp.REQUEST_TYPES.UPDATE);
    }

    deletePlans(id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Plans/Delete?id="+id,{},MyAppHttp.REQUEST_TYPES.DELETE);

    }

    savePlans(Plans) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Plans/Post',Plans,MyAppHttp.REQUEST_TYPES.ADD);   
    }
}