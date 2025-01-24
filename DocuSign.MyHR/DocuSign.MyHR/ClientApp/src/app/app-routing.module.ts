import { NgModule } from '@angular/core'
import { Routes, RouterModule } from '@angular/router'
import { AuthGuard } from './core/authentication/auth.guard'

const routes: Routes = [
    {
        path: '',
        loadChildren: () => import('./home/home.module').then((m) => m.HomeModule),
        canLoad: [AuthGuard],
        canActivate: [AuthGuard]
    },
    {
        path: 'employee',
        loadChildren: () => import('./employee/employee.module').then((m) => m.EmployeeModule),
        canLoad: [AuthGuard],
        canActivate: [AuthGuard]
    },
    {
        path: '**',
        redirectTo: ''
    }
]

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {}
