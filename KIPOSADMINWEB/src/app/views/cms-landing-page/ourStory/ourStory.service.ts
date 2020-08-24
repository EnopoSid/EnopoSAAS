import { Injectable} from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class OurStoryComponentService {

  isOurStoryView :boolean;
    constructor(private sendReceiveService: SendReceiveService) { }
    getAllStory() {
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/allourstorys",{});
    } 
    UserView(userView : boolean)
    {
       this.isOurStoryView = userView
    }
    getStoryListById(ID: number) {
     
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/OurStoryById?id="+ID,{});
        }
        saveStoryList(storydata,file: FileList){
          
          let formdata : FormData = new FormData();
          if(file!=null){
              for(let i =0; i < file.length; i++){
                formdata.append( file[i]['name'],file[i]);
              }
          }
          formdata.append('objArr', JSON.stringify( storydata ))
          return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/ourstory/post",formdata,MyAppHttp.REQUEST_TYPES.ADD);
      }
      editStoryList(storyText,file: FileList,ID){
        
        let storydata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
              storydata.append( file[i]['name'],file[i]);
            }
        }
        storydata.append('objArr', JSON.stringify( storyText ))
          return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/ourstory/put?id="+ID,storydata,MyAppHttp.REQUEST_TYPES.UPDATE);
      }
      deleteStory(ID){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/ourstory/delete?id="+ ID,{},MyAppHttp.REQUEST_TYPES.DELETE);
     }
     activateRecord(ID){
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/ourstorystatus/put?id="+ID,{},MyAppHttp.REQUEST_TYPES.UPDATE);
   }
}