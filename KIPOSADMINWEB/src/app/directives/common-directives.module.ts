import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { filterDirective } from 'src/app/directives/common/filter.directive';
import { restrictInput } from 'src/app/directives/common/restrictInputPattern.directive';
import { LetterAvatarDirective } from 'src/app/directives/common/letter-avatar.directive';



@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        LetterAvatarDirective,
        restrictInput,
        filterDirective
    ],
    exports: [
         LetterAvatarDirective,
         restrictInput,
        filterDirective
    ]
})
export class CommonDirectivesModule { }
