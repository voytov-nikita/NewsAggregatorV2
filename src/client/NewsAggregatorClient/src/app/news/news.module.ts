import { NgModule } from '@angular/core';
import { NewsRoutingModule } from './news-routing.module';
import { NewsList } from './components/list/list';
import { FilterRail } from './components/filter-rail/filter-rail';
import SharedModule from '../shared/shared.module';

@NgModule({
  declarations: [NewsList, FilterRail],
  imports: [SharedModule, NewsRoutingModule],
})
export default class NewsModule {}
