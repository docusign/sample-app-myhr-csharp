import { async, ComponentFixture, TestBed } from '@angular/core/testing'
import { EmployeeComponent } from './employee.component'
import { EmployeeService } from './employee.service'
import { IUser } from './shared/user.model'
import { Subject, of } from 'rxjs'
import { DatePipe } from '@angular/common'
import { TranslateModule } from '@ngx-translate/core'
import { RouterTestingModule } from '@angular/router/testing'

class EmployeeServiceStub {
    public getUser() {}
    public user$: Subject<string> = new Subject<string>()
}

describe('EmployeeComponent', () => {
    let component: EmployeeComponent
    let fixture: ComponentFixture<EmployeeComponent>
    let employeeService: EmployeeService
    const datePipe = new DatePipe('en-US')

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [EmployeeComponent],
            providers: [{ provide: EmployeeService, useClass: EmployeeServiceStub }],
            imports: [TranslateModule.forRoot(), RouterTestingModule]
        }).compileComponents()
    }))

    beforeEach(() => {
        fixture = TestBed.createComponent(EmployeeComponent)
        component = fixture.componentInstance
        component.user = <IUser>{
            firstName: 'TestName',
            lastName: 'TestLastName',
            profileImage: 'image',
            profileId: 'id',
            hireDate: datePipe.transform('01/01/2001', 'MM/dd/yyyy'),
            address: {}
        }
        employeeService = TestBed.inject(EmployeeService)
        spyOn(employeeService, 'getUser').and.returnValue(of(null))

        fixture.detectChanges()
    })

    it('should create', () => {
        expect(component).toBeTruthy()
    })
})
