import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NewsList } from './components/list/list';

const routes: Routes = [
  { path: 'allnews', component: NewsList, data: { pageTitle: 'All news' } },
  { path: 'feed', redirectTo: 'allnews', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class NewsRoutingModule {}
