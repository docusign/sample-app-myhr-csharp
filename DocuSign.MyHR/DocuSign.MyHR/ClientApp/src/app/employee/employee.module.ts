import { SharedModule } from './../shared/shared.module'
import { NgModule } from '@angular/core'
import { EmployeeComponent } from './employee.component'
import { EmployeeRoutingModule } from './employee-routing.module'
import { ProfileComponent } from './profile/profile.component'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { ManagerActionsComponent } from './manager-actions/manager-actions.component'
import { ProfileEditComponent } from './profile-edit/profile-edit.component'
import { TimeCardComponent } from './timecard/timecard.component'
import { EmployeeActionsComponent } from './employee-actions/employee-actions.component'

@NgModule({
    declarations: [
        EmployeeComponent,
        ProfileComponent,
        EmployeeActionsComponent,
        ManagerActionsComponent,
        ProfileEditComponent,
        TimeCardComponent
    ],
    imports: [EmployeeRoutingModule, FormsModule, ReactiveFormsModule, SharedModule]
})
export class EmployeeModule {}
