import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { Observable } from 'rxjs';

@Injectable()
export class KpointsService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    
    getKpoints(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/posorders/GetPOSandOnlineOrdersKPoints',{});
    }

    getPlansById(id: number) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Plans/GetbyId?id="+id,{});
        }
        saveKpoints(Kpoints) {
            return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/PostManualRewardPoints',Kpoints,MyAppHttp.REQUEST_TYPES.ADD);   
        }
        getAddedKpoints(){
            return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/GetManualRewardPoints',{});
        } 
        updatePlans(Plans,id) {
            return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Plans/Put?id='+id,Plans,MyAppHttp.REQUEST_TYPES.UPDATE);
        }
        DeleteKpoints(MemberId){
            return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/DeleteManualRewardPoints?MemberId='+MemberId,MyAppHttp.REQUEST_TYPES.DELETE);
         }
         getIsLoyalityMemberDetails(memberId) {
            return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/POSUser/GetIsLoyalityMemberDetails?memberId='+memberId, {});
        
          }
    
}