import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizedLayout } from './layout/components/authorized-layout/authorized-layout';
import { NotFound } from './shared/components/not-found/not-found';
import { authGuard } from '@auth/index';

const routes: Routes = [
  // Sibling of the layout, not a child: the sign-in screens render before the application frame.
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.module').then((m) => m.default),
  },
  { path: 'login', redirectTo: 'auth/login' },
  { path: 'register', redirectTo: 'auth/register' },
  {
    path: '',
    component: AuthorizedLayout,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'allnews' },
      {
        // Public: reading the site never requires an account.
        path: '',
        loadChildren: () => import('./news/news.module').then((m) => m.default),
      },
      {
        path: 'settings',
        canActivate: [authGuard],
        loadChildren: () => import('./settings/settings.module').then((m) => m.default),
      },
      {
        // The two admin screens are gated by different permissions, so those guards live on the
        // children in AdminRoutingModule; this level only requires an account.
        path: 'admin',
        canActivate: [authGuard],
        loadChildren: () => import('./admin/admin.module').then((m) => m.default),
      },
      {
        path: 'foryou',
        loadChildren: () => import('./foryou/foryou.module').then((m) => m.default),
      },
      {
        path: 'trending',
        loadChildren: () => import('./trending/trending.module').then((m) => m.default),
      },
      {
        path: 'saved',
        canActivate: [authGuard],
        loadChildren: () => import('./saved/saved.module').then((m) => m.default),
      },
      {
        path: 'subscriptions',
        canActivate: [authGuard],
        loadChildren: () => import('./subscriptions/subscriptions.module').then((m) => m.default),
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
