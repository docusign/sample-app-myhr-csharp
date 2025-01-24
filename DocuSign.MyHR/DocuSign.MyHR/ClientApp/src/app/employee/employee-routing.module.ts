import { NgModule } from '@angular/core'
import { Routes, RouterModule } from '@angular/router'
import { EmployeeComponent } from './employee.component'
import { TimeCardComponent } from './timecard/timecard.component'

const routes: Routes = [
    { path: '', component: EmployeeComponent },
    {
        path: 'timecard',
        component: TimeCardComponent
    }
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class EmployeeRoutingModule {}
