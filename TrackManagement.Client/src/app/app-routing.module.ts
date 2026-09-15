import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TrackListComponent } from './track-list/track-list.component';
import { TrackDetailComponent } from './track-detail/track-detail.component';
import { TrackFormComponent } from './track-form/track-form.component';
import { LoginComponent } from './login/login.component';
import { ArtistListComponent } from './artist-list/artist-list.component';

const routes: Routes = [
  { path: '', redirectTo: 'tracks', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'tracks', component: TrackListComponent },
  { path: 'artists', component: ArtistListComponent },
  { path: 'tracks/new', component: TrackFormComponent },
  { path: 'tracks/:id', component: TrackDetailComponent },
  { path: '**', redirectTo: 'tracks' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
