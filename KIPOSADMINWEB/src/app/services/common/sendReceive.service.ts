import { Injectable, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';


import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable, BehaviorSubject} from 'rxjs';
import {Subject} from 'rxjs';
import 'rxjs';
import { map, take } from 'rxjs/operators';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import MyAppHttp from './myAppHttp.service';

import {Router} from '@angular/router';
import { MatDialog } from '@angular/material';
import { catchError } from 'rxjs/internal/operators/catchError';
import { InfoDialogComponent } from '../../views/app-dialogs/info-dialog/info-dialog.component';
import Utils from 'src/app/services/common/utils';


declare var jsSHA: any;
declare const ActiveXObject: (type: string) => void;
@Injectable()
export class SendReceiveService {
    private _sequence: number = 1;

    myAppUrl: string = environment.url;
    myNopUrl:string=environment.apiURL;
    sendingUrl:string=environment.url;
    header:HttpHeaders;
    accessToken:any = '';
    globalPageLevelPermission = new Subject;
    globalMenuList = new Subject;
    globalRoleId:number  =0; 
    globalDeptId: number =0;
    globalMenuId : number = 0;
    globalSubmenuId: number =0;
    globalUserId: number =0;
    globalExtentionNumber : string;
    globalPhoneNumber : string;
    globalIsPasswordChangedFlag:boolean;
    public httpVerb = {'Get':'Get', 'Post': 'Post', 'Put':'Put', 'Delete': 'Delete'};
    public posRoleIds = {'USER_ID':21,'MANAGER_ID':1024}
    backflag1:boolean=false;
    isInCall : boolean = false;
    isMobileNavigation:boolean;
    currentuserip : string;
    callcategoryId : number;
    private HTTP_REQUEST_PARAMS = {
        HEADER_CONTENT_TYPE: { value: "application/json; charset=UTF-8", valueContains: "application/json;" }
    };

    constructor(private http: Http, private _http: HttpClient, private router: Router,
    private dialog:MatDialog) { }
         
        private messageSource = new BehaviorSubject({});
        currentMessage = this.messageSource.asObservable();
        private messageSourceForFresh = new BehaviorSubject({});
        currentMessageForFresh = this.messageSourceForFresh.asObservable();
         private senddataForSearchOrder = new BehaviorSubject({});
        serachOrderedData= this.senddataForSearchOrder.asObservable();
        SendGlobalParams(data: any) {
            this.messageSource.next(data);
        }
        SendGlobalParamsForFresh(dataForFresh: any) {
            this.messageSourceForFresh.next(dataForFresh);
        }
        SendGlobalParamsForReportOrder(dataForSearchReport: any) {
            this.senddataForSearchOrder.next(dataForSearchReport);
        }
    private extractData(res: Response) {
        let body = res.json();
        Utils.logMessage("SendReceiveService - extractData: ", res);
        this.updateSequence();
        return body || {};
    }

    private subscribeData(res: Response) {
        let body = res.json();
        Utils.logMessage("subscribeData: ", res);
        return body.data || {};
    }

    onInit(){
        if(!!localStorage.getItem("accessToken"))
        {
        var userRoleId =   JSON.parse(localStorage.getItem("accessToken")).roleId;
        var userId =   JSON.parse(localStorage.getItem("accessToken")).id;
        var deptId =   JSON.parse(localStorage.getItem("accessToken")).deptId;

        this.globalRoleId = parseInt(userRoleId);  
        this.globalUserId = parseInt(userId);
        this.globalDeptId = parseInt(deptId);
        }
    }

    createAuthorizationHeader(headers: HttpHeaders){
        this.accessToken = JSON.parse(localStorage.getItem('accessToken'));

       if(!!this.accessToken){
       headers = new HttpHeaders({
        'Content-Type': 'application/json',
         'Authorization': 'bearer ' + this.accessToken.access_token
      });
        }
        
    return headers;
    }

    public send(method= null, apiUrl = null, data = null, requestType= null,isFromNop=false): Observable<any> {
        try {
            if(!!isFromNop){
                data.ApiSecretKey = MyAppHttp.apiKey;
                data.StoreId = MyAppHttp.StoreId;
                data.LanguageId = MyAppHttp.LanguageId;
                data.CurrencyId = MyAppHttp.CurrencyId;
                this.sendingUrl=this.myNopUrl
            }else{
                this.sendingUrl=this.myAppUrl
                this.accessToken = JSON.parse(localStorage.getItem('accessToken'));

                if(!!this.accessToken){
                    if(data.constructor.name === "FormData")
                    {
                        this.header = new HttpHeaders({
                             'Authorization': 'bearer ' + this.accessToken.access_token
                          });
                    }
                    else{
                    this.header = new HttpHeaders({
                        'Content-Type': 'application/json',
                         'Authorization': 'bearer ' + this.accessToken.access_token
                      });
                   }   
                }
    
            }
               if(method == this.httpVerb.Get){
              return this._http.get(this.sendingUrl + apiUrl,{headers: this.header})
                .pipe(map((response: Response) =>{
               return  response;
            })
        )
        
    
            }
            else if(method == this.httpVerb.Post){
                return this._http.post(this.sendingUrl + apiUrl,data, {headers: this.header})
                .pipe(map((response: Response) => {
                    if(MyAppHttp.REQUEST_TYPES.ADD == requestType){
                        this.showDialog('Added Successfully');
                    }
                    else if(MyAppHttp.REQUEST_TYPES.UPDATE == requestType){
                        this.showDialog('Updated Successfully');
                    }
                    else if(MyAppHttp.REQUEST_TYPES.DELETE == requestType){
                        this.showDialog('Deleted Successfully');
                    }
                    else if(MyAppHttp.REQUEST_TYPES.CANCEL == requestType){
                        this.showDialog('Cancel Order Successfully');
                    }else if(MyAppHttp.Conerted_REQUEST_TYPES.MemToNonMem == requestType ){
                        this.showDialog('Converted Member to Non Member');
                         }
                         else if(MyAppHttp.Conerted_REQUEST_TYPES.MemToFreeMem == requestType ){
                            this.showDialog('Converted Member to Free Member');
                             }
                             else if(MyAppHttp.Conerted_REQUEST_TYPES.NonMemToMem == requestType ){
                                this.showDialog('Converted Non Member to Member');
                                 }
                                 else if(MyAppHttp.Conerted_REQUEST_TYPES.NonMemToFreeMem == requestType ){
                                    this.showDialog('Converted Non Member to Free Member');
                                     }
                                     else if(MyAppHttp.Conerted_REQUEST_TYPES.FreeMemToMem == requestType ){
                                        this.showDialog('Converted Free Member to Member');
                                         }
                                         else if(MyAppHttp.Conerted_REQUEST_TYPES.FreeMemToNonMem == requestType ){
                                            this.showDialog('Converted Free Member to Non Member');
                                             }
                                             else if(MyAppHttp.REQUEST_TYPE_Reset.RESETPASSWORD == requestType ){
                                                this.showDialog('Changed Password Sucessfully');
                                                 }

                   return response;
                }))
            }
            else if(method == this.httpVerb.Put){
                return new Observable;
            }
            else if(method == this.httpVerb.Delete){
                return new Observable;
            }
            else{}
    

        } catch (e) {
        }
    }

