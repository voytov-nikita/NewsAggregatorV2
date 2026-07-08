import { NgModule } from '@angular/core';
import SharedModule from '../shared/shared.module';
import { TrendingRoutingModule } from './trending-routing.module';
import { Trending } from './components/trending/trending';

@NgModule({
  declarations: [Trending],
  imports: [SharedModule, TrendingRoutingModule],
})
export default class TrendingModule {}
