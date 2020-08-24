import { Injectable} from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class CategoriesService {

  isOurStoryView :boolean;
    constructor(private sendReceiveService: SendReceiveService) { }
    getAllCategories() {
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/GetAllCategory",{});
    } 
    UserView(userView : boolean)
    {
       this.isOurStoryView = userView
    }
      
     activateRecord(categoryObj){
      let apiuRL ="api/client/UpdateCategoryForOnlinePos"
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,`${apiuRL}`,categoryObj,null,true);
   }
}