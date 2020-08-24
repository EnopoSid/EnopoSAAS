import { Injectable} from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class BannerImageComponentService {

  isOurStoryView :boolean;
    constructor(private sendReceiveService: SendReceiveService) { }s
    getAllBrandImage(pageName) {
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/KiposBannerForLandingPage/GetLandingPageBannerImages?PageName="+pageName,{});
    } 
    UserView(userView : boolean)
    {
       this.isOurStoryView = userView
    }
    getAllBrandImageListById(ID: number,pageName) {
     let apiuRL ="api/KiposBannerForLandingPage/GetLandingPageBannerImagesById"
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,`${apiuRL}?id=${ID}&&PageName=${pageName}`,{});
     
    }
   saveBrandImageList(pageName,file: FileList){          
          let formdata : FormData = new FormData();
          if(file!=null){
              for(let i =0; i < file.length; i++){
                formdata.append( file[i]['name'],file[i]);
              }
          }
          formdata.append('objData', JSON.stringify(pageName ))
          return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/KiposBannerForLandingPage/InsertLandingPageBannerImages",formdata,MyAppHttp.REQUEST_TYPES.ADD);
      }
      editBrandImageList(storyText,file: FileList,ID){       
        let storydata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
              storydata.append( file[i]['name'],file[i]);
            }
        }
        storydata.append('objArr', JSON.stringify( storyText ))
          return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/KiposBannerForLandingPage/UpdateLandingPageBannerImages",storydata);
      }
      deleteBrandImage(ID,pageName){
        let apiuRL ="api/KiposBannerForLandingPage/DeleteLandingPageBannerImages"
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,`${apiuRL}?id=${ID}&&PageName=${pageName}`,MyAppHttp.REQUEST_TYPES.DELETE);
     }
     activateRecord(ID,pageName){
      let apiuRL ="api/KiposBannerForLandingPage/ActivateParticularRecord"
      return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,`${apiuRL}?id=${ID}&&PageName=${pageName}`,{});
   }
}