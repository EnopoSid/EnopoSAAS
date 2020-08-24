import { Injectable } from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class ReferralSectionService {

  isOurReferralView: boolean;
  constructor(private sendReceiveService: SendReceiveService) { }
  getAllReferral() {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/allReferralSection", {});
  }
  UserView(userView: boolean) {
    this.isOurReferralView = userView
  }
  getReferralListById(ID: number) {

    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/ReferralSectionById?id=" + ID, {});
  }
  saveReferralList(referraldata) {   
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/referralsection/post", referraldata, MyAppHttp.REQUEST_TYPES.ADD);
  }
  editReferralList(referralText, ID) {    
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/referralssection/put?id=" + ID, referralText, MyAppHttp.REQUEST_TYPES.UPDATE);
  }
  deleteReferral(ID) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/ReferralSection/delete?id=" + ID, {}, MyAppHttp.REQUEST_TYPES.DELETE);
  }
  activateRecord(ID) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Referralsectionstatus/put?id=" + ID, {}, MyAppHttp.REQUEST_TYPES.UPDATE);
  }
}