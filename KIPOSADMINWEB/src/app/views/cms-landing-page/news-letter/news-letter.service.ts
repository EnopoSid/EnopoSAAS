import { Injectable } from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class NewsLetterComponentService {

  isOurStoryView: boolean;
  constructor(private sendReceiveService: SendReceiveService) { }
  getAllNewsLetter() {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/NewsLetterContent/GetNewsLetterContent", {});
  }

  UserView(userView: boolean) {
    this.isOurStoryView = userView
  }

  getNewsLetterListById(ID: number) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/NewsLetterContent/GetNewsLetterContentById?id=" + ID, {});
  }
  sendMessage(id){
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/NewsLetterContent/sendnews?id="+id,{}, 'POST');
  }
  saveNewsLetterList(storydata) {    
   
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/NewsLetterContent/PostNewsLetterContent", storydata, MyAppHttp.REQUEST_TYPES.ADD);
  }
  editNewsLetterList(storyText, ID) {   
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/NewsLetterContent/PutNewsLetterContent?id=" + ID, storyText, MyAppHttp.REQUEST_TYPES.UPDATE);
  }
  deleteArticle(ID) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/NewsLetterContent/DeleteNewsLetterContent?id=" + ID, {}, MyAppHttp.REQUEST_TYPES.DELETE);
  }
  activateRecord(ID, IsActive) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Articles/Articleput?Id=" + ID+"&IsActive="+IsActive, {}, MyAppHttp.REQUEST_TYPES.UPDATE);
  }
}