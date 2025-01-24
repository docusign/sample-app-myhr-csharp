import { of } from 'rxjs'
import { async, ComponentFixture, TestBed } from '@angular/core/testing'
import { RouterTestingModule } from '@angular/router/testing'
import { EmployeeActionsComponent } from './employee-actions.component'
import { ActionsService } from '../shared/actions.service'
import { TranslateModule } from '@ngx-translate/core'
import { NotificationService } from 'src/app/shared/notification/notification.service'

class ActionsServiceStub {
    public getUser() {
        return of(null)
    }
}

class NotificationServiceStub {
    public showNotificationMessage() {}
}

describe('ActionsComponent', () => {
    let component: EmployeeActionsComponent
    let fixture: ComponentFixture<EmployeeActionsComponent>
    let notificationService: NotificationService

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            imports: [RouterTestingModule, TranslateModule.forRoot()],
            declarations: [EmployeeActionsComponent],
            providers: [
                { provide: ActionsService, useClass: ActionsServiceStub },
                { provide: NotificationService, useClass: NotificationServiceStub }
            ]
        }).compileComponents()
    }))

    beforeEach(() => {
        fixture = TestBed.createComponent(EmployeeActionsComponent)
        component = fixture.componentInstance
        notificationService = TestBed.inject(NotificationService)
        spyOn(notificationService, 'showNotificationMessage').and.stub()
        fixture.detectChanges()
    })

    it('should create', () => {
        expect(component).toBeTruthy()
    })
})
