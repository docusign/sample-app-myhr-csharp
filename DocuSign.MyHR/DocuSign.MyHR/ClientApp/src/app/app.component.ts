import { Component, OnInit } from '@angular/core'
import { TranslateService } from '@ngx-translate/core'
import { environment } from 'src/environments/environment'

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
    constructor(private translateService: TranslateService) {}
    ngOnInit(): void {
        this.translateService.use(environment.defaultLanguage)
    }
}
