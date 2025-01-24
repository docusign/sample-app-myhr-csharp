import { IMessage } from '../../employee/shared/message.model'
import { Injectable } from '@angular/core'
import { BehaviorSubject } from 'rxjs'

@Injectable({ providedIn: 'root' })
export class NotificationService {
    message$ = new BehaviorSubject<IMessage>(null)

    showNotificationMessage(message: IMessage): void {
        this.message$.next(message)
    }

    clear(): void {
        this.message$.next(null)
    }
}
