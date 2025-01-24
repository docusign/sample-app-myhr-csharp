import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core'
import { ActionsService } from '../shared/actions.service'
import { DocumentType } from '../shared/document-type.enum'
import { Router, ActivatedRoute, Params } from '@angular/router'

import { filter, map, switchMap } from 'rxjs/operators'
import { of, Observable } from 'rxjs'
import { NotificationService } from 'src/app/shared/notification/notification.service'
import { IMessage } from '../shared/message.model'
import { popSavedDataFromStorage } from '../shared/storage-utils'

@Component({
    selector: 'app-employee-actions',
    templateUrl: './employee-actions.component.html'
})
export class EmployeeActionsComponent implements OnInit {
    public documentType = DocumentType
    actionExecuted: boolean
    directDepositPayload
    messageBody

    @ViewChild('directDepositTemplate', { static: true }) directDepositTemplate: TemplateRef<unknown>

    constructor(
        private actionServise: ActionsService,
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private notificationService: NotificationService
    ) {}

    ngOnInit(): void {
        this.activatedRoute.queryParams
            .pipe(
                map((params: Params) => params.event),
                filter((event: string) => !!event && event === 'signing_complete'),
                switchMap(() => this.getNotificationMessage())
            )
            .subscribe((message: IMessage) => this.notificationService.showNotificationMessage(message))
    }

    sendEnvelope(type: DocumentType): void {
        this.actionExecuted = true
        this.actionServise.sendEnvelope(type, null).subscribe((payload) => {
            sessionStorage.setItem('envelopeId', payload.envelopeId)
            sessionStorage.setItem('documentType', type)
            window.location.href = payload.redirectUrl
        })
    }

    sendTimeCard(): void {
        this.actionExecuted = true
        this.router.navigate(['/employee/timecard'])
    }

    private getNotificationMessage(): Observable<IMessage> {
        const documentType = popSavedDataFromStorage('documentType')
        const header = `Notifications.SuccessMessageHeader.${documentType || 'Timecard'}`

        if (documentType === DocumentType.DirectDeposit) {
            const envelopeId = popSavedDataFromStorage('envelopeId')
            return this.actionServise.getEnvelopeInfo(envelopeId).pipe(
                map((payload) => {
                    this.directDepositPayload = payload
                    return { header, body: this.directDepositTemplate }
                })
            )
        }
        return of({ header, body: `Notifications.SuccessMessageBody.${documentType || 'Timecard'}` })
    }
}
