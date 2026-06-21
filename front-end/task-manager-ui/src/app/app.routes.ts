import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './auth/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'tasks', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent),
    canActivate: [guestGuard],
  },
  {
    path: 'tasks',
    loadComponent: () => import('./tasks/task-board/task-board.component').then(m => m.TaskBoardComponent),
    canActivate: [authGuard],
  },
  { path: '**', redirectTo: 'tasks' },
];
