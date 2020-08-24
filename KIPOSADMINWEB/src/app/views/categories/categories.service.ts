import { Injectable} from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class CategoriesService {

  isOurStoryView :boolean;
  HMACKey: string;

    constructor(private sendReceiveService: SendReceiveService) { }
    public setHMACKey(input) {
      this.HMACKey = input;
  }

    getAllCategories() {
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Common/getCatSubcatStatus",{});
    } 
    UserView(userView : boolean)
    {
       this.isOurStoryView = userView
    }
      
     activateRecord(CatSubcatStatus){
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Common/UpdateCatSubcatStatus',CatSubcatStatus,MyAppHttp.REQUEST_TYPES.UPDATE);
   }
}