import { EmployeeService } from './employee.service'
import { Component, OnInit } from '@angular/core'
import { IUser } from './shared/user.model'

@Component({
    selector: 'app-employee',
    templateUrl: './employee.component.html'
})
export class EmployeeComponent implements OnInit {
    isEditUser = false
    user: IUser

    constructor(private employeeService: EmployeeService) {}

    ngOnInit(): void {
        this.employeeService.getUser().subscribe()
        this.employeeService.user$.subscribe((user) => (this.user = user))
    }

    editUser(): void {
        this.isEditUser = true
    }

    cancelEdit(): void {
        this.isEditUser = false
    }

    exitSaving(user: IUser): void {
        this.user = user
        this.isEditUser = false
    }
}
