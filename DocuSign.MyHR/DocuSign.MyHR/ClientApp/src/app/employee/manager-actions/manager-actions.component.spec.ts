import { async, ComponentFixture, TestBed } from '@angular/core/testing'
import { ManagerActionsComponent } from './manager-actions.component'
import { ActionsService } from '../shared/actions.service'
import { FormBuilder } from '@angular/forms'
import { DocumentType } from '../shared/document-type.enum'
import { of } from 'rxjs'
import { TranslateModule } from '@ngx-translate/core'

class ActionsServiceStub {
    sendEnvelope() {
        return of(null)
    }
}

describe('ManagerActionsComponent', () => {
    let component: ManagerActionsComponent
    let fixture: ComponentFixture<ManagerActionsComponent>

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ManagerActionsComponent],
            providers: [{ provide: ActionsService, useClass: ActionsServiceStub }, FormBuilder],
            imports: [TranslateModule.forRoot()]
        }).compileComponents()
    }))

    beforeEach(() => {
        fixture = TestBed.createComponent(ManagerActionsComponent)
        component = fixture.componentInstance
        fixture.detectChanges()
    })

    it('should create', () => {
        expect(component).toBeTruthy()
    })

    describe('setDocumentType', () => {
        it('should set type correctly', () => {
            // arrange
            const type = DocumentType.I9
            // act
            component.setDocumentType(type)
            // assert
            expect(component.type).toEqual(type)
        })
    })
})
