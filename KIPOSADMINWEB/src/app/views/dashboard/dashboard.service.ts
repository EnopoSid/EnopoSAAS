import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class DashboardService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    
    getDelieveryPickupTotalOrders(productId,year,month){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, 'api/Dashboard/GetDelieveryPickupTotalOrders?flag=3&&prodId='+productId+'&&year='+year+'&&month='+month,{})
    }
    getOrderscount(month,year){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Dashboard/GetTotalOrders?month='+month+'&&year='+year,{})
    }
    getTotalOrdersForPieChart()
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, 'api/Dashboard/getPiechartOrders',{})
    }
    sendMonthandyear(month:number,year:number)
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Dashboard/getMonthandYearRecords?month='+month+'&&year='+year,{})
    }
    getTotalSalesOrders(flag,fDate,tDate){
           return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Dashboard/getbasedonDatesRecords?flag='+flag+'&fdate='+fDate+'&tdate='+tDate,{});
           
    }
    getPosSalesOrders(flag,fDate,tDate){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Dashboard/getPOSPickUpReports?flag='+flag+'&fdate='+fDate+'&tdate='+tDate,{});
        
 }

    getOnlineSalesOrdersByShipping(flag,fDate,tDate){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Dashboard/getOnlineDeliveryPickUpReports?flag='+flag+'&fdate='+fDate+'&tdate='+tDate,{});
    }
    
  
}