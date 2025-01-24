import { async, ComponentFixture, TestBed } from '@angular/core/testing'

import { HeaderComponent } from './header.component'
import { AuthenticationService } from '../authentication/auth.service'
import { RouterTestingModule } from '@angular/router/testing'

class AuthenticationServiceStub {
    public logout() {}
    public isAuthenticated() {}
}

describe('HeaderComponent', () => {
    let component: HeaderComponent
    let fixture: ComponentFixture<HeaderComponent>
    let authenticationService: AuthenticationService

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            imports: [RouterTestingModule],
            declarations: [HeaderComponent],
            providers: [
                {
                    provide: AuthenticationService,
                    useClass: AuthenticationServiceStub
                }
            ]
        }).compileComponents()
    }))

    beforeEach(() => {
        fixture = TestBed.createComponent(HeaderComponent)
        component = fixture.componentInstance
        authenticationService = TestBed.inject(AuthenticationService)
        spyOn(authenticationService, 'logout').and.stub()
        spyOn(authenticationService, 'isAuthenticated').and.stub()
        fixture.detectChanges()
    })

    it('should create', () => {
        expect(component).toBeTruthy()
    })

    describe('logout', () => {
        it('should call logout method from authentication service', () => {
            // act
            component.logout()
            // assert
            expect(authenticationService.logout).toHaveBeenCalled()
        })
    })
})
