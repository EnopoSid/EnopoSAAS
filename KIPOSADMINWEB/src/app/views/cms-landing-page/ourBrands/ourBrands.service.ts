import { Injectable } from '@angular/core';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class OurBrandComponentService {

    isOurBrandView: boolean;
  
    constructor(private sendReceiveService: SendReceiveService) { }
    getAllBrands() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Sponsor/GetAllSponsors", {});
    }
    UserView(userView: boolean) {
        this.isOurBrandView = userView
    }
    getBrandListById(ID: number) {

        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Sponsor/SponsorById?id=" + ID, {});
    }
    saveBrandList(brand, file: FileList) {    
        let formdata: FormData = new FormData();
        if (file != null) {
            for (let i = 0; i < file.length; i++) {
                formdata.append(file[i]['name'], file[i]);
            }
        }
        formdata.append('objArr', JSON.stringify(brand))
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Sponsor/PostSponsor", formdata, MyAppHttp.REQUEST_TYPES.ADD);
    }
    editBrandList(brandText, file: FileList, ID) {      
        let branddata: FormData = new FormData();
        if (file != null) {
            for (let i = 0; i < file.length; i++) {
                branddata.append(file[i]['name'], file[i]);
            }
        }
        branddata.append('objArr', JSON.stringify(brandText))
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Sponsor/PutSponsor?id=" + ID, branddata, MyAppHttp.REQUEST_TYPES.UPDATE);
    }

    deleteBrand(ID) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Sponsor/Sponsordelete?id=" + ID, {}, MyAppHttp.REQUEST_TYPES.DELETE);
    }
    activateRecord(ID,event:boolean) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Sponsor/SponsorActiveInActiveput?id=" + ID +"&isactive="+event, {}, MyAppHttp.REQUEST_TYPES.UPDATE);
    }
}