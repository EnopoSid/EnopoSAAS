import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class SiteReviewService {
    HMACKey: string;

    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }
  
    getAllSiteReviews() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/SiteReviews/GetAllAdminReviews",{});
    }
    updateIsApproved(sitereview ){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/SiteReviews/UpdateIsApproved",sitereview)
    }
    UpdateAllIsApproved(sitereview){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/SiteReviews/UpdateAllIsApproved",sitereview)
    }
    deletereview(reviewId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/SiteReviews/DeleteReview?reviewId="+ reviewId, {})

    }
}
