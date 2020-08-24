import { Injectable} from '@angular/core';
import { SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class CorpMasterService { 
    constructor(private sendReceiveService: SendReceiveService) { }
    getAllDomain() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Domain/Get",{});
       }
       getAllDomainByid(id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Domain/GetByID?id="+id,{});
       }
       getAllDiscounts() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Domain/NopDisocunt",{});
       }

       SaveDomain(Domian){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Domain/Insert",Domian,MyAppHttp.REQUEST_TYPES.ADD)   
      }
      updateDomain(SubMenu) {
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Domain/Update",SubMenu,MyAppHttp.REQUEST_TYPES.UPDATE);
      }
       Delete(id) {
       return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Domain/Delete?id="+id,{});
       }
      duplicateCompany(Domainobj){
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Domain/DuplicateCompany",Domainobj);
       }
       duplicateDomain(Domainobj){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Domain/DuplicateDomain",Domainobj);
         }
  
}