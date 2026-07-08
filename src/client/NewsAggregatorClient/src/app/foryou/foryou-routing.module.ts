import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ForYou } from './components/foryou/foryou';

const routes: Routes = [{ path: '', component: ForYou, data: { pageTitle: 'For you' } }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ForYouRoutingModule {}
