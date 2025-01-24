import { Component, Input, Output, EventEmitter } from '@angular/core'
import { IUser } from '../shared/user.model'

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html'
})
export class ProfileComponent {
    @Output() editUserClicked = new EventEmitter<void>()
    @Input() user: IUser

    editProfile(): void {
        this.editUserClicked.next()
    }

    getCityandState(city: string, state: string): string {
        return state ? `${city}, ${state}` : city
    }
}
