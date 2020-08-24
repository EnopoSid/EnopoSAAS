import { Injectable } from '@angular/core';


import { Router } from '@angular/router';

import { Http, Response, Headers, RequestOptions } from '@angular/http';

// Import RxJs required methods
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

@Injectable()
export class SignInService {

    constructor(
    ) { }

    HMACKey: string;

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    initialize(): void {
        try {
        }
        catch (e) {

        }
    }
}
