import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class ResetpasswordService { 
    constructor(private sendReceiveService: SendReceiveService) { }

    getAllMemberLoginCustomers(emailId: string) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/MemberLoginCustomer/AllCustomers?emailId="+emailId,{});
    }

    ChangePassword(passobj) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"Api/Client/PasswordRecovery",passobj,{},true);
    }
  
}
