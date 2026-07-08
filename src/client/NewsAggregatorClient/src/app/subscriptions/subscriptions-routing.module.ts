import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Subscriptions } from './components/subscriptions/subscriptions';

const routes: Routes = [
  { path: '', component: Subscriptions, data: { pageTitle: 'Subscriptions' } },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SubscriptionsRoutingModule {}
