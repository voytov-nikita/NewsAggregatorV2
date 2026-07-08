import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Trending } from './components/trending/trending';

const routes: Routes = [{ path: '', component: Trending, data: { pageTitle: 'Trending' } }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TrendingRoutingModule {}
