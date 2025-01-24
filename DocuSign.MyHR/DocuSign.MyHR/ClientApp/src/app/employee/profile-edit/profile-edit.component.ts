import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core'
import { IUser } from '../shared/user.model'
import * as i18nIsoCountries from 'i18n-iso-countries'
import { EmployeeService } from '../employee.service'
import { IMessage } from '../shared/message.model'
import { NotificationService } from '../../shared/notification/notification.service'

@Component({
    selector: 'app-profile-edit',
    templateUrl: './profile-edit.component.html'
})
export class ProfileEditComponent implements OnInit {
    @Input() user: IUser
    @Output() canceled = new EventEmitter<void>()
    @Output() saved = new EventEmitter<IUser>()
    countries = [] as Array<any>

    constructor(private employeeService: EmployeeService, private notificationService: NotificationService) {}

    ngOnInit(): void {
        i18nIsoCountries.registerLocale(
            // eslint-disable-next-line @typescript-eslint/no-var-requires
            require('i18n-iso-countries/langs/en.json')
        )

        const countriesDictionary = i18nIsoCountries.getNames('en')
        const usIsoCode = 'US'
        countriesDictionary[usIsoCode] = 'United States'
        this.countries = Object.entries(countriesDictionary).map(([key, value]) => ({ key, value }))
        console.log(this.countries)
    }

    saveUser(user: IUser): void {
        const message: IMessage = {
            header: `Profile.Edit.SuccessMessage.Header`,
            body: `Profile.Edit.SuccessMessage.Message`
        }
        this.employeeService.saveUser(user).subscribe(() => this.notificationService.showNotificationMessage(message))
        this.employeeService.user$.subscribe((user) => {
            this.user = user
        })
        user.profileImage = this.user.profileImage
        user.profileId = this.user.profileId
        user.hireDate = this.user.hireDate
        this.saved.next(user)
    }

    cancel(): void {
        this.user = { ...this.employeeService.user$.getValue() }
        this.canceled.next()
    }
}
