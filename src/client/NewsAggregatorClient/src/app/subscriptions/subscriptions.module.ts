import { NgModule } from '@angular/core';
import SharedModule from '../shared/shared.module';
import { SubscriptionsRoutingModule } from './subscriptions-routing.module';
import { Subscriptions } from './components/subscriptions/subscriptions';

@NgModule({
  declarations: [Subscriptions],
  imports: [SharedModule, SubscriptionsRoutingModule],
})
export default class SubscriptionsModule {}
