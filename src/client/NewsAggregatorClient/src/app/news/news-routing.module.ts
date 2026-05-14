import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NewsList } from './components/list/list';
import { NewsDetail } from './components/detail/detail';

const routes: Routes = [
  { path: 'feed', component: NewsList, data: { pageTitle: 'Feed' } },
  { path: 'article/:id', component: NewsDetail, data: { pageTitle: 'Discussion' } },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class NewsRoutingModule {}
