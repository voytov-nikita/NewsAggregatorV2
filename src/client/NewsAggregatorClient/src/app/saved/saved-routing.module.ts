import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Saved } from './components/saved/saved';

const routes: Routes = [{ path: '', component: Saved, data: { pageTitle: 'Saved' } }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SavedRoutingModule {}
