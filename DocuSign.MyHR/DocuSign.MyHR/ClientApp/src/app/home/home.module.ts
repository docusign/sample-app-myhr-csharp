import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { TranslateModule } from '@ngx-translate/core'
import { InfoComponent } from './info/info.component'
import { HomeComponent } from './home.component'
import { HomeRoutingModule } from './home-routing.module'

@NgModule({
    declarations: [HomeComponent, InfoComponent],
    imports: [CommonModule, HomeRoutingModule, TranslateModule.forChild()]
})
export class HomeModule {}
