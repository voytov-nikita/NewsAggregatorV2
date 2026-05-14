import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizedLayout } from './layout/components/authorized-layout/authorized-layout';
import { NotFound } from './shared/components/not-found/not-found';

const routes: Routes = [
  {
    path: '',
    component: AuthorizedLayout,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'feed' },
      {
        path: '',
        loadChildren: () => import('./news/news.module').then((m) => m.default),
      },
      {
        path: 'settings',
        loadChildren: () => import('./settings/settings.module').then((m) => m.default),
      },
      {
        path: 'admin',
        loadChildren: () => import('./admin/admin.module').then((m) => m.default),
      },
      { path: '**', component: NotFound, data: { pageTitle: 'Not found' } },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
