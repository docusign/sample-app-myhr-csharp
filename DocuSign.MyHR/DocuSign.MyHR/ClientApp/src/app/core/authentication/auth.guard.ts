import { Injectable } from '@angular/core'
import { Router, CanLoad, Route, UrlSegment, ActivatedRouteSnapshot, CanActivate } from '@angular/router'
import { AuthenticationService } from './auth.service'
import { catchError, map } from 'rxjs/operators'
import { of, Observable } from 'rxjs'

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanLoad, CanActivate {
    constructor(private authenticationService: AuthenticationService, private router: Router) {}

    canActivate(route: ActivatedRouteSnapshot): Observable<boolean> | boolean {
        return this.canNavigatePage(route.url)
    }

    canLoad(route: Route, segments: UrlSegment[]): Observable<boolean> | boolean {
        return this.canNavigatePage(segments)
    }

    canNavigatePage(urlEgments: UrlSegment[]): Observable<boolean> | boolean {
        if (urlEgments.length === 0 || urlEgments[0].path === '/') {
            return this.authenticationService.isAuthenticated().pipe(
                map((res: boolean) => {
                    if (res) {
                        this.router.navigate(['/employee'])
                        return false
                    } else {
                        return true
                    }
                }),
                catchError(() => {
                    return of(true)
                })
            )
        }
        if (urlEgments[0].path === 'employee') {
            return this.authenticationService.isAuthenticated().pipe(
                map((res: boolean) => {
                    if (!res) {
                        this.router.navigate(['/'])
                        return false
                    } else {
                        return true
                    }
                }),
                catchError(() => {
                    return of(true)
                })
            )
        }
    }
}
