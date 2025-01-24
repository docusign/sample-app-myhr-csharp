import { Component, OnInit } from '@angular/core'
import { AuthenticationService } from '../authentication/auth.service'
import { Router, NavigationEnd } from '@angular/router'
import { filter, map, startWith } from 'rxjs/operators'
import { Observable } from 'rxjs'

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html'
})
export class HeaderComponent implements OnInit {
    isHomePage$: Observable<boolean>
    isAuthenticated$: Observable<boolean>
    constructor(private authenticationService: AuthenticationService, private router: Router) {}

    ngOnInit(): void {
        this.isHomePage$ = this.router.events.pipe(
            filter((event) => event instanceof NavigationEnd),
            map((event: NavigationEnd) => event.url === '/'),
            startWith(true)
        )
        this.isAuthenticated$ = this.authenticationService.isAuthenticated()
    }

    logout(): void {
        this.authenticationService.logout()
    }
}