    public loginService(messageToSend): Observable<any> {
                try {
                    const header = new HttpHeaders({
                        'Content-Type': 'application/x-www-form-urlencoded'
                      });
                   let data = "grant_type=password&client_id=restapp&client_secret=restapp&username=" + messageToSend.adminEmail + "&password=" + messageToSend.adminPassword + "&ClientType="+MyAppHttp.Admin_ClientType+"&scope=read,write,trust";
                    return this._http.post<any>(environment.url+"api/authtoken", data,{headers:header})
                        .pipe(map(data => {
                            if (data && data.access_token) {
                                localStorage.setItem('accessToken', JSON.stringify(data));

                                var userRoleId =   JSON.parse(localStorage.getItem("accessToken")).roleId;
                                this.globalRoleId = parseInt(userRoleId);
                                

                                var userId =   JSON.parse(localStorage.getItem("accessToken")).id;
                                this.globalUserId = parseInt(userId);

                                
                                var deptId =   JSON.parse(localStorage.getItem("accessToken")).deptId;
                                this.globalDeptId = parseInt(deptId);
                                
                                var extention =  JSON.parse(localStorage.getItem("accessToken")).extentionNumber;
                                this.globalExtentionNumber = extention; 

                                      
                                var requestData = {
                                    EmailId: messageToSend.adminEmail,
                                    Password: messageToSend.adminPassword
                                }

                                this.send(this.httpVerb.Post, "api/Login", requestData).subscribe((userdata)=> {

                               
                                    localStorage.setItem('userData', JSON.stringify(userdata));
                                    if(userdata.IsPasswordChanged){
                                        this.router.navigate(['change_password']);
                                    }else{
                                      this.router.navigate(['dashboard']);
                                    }
                                    this.findMenusAndSubMenus(userdata.RolePermissions);
                                  },
                                error => {
                                    
                                });
                            }
                            return data;
                        }));
            
        } catch (e) {
            Utils.logMessage("Error", e);
        }
    }

    findMenusAndSubMenus(data){
        let Menus:any= []; 
        let SubMenus:any= [];
        for(let Menu of data){
            if (Menu.SubMenuId != null) {
                SubMenus.push(Menu);
                if(Menus.filter(function (e, index) {
                    return (e.MenuId == Menu.MenuId);
                 }).length == 0){
                    let tempObj = Object.assign({}, Menu)
                    tempObj.subMenus = [];
                    Menus.push(tempObj);
                 }
            }
            else {
                Menu.subMenus = [];
                Menus.push(Menu)
            }
        }

        for(let sm of SubMenus){
            Menus.filter(function (m, index) {
                if (sm.MenuId == m.MenuId) {

                    if(m.subMenus.filter(function (e, index) {
                        return (e.SubMenuId == m.SubMenuId);
                     }).length >= 0){
                        m.subMenus.push(sm);
                     }
                }
            });
        }

        this.globalMenuList.next({
            Menus
          });

        localStorage.setItem('userMenuList', JSON.stringify(Menus));
        
    }

    public logoutService(){
       
        localStorage.removeItem('accessToken');
        localStorage.removeItem('userMenuList');
        localStorage.removeItem('userData');

        this.globalUserId =0;
        this.globalRoleId = 0;
        this.globalDeptId =0;
        document.location.href = '/sessions/signin';
         
    }

   
    getSequence(): number {
        return this._sequence;
    }
    updateSequence(): void {
        this._sequence = this._sequence + 1;
    }
    setSequence(input: number): void {
        this._sequence = input;
    }

    errorHandler(error: Response) {
        return Observable.throw(error);
    }

    getKeyValueName(Id)
    {
        return this.send(this.httpVerb.Get,'api/Common/GetTableKeyNameByKeyTypeAndId?type='+this.router.url.split('/')[1]+'&id='+Id+'',{})
    }

    
    showDialog(message){
        let dialogInstance = this.dialog.open(
            InfoDialogComponent, {
            data: {
              message: message  
            }
          });
    
          setTimeout(() => {
            dialogInstance.close();
          }, 2000);
    }
}