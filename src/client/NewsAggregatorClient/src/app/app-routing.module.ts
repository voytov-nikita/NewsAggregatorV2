import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizedLayout } from './layout/components/authorized-layout/authorized-layout';

const routes: Routes = [
  {
    path: '',
    component: AuthorizedLayout,
    children: [
      {
        path: '',
        loadChildren: () => import('./news/news.module').then((m) => m.default),
      },
      {
        path: 'settings',
        loadChildren: () => import('./settings/settings.module').then((m) => m.default),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
