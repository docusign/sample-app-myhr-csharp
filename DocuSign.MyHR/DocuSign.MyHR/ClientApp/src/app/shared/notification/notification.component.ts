import { Router } from '@angular/router'
import { Component, OnInit, Injectable } from '@angular/core'
import { Observable } from 'rxjs'
import { tap } from 'rxjs/operators'
import { NotificationService } from './notification.service'
import { IMessage } from './../../employee/shared/message.model'

@Component({
    selector: 'app-notification',
    templateUrl: './notification.component.html'
})
@Injectable()
export class NotificationComponent implements OnInit {
    message$: Observable<IMessage>
    isClosed = true
    withTemplate: boolean

    constructor(private notificationService: NotificationService, private router: Router) {}

    ngOnInit(): void {
        this.message$ = this.notificationService.message$.pipe(
            tap((message) => {
                this.withTemplate = !(typeof message?.body === 'string')
                this.isClosed = !message
            })
        )
    }

    close(): void {
        this.notificationService.clear()
        this.router.navigate(['./'], { queryParams: null })
    }
}
