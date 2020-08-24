import { Subject } from "rxjs";
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
  })
  export class MenuObservableService {
    public  menus = new Subject<any>();
  }

