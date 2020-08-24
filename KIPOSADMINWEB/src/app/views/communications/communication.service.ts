import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class CommunicationService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllCommunicationTypes() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Communication/Get",{});
    }

    getCommunicationTypeById(id: number) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Communication/GetById?id="+id,{});
    }

    saveCommunicationType(CommunicationType) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post , 'api/Communication/Post', CommunicationType,MyAppHttp.REQUEST_TYPES.ADD)   
    }
    updateCommunicationType(CommunicationType,id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Communication/Put?id='+id, CommunicationType,MyAppHttp.REQUEST_TYPES.UPDATE)
    }

    deleteCommunicationType(id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Communication/Delete?id=" + id, {},MyAppHttp.REQUEST_TYPES.DELETE)

    }
    duplicateCommunicationType(CommunicationType){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Communication/Duplicate', CommunicationType)
    }

}