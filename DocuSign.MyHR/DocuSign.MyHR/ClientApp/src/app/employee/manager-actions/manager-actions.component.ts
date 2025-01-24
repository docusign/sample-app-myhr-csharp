import { Component, ViewChild, ElementRef } from '@angular/core'
import { FormControl, Validators, FormGroup, AbstractControl } from '@angular/forms'
import { ActionsService } from '../shared/actions.service'
import { DocumentType } from '../shared/document-type.enum'
import { NotificationService } from 'src/app/shared/notification/notification.service'
import { IMessage } from '../shared/message.model'

@Component({
    selector: 'app-manager-actions',
    templateUrl: './manager-actions.component.html'
})
export class ManagerActionsComponent {
    @ViewChild('closeModalButton') closeModalButton: ElementRef
    documentType = DocumentType
    type: DocumentType
    executingAction: boolean
    message: IMessage

    additionalUserForm: FormGroup = new FormGroup({
        Name: new FormControl('', Validators.required),
        Email: new FormControl('', [Validators.required, Validators.email])
    })

    constructor(private actionServise: ActionsService, private notificationService: NotificationService) {}

    setDocumentType(type: DocumentType): void {
        this.type = type
    }

    sendEnvelope(): void {
        this.executingAction = true
        this.actionServise.sendEnvelope(this.type, this.additionalUserForm.value).subscribe((payload) => {
            if (payload.redirectUrl != null && payload.redirectUrl !== '') {
                window.location.href = payload.redirectUrl
            }
            sessionStorage.setItem('envelopeId', payload.envelopeId)
            sessionStorage.setItem('documentType', this.type)
            this.closeModalButton.nativeElement.click()
            this.resetForm()
            this.executingAction = false
            if (this.type === DocumentType.I9) {
                this.showNotification()
            }
        })
    }

    isInvalid(control: AbstractControl): boolean {
        const form = <FormGroup>control
        return form.invalid && form.touched
    }

    resetForm(): void {
        this.additionalUserForm.reset()
    }

    private showNotification(): void {
        const message: IMessage = {
            header: `Notifications.SuccessMessageHeader.${this.type}`,
            body: `Notifications.SuccessMessageBody.${this.type}`
        }
        this.notificationService.showNotificationMessage(message)
    }
}
