import { tap, catchError, switchMap } from 'rxjs/operators'
import { Injectable, Inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { BehaviorSubject, Observable, EMPTY } from 'rxjs'
import { IUser } from './shared/user.model'

@Injectable({ providedIn: 'root' })
export class EmployeeService {
    user$ = new BehaviorSubject<IUser>(null)

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {}

    saveUser(user: IUser): Observable<IUser> {
        return this.http.put<any>(this.baseUrl + 'api/user', user).pipe(
            switchMap(() => {
                return this.getUser()
            }),
            catchError((error) => {
                console.error(error)
                return EMPTY
            })
        )
    }

    getUser(): Observable<IUser> {
        return this.http.get<any>(this.baseUrl + 'api/user').pipe(
            tap((result) => {
                this.user$.next({ ...result })
            }),
            catchError((error) => {
                console.error(error)
                return EMPTY
            })
        )
    }
}
