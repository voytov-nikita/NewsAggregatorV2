import { NgModule } from '@angular/core';
import { NewsRoutingModule } from './news-routing.module';
import { NewsList } from './components/list/list';
import { NewsDetail } from './components/detail/detail';
import SharedModule from '../shared/shared.module';

@NgModule({
  declarations: [NewsList, NewsDetail],
  imports: [SharedModule, NewsRoutingModule],
})
export default class NewsModule {}
