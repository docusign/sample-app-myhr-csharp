import { async, ComponentFixture, TestBed } from '@angular/core/testing'
import { ProfileEditComponent } from './profile-edit.component'
import { EmployeeService } from '../employee.service'
import { IUser } from '../shared/user.model'
import { FormsModule, NgForm } from '@angular/forms'
import { DatePipe } from '@angular/common'
import { HttpClientTestingModule } from '@angular/common/http/testing'
import { Observable, BehaviorSubject, of } from 'rxjs'
import { By } from '@angular/platform-browser'
import { NotificationService } from '../../shared/notification/notification.service'

class EmployeeServiceStub {
    public saveUser(user: IUser) {
        return of(user)
    }

    user$: Observable<IUser> = new BehaviorSubject(null)
}

class NotificationServiceStub {
    public showNotificationMessage() {}
}

describe('ProfileEditComponent', () => {
    let form: NgForm

    let component: ProfileEditComponent
    let fixture: ComponentFixture<ProfileEditComponent>
    let employeeService: EmployeeService
    const datePipe = new DatePipe('en-US')

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ProfileEditComponent],
            imports: [FormsModule, HttpClientTestingModule],
            providers: [
                { provide: EmployeeService, useClass: EmployeeServiceStub },
                { provide: NotificationService, useClass: NotificationServiceStub }
            ]
        }).compileComponents()
    }))

    beforeEach(() => {
        fixture = TestBed.createComponent(ProfileEditComponent)
        component = fixture.componentInstance
        employeeService = TestBed.get(EmployeeService)
        spyOn(employeeService, 'saveUser').and.returnValue(of(null))
        const user = <IUser>{
            firstName: 'TestName',
            lastName: 'TestLastName',
            profileImage: 'image',
            profileId: 'id',
            hireDate: datePipe.transform('01/01/2001', 'MM/dd/yyyy'),
            address: {}
        }
        component.user = user
        employeeService.user$ = new BehaviorSubject(user)
        const formElement = fixture.debugElement.query(By.css('#profileForm'))
        form = formElement.injector.get(NgForm)
        fixture.detectChanges()
    })

    it('should create', () => {
        expect(component).toBeTruthy()
    })

    describe('saveUser', () => {
        it('should save user with correct parameters', () => {
            const updatedUser = <IUser>{
                firstName: 'FirstNameUpdated',
                lastName: 'LastNameUpdated',
                profileImage: 'image',
                profileId: 'id',
                hireDate: datePipe.transform('01/01/2001', 'MM/dd/yyyy'),
                address: {
                    city: 'City',
                    country: 'USA',
                    phone: '111',
                    postalCode: '222',
                    fax: '123',
                    stateOrProvince: 'state'
                }
            }
            // act
            component.user = <IUser>{
                firstName: 'TestName',
                lastName: 'TestLastName',
                profileImage: 'image',
                profileId: 'id',
                hireDate: datePipe.transform('01/01/2001', 'MM/dd/yyyy'),
                address: {}
            }
            component.saveUser(<IUser>{
                firstName: 'FirstNameUpdated',
                lastName: 'LastNameUpdated',
                address: {
                    city: 'City',
                    country: 'USA',
                    phone: '111',
                    postalCode: '222',
                    fax: '123',
                    stateOrProvince: 'state'
                }
            })

            // assert
            expect(employeeService.saveUser).toHaveBeenCalledWith(updatedUser)
        })

        it('should trigger saved event with correct user', () => {
            spyOn(component.saved, 'next')
            const updatedUser = <IUser>{
                firstName: 'FirstNameUpdated',
                lastName: 'LastNameUpdated',
                profileImage: 'image',
                profileId: 'id',
                hireDate: datePipe.transform('01/01/2001', 'MM/dd/yyyy'),
                address: {}
            }
            component.user = <IUser>{
                firstName: 'TestName',
                lastName: 'TestLastName',
                profileImage: 'image',
                profileId: 'id',
                hireDate: datePipe.transform('01/01/2001', 'MM/dd/yyyy'),
                address: {}
            }
            // act
            component.saveUser(<IUser>{
                firstName: 'FirstNameUpdated',
                lastName: 'LastNameUpdated',
                address: {}
            })

            // assert
            expect(component.saved.next).toHaveBeenCalled()
            expect(component.saved.next).toHaveBeenCalledWith(updatedUser)
        })
    })

    describe('cancel', () => {
        it('should cancel editing and return user to initial state', () => {
            spyOn(component.canceled, 'next')
            const initialUser = component.user
            const updatedUser = <IUser>{
                firstName: 'FirstNameUpdated',
                lastName: 'LastNameUpdated',
                profileImage: 'image',
                profileId: 'id',
                hireDate: datePipe.transform('01/01/2001', 'MM/dd/yyyy'),
                address: {
                    city: 'City',
                    country: 'USA',
                    phone: '111',
                    postalCode: '222',
                    fax: '123',
                    stateOrProvince: 'state'
                }
            }
            // act
            component.user = updatedUser
            component.cancel()

            // assert
            expect(component.user).toEqual(initialUser)
            expect(component.canceled.next).toHaveBeenCalled()
        })
    })

    describe('from 2', () => {
        it('form valid when all fields field', () => {
            component.user = <IUser>{
                firstName: 'TestName',
                lastName: 'TestLastName',
                profileImage: 'image',
                profileId: 'id',
                hireDate: datePipe.transform('01/01/2001', 'MM/dd/yyyy'),
                address: {
                    city: 'City',
                    country: 'USA',
                    phone: '111',
                    postalCode: '222',
                    fax: '123',
                    stateOrProvince: 'state',
                    address1: 'address'
                }
            }
            fixture.detectChanges()
            fixture.whenStable().then(() => {
                expect(form.valid).toBe(true, 'Form is valid')

                // act
                const saveButton = fixture.debugElement.query(By.css('#saveButton')).nativeElement
                saveButton.click()

                // assert
                expect(component.saveUser).toHaveBeenCalled()
            })
        })
        it('form is not valid when not all fields field', () => {
            component.user = <IUser>{
                firstName: '  ',
                lastName: 'TestLastName',
                profileImage: 'image',
                profileId: 'id',
                hireDate: datePipe.transform('01/01/2001', 'MM/dd/yyyy'),
                address: {
                    city: 'City',
                    country: 'USA',
                    phone: '111',
                    postalCode: '222',
                    fax: '123',
                    stateOrProvince: 'state',
                    address1: 'address'
                }
            }
            fixture.detectChanges()
            fixture.whenStable().then(() => {
                expect(form.valid).toBe(false, 'Form is not valid')

                // act
                const saveButton = fixture.debugElement.query(By.css('#saveButton')).nativeElement
                saveButton.click()

                // assert
                expect(component.saveUser).not.toHaveBeenCalled()
            })
        })
    })
})
