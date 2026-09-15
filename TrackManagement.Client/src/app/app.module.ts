import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TrackListComponent } from './track-list/track-list.component';
import { TrackDetailComponent } from './track-detail/track-detail.component';
import { TrackFormComponent } from './track-form/track-form.component';
import { LoginComponent } from './login/login.component';
import { ArtistListComponent } from './artist-list/artist-list.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { AuthService } from './services/auth.service';
import { RegisterComponent } from './register/register.component';
import { NotFoundComponent } from './not-found/not-found.component';

export function restoreAuthSession(authService: AuthService): () => Promise<void> {
  return () => authService.restoreSession();
}

@NgModule({
  declarations: [
    AppComponent,
    TrackListComponent,
    TrackDetailComponent,
    TrackFormComponent,
    LoginComponent,
    ArtistListComponent,
    RegisterComponent,
    NotFoundComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    RouterModule,
    HttpClientModule,
    AppRoutingModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    {
      provide: APP_INITIALIZER,
      useFactory: restoreAuthSession,
      deps: [AuthService],
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
