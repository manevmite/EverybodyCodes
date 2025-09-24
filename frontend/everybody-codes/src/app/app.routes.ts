import { Routes } from '@angular/router';
import { CamerasComponent } from './components/cameras/cameras';

export const routes: Routes = [
    { path: '', component: CamerasComponent },
    { path: 'cameras', component: CamerasComponent }
];
