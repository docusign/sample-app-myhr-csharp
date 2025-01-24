import { AuthType } from './auth-type.enum'
import { Injectable, Inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs'

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    public authType: AuthType

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {}

    isAuthenticated(): Observable<boolean> {
        return this.http.get<boolean>(this.baseUrl + 'api/isauthenticated')
    }

    saveAuthType(authType: AuthType): void {
        sessionStorage.setItem('authType', authType)
    }

    getAuthType(): string {
        return sessionStorage.getItem('authType')
    }

    logout(): void {
        sessionStorage.removeItem('authType')
    }
}
