import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { TranslateModule } from '@ngx-translate/core'
import { RouterModule } from '@angular/router'
import { NotificationComponent } from './notification/notification.component'

@NgModule({
    declarations: [NotificationComponent],
    imports: [CommonModule, RouterModule, TranslateModule.forChild()],
    exports: [NotificationComponent, CommonModule, TranslateModule]
})
export class SharedModule {}
