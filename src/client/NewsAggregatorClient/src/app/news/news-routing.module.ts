import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NewsList } from './components/list/list';

const routes: Routes = [
  {
    path: '',
    component: NewsList,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class NewsRoutingModule {}
