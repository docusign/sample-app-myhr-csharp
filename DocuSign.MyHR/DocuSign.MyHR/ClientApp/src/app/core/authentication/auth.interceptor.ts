import { Injectable } from '@angular/core'
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http'
import { Observable, throwError } from 'rxjs'
import { catchError } from 'rxjs/operators'
import { Router } from '@angular/router'
import { AuthenticationService } from './auth.service'

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private router: Router, private authenticationService: AuthenticationService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(
            catchError((error: any) => {
                if (error instanceof HttpErrorResponse) {
                    const authType = this.authenticationService.getAuthType()
                    if (error.status !== 401) {
                        return throwError(error)
                    } else if (error.status === 401 && !!authType) {
                        window.location.href = `Account/Login?authType=${authType}`
                    } else if (error.status === 401 && !authType) {
                        window.location.href = 'Account/Logout'
                    }
                }
                return throwError(error)
            })
        )
    }
}
