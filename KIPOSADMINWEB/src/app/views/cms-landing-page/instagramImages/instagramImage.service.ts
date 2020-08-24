import {Injectable} from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class InstagramPageService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllInstagramImages() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/KiposInstagramPageImage/GetallActiveimages",{});
    }
    getViewInstagramImages() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/KiposInstagramPageImage/Getallimages",{});
    }

    saveInstagramImages(MemberData,file: FileList) {
        let formdata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
                formdata.append( file[i]['name'],file[i]);
            }
        }
        formdata.append('objArr', JSON.stringify( MemberData ))
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post , 'api/KiposInstagramPageImage/insert', formdata,MyAppHttp.REQUEST_TYPES.ADD)   
    }
    editInstagramImagesList(storyText,file: FileList, ID){        
        let storydata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
              storydata.append( file[i]['name'],file[i]);
            }
        }
        storydata.append('objArr', JSON.stringify( storyText ))
          return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/KiposInstagramPageImage/Update",storydata);
      }
      getInstagramImagesListById(ID: number) {        
         return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/KiposInstagramPageImage/GetImageById?id="+ID,{});
        
       }
       activateRecord(ID){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/KiposInstagramPageImage/changeviewstatus?id="+ID,{},MyAppHttp.REQUEST_TYPES.UPDATE);
     }
     deleteImage(ID){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/KiposInstagramPageImage/Delete?id="+ ID,{},MyAppHttp.REQUEST_TYPES.DELETE);
     }

       deleteInstagrame(id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/KiposInstagramPageImage/Delete?id=" + id, {})

    }
    viewinstallDetails(id,orderid) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/KiposInstagramPageImage/changeviewstatus?id=" + id+"&orderid="+ orderid,{})

    }


 



 




}