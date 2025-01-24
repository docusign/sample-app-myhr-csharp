import { AuthenticationService } from './../core/authentication/auth.service'
import { async, ComponentFixture, TestBed } from '@angular/core/testing'
import { HomeComponent } from './home.component'
import { AuthType } from '../core/authentication/auth-type.enum'

class AuthenticationServiceStub {
    public saveAuthType() {}
}

describe('HomeComponent', () => {
    let component: HomeComponent
    let authenticationService: AuthenticationService
    let fixture: ComponentFixture<HomeComponent>

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [HomeComponent],
            providers: [
                {
                    provide: AuthenticationService,
                    useClass: AuthenticationServiceStub
                }
            ]
        }).compileComponents()
    }))

    beforeEach(() => {
        fixture = TestBed.createComponent(HomeComponent)
        component = fixture.componentInstance
        authenticationService = TestBed.inject(AuthenticationService)
        spyOn(authenticationService, 'saveAuthType').and.stub()
        fixture.detectChanges()
    })

    it('should create', () => {
        expect(component).toBeTruthy()
    })

    describe('login', () => {
        it('You should call the saveAuthType method from the authentication service with the appropriate parameter', () => {
            // arrange
            const authType = AuthType.CodeGrant
            // act
            component.login(authType)
            // assert
            expect(authenticationService.saveAuthType).toHaveBeenCalledWith(authType)
        })
    })
})
