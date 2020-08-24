import { Routes } from '@angular/router';
import { NewsLetterMailComponent } from './news-letter-mail.component';



export const NewsLetterMailRoutes: Routes = [
    {path: '', component: NewsLetterMailComponent, data:{title: 'News-Letter'}},
];