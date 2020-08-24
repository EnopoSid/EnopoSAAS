import { Directive, HostListener, ElementRef, Input, Renderer2 } from '@angular/core';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Directive({
selector: '[cmsRestrictInput]',
inputs:['cmsRestrictInput'],
host:{
  '(keypress)': 'onInput($event)'
}
})
export class restrictInput {

private regexMap = {
onlyAlphabets: MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets,
onlyAlphabetsCapsOnly:MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsCapsOnly,
onlyNumbers: MyAppHttp.HTML_VALIDATION_PATTERNS.onlyNumbers,
onlyAlphabetsWithoutSpaces: MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabetsWithoutSpaces,
alphaNumerics: MyAppHttp.HTML_VALIDATION_PATTERNS.alphaNumerics,
floatNumbers: MyAppHttp.HTML_VALIDATION_PATTERNS.floatNumbers,
alphaNumericsWithoutSpaces:  MyAppHttp.HTML_VALIDATION_PATTERNS.alphaNumericsWithoutSpaces,
alphaNumericsCapsOnly: MyAppHttp.HTML_VALIDATION_PATTERNS.alphaNumericsCapsOnly,
phoneNumber:MyAppHttp.HTML_VALIDATION_PATTERNS.phoneNumber,
alphanumericWithHyphen:MyAppHttp.HTML_VALIDATION_PATTERNS.alphanumericWithHyphen,
DOMAIN:MyAppHttp.HTML_VALIDATION_PATTERNS.Domain
};

pattern: RegExp;
cmsRestrictInput:string;
constructor(public el: ElementRef, public renderer: Renderer2) { };

onInput(e) {
this.pattern = this.regexMap[this.cmsRestrictInput];
const inputChar = e.key;
if (!this.pattern.test(inputChar)) {
  e.preventDefault();
} 
}
}