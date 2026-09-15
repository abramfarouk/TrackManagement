import { inject, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Router } from '@angular/router';
import { TrackListComponent } from './track-list/track-list.component';
import { TrackDetailComponent } from './track-detail/track-detail.component';
import { TrackFormComponent } from './track-form/track-form.component';
import { LoginComponent } from './login/login.component';
import { ArtistListComponent } from './artist-list/artist-list.component';
import { RegisterComponent } from './register/register.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { AuthService } from './services/auth.service';

const routes: Routes = [
  { path: '', redirectTo: 'tracks', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'tracks', component: TrackListComponent },
  { path: 'artists', component: ArtistListComponent },
  { path: 'tracks/new', component: TrackFormComponent },
  { path: 'tracks/:id', component: TrackDetailComponent },
  { path: 'register', component: RegisterComponent, canActivate: [() => {
    const authService = inject(AuthService);
    const router = inject(Router);
    return authService.isAdmin() ? true : router.parseUrl('/login');
  }] },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
