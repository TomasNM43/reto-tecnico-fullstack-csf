import { Routes } from '@angular/router';

import { authGuard, sinSesionGuard } from './core/guards/auth.guard';
import { Layout } from './layout/layout';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [sinSesionGuard],
    loadComponent: () => import('./features/login/login').then((m) => m.Login),
  },
  {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'kardex' },
      {
        path: 'compras',
        loadComponent: () => import('./features/compras/compras').then((m) => m.Compras),
      },
      {
        path: 'ventas',
        loadComponent: () => import('./features/ventas/ventas').then((m) => m.Ventas),
      },
      {
        path: 'kardex',
        loadComponent: () => import('./features/kardex/kardex').then((m) => m.Kardex),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
