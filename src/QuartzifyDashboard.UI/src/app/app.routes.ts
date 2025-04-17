import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth-guard.guard';
export const routes: Routes = [
    {
        path: 'auth',
        loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
    },
    {
        path: '',
        canActivate: [AuthGuard],
        loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule)
    },
    {
        path: 'jobs',
        canActivate: [AuthGuard],
        loadChildren: () => import('./jobs/jobs.module').then(m => m.JobsModule)
    },
    {
        path: 'triggers',
        canActivate: [AuthGuard],
        loadChildren: () => import('./triggers/triggers.module').then(m => m.TriggersModule)
    },
    {
        path: 'scheduler',
        canActivate: [AuthGuard],
        loadChildren: () => import('./scheduler/scheduler.module').then(m => m.SchedulerModule)
    },
    { path: '**', redirectTo: '' }
];