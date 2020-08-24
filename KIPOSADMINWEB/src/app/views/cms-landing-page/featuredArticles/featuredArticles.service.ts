import { Injectable } from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class FeaturedArticlesComponentService {

  isOurStoryView: boolean;
  constructor(private sendReceiveService: SendReceiveService) { }
  getAllFeaturedArticles() {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Articles/GetNuqiArticles", {});
  }

  UserView(userView: boolean) {
    this.isOurStoryView = userView
  }

  getArticleListById(ID: number) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Articles/GetNuqiArticleById?id=" + ID, {});
  }

  saveArticleList(storydata, file: FileList) {    
    let formdata: FormData = new FormData();
    if (file != null) {
      for (let i = 0; i < file.length; i++) {
        formdata.append(file[i]['name'], file[i]);
      }
    }
    formdata.append('objArr', JSON.stringify(storydata))
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Articles/Articlespost", formdata, MyAppHttp.REQUEST_TYPES.ADD);
  }
  editArticleList(storyText, file: FileList, ID) {   
    let storydata: FormData = new FormData();
    if (file != null) {
      for (let i = 0; i < file.length; i++) {
        storydata.append(file[i]['name'], file[i]);
      }
    }
    storydata.append('objArr', JSON.stringify(storyText))
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Articles/Articlesput?id=" + ID, storydata, MyAppHttp.REQUEST_TYPES.UPDATE);
  }
  deleteArticle(ID) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Articles/Articlesdelete?id=" + ID, {}, MyAppHttp.REQUEST_TYPES.DELETE);
  }
  activateRecord(ID, IsActive) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Articles/Articleput?Id=" + ID+"&IsActive="+IsActive, {}, MyAppHttp.REQUEST_TYPES.UPDATE);
  }
}