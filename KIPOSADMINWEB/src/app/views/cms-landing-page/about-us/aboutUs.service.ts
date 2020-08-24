import { Injectable} from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class AboutUsComponentService {

  isAboutUsView :boolean;
    constructor(private sendReceiveService: SendReceiveService) { }
    getAllAboutUs() {
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/AboutUs/GetAboutus",{});
    } 
    UserView(userView : boolean)
    {
       this.isAboutUsView = userView
    }
    getAboutUsListById(ID: number) {
     
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/AboutUs/GetAboutUsById?id="+ID,{});
        }
        saveStoryList(storydata,file: FileList){        
          let formdata : FormData = new FormData();
          if(file!=null){
              for(let i =0; i < file.length; i++){
                formdata.append( file[i]['name'],file[i]);
              }
          }
          formdata.append('objArr', JSON.stringify( storydata ))
          return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/AboutUs/AboutUspost",formdata,MyAppHttp.REQUEST_TYPES.ADD);
      }
      editAboutUsList(storyText,file: FileList,ID){      
        let storydata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
              storydata.append( file[i]['name'],file[i]);
            }
        }
        storydata.append('objArr', JSON.stringify( storyText ))
          return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/AboutUs/AboutUsput?id="+ID,storydata,MyAppHttp.REQUEST_TYPES.UPDATE);
      }
      deleteAboutUs(ID){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/AboutUs/AboutUsdelete?id="+ ID,{},MyAppHttp.REQUEST_TYPES.DELETE);
     }
     activateAboutUsRecord(ID){
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/AboutUs/AboutUsstatusput?id="+ID,{},MyAppHttp.REQUEST_TYPES.UPDATE);
   }
}