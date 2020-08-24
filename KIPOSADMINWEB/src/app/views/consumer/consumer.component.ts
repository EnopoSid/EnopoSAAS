import {Component, OnInit} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';
import { ConsumerService } from './consumer.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as $ from 'jquery';
import { NgxSpinnerService } from 'ngx-spinner';


@Component({
    selector: 'consumer-table',
    templateUrl: './consumer.component.html',
     styleUrls: ['./consumer.component.css']
})
export class ConsumerComponent implements OnInit {
    complaintNum: string;
    constructor( public service: ConsumerService,
        private _router: Router, private formBuilder: FormBuilder ,
        private spinner: NgxSpinnerService,) {
    }

    ConsumerForm : FormGroup;
    isButtonClicked: boolean;
    complaintValidate:boolean=false;
    errorFlag:boolean=false;


ngOnInit(){
    this.ConsumerForm = this.formBuilder.group({
         'complaintStatus': [null, Validators.required],
     });
}

GetComplaintNum() { 
    if(this.errorFlag)
    {
        return;
    }
    this.isButtonClicked = true;
    if(!this.ConsumerForm.valid){
        $('input.ng-invalid').first().focus();
        setTimeout(() => {
            this.isButtonClicked = true;
        }, 100);
        return false;
    }
    if(this.complaintNum != undefined)
    {
        this.service.validateComplaintNumber(this.complaintNum).subscribe((resp)=>{
            this.complaintValidate=resp;
            if(this.complaintValidate){
                this.service.ComplaintNum =  this.complaintNum;
                this._router.navigate(['/consumer_complaint/checkstatus']);
            }
            else{
                this.errorFlag=true;
           } 
        }) 
    }
    }
onComplaintNumberBlur(){
    this.isButtonClicked = false;

}

validateComplaintNum(){
var  regexp = new RegExp(/^[Cc0-9]*$/),
test = regexp.test(this.complaintNum),
onlynumber = /^\d+$/.test(this.complaintNum);
   if (test == false || onlynumber == true) {
       this.errorFlag = true;
   }
   else {
       this.errorFlag = false;
   }
}

}
