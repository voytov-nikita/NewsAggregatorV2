import { NgModule, inject, isDevMode, provideAppInitializer } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';
import { AuthService, authInterceptor } from '@auth/index';
import { AppRoutingModule } from './app-routing.module';
import { App } from './app';
import LayoutModule from './layout/layout.module';
import { feedFeature, FeedEffects, uiFeature, UiEffects } from './store';

@NgModule({
  declarations: [App],
  imports: [BrowserModule, AppRoutingModule, LayoutModule],
  providers: [
    provideHttpClient(withInterceptors([authInterceptor])),
    // Silent restore: a valid refresh cookie signs the user back in across a reload before the
    // first screen paints, so a guarded route does not bounce them to the login page.
    provideAppInitializer(() => inject(AuthService).restore()),
    provideAnimationsAsync(),
    provideStore({
      [uiFeature.name]: uiFeature.reducer,
      [feedFeature.name]: feedFeature.reducer,
    }),
    provideEffects([UiEffects, FeedEffects]),
    provideStoreDevtools({ maxAge: 25, logOnly: !isDevMode() }),
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: '[data-theme="dark"]',
          cssLayer: {
            name: 'primeng',
            order: 'app-styles, primeng',
          },
        },
      },
    }),
  ],
  bootstrap: [App],
})
export class AppModule {}
