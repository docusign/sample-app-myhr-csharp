import { Component, OnInit, Renderer2, Inject, HostListener } from '@angular/core'
import { ActionsService } from '../shared/actions.service'
import { FormControl, Validators, FormGroup } from '@angular/forms'
import { DOCUMENT } from '@angular/common'

import { getWeek, startOfWeek, endOfWeek, format } from 'date-fns'
import { EmployeeService } from '../employee.service'
import { IUser } from '../shared/user.model'

import { Router } from '@angular/router'

declare const window: Window & { docuSignClick: any }

@Component({
    selector: 'app-timecard',
    templateUrl: './timecard.component.html'
})
export class TimeCardComponent implements OnInit {
    period: Date
    total = 0
    workWeekDates: string
    user: IUser
    timecartSent: boolean
    private readonly scriptUrl = '/clickapi/sdk/latest/docusign-click.js'

    workLogs: number[] = []
    weekDays: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday']

    timecardForm: FormGroup = new FormGroup({
        Monday: new FormControl('', Validators.required),
        Tuesday: new FormControl('', Validators.required),
        Wednesday: new FormControl('', Validators.required),
        Thursday: new FormControl('', Validators.required),
        Friday: new FormControl('', Validators.required),
        Saturday: new FormControl(''),
        Sunday: new FormControl('')
    })

    constructor(
        private actionServise: ActionsService,
        private renderer2: Renderer2,
        @Inject(DOCUMENT) private document: Document,
        private employeeService: EmployeeService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.employeeService.getUser().subscribe()
        this.employeeService.user$.subscribe((user) => (this.user = user))
        this.workWeekDates = this.getWorkWeek()
    }

    @HostListener('window:message', ['$event'])
    onMessage(event: MessageEvent): void {
        if (event.data.type === 'CLOSE_HAS_AGREED') {
            this.router.navigate(['/employee'], { queryParams: { event: 'signing_complete' } })
        }
    }

    updateWorkLogs(): void {
        this.workLogs = this.weekDays.map((day) => +this.timecardForm.controls[day].value)
        this.total = this.workLogs.reduce((total: number, workLog: number) => total + workLog, 0)
    }

    sendTimeCard(): void {
        this.timecartSent = true
        this.actionServise.createClickWrap(this.workLogs).subscribe((payload) => {
            const clickwrap = payload.clickWrap
            const baseUrl = payload.docuSignBaseUrl
            this.loadClickWrap(clickwrap, baseUrl)
        })
    }

    private showClickWrap(clickwrap: any, baseUrl: string) {
        window.docuSignClick.Clickwrap.render(
            {
                environment: baseUrl,
                accountId: clickwrap.accountId,
                clickwrapId: clickwrap.clickwrapId,
                clientUserId: clickwrap.accountId
            },
            '#ds-clickwrap'
        )
    }

    private getWorkWeek(): string {
        const currentDay = new Date()
        const currentWeek = getWeek(currentDay)
        const firstDay = format(
            startOfWeek(currentDay, {
                weekStartsOn: 1
            }),
            'MMM dd'
        )
        const lastDay = format(
            endOfWeek(currentDay, {
                weekStartsOn: 1
            }),
            'MMM dd'
        )
        return `W${currentWeek} (${firstDay} - ${lastDay})`
    }

    private loadClickWrap(clickwrap: any, baseUrl: string): void {
        const existingScript = document.getElementById('clickwrapscript')
        if (existingScript) {
            this.showClickWrap(clickwrap, baseUrl)
        } else {
            const textScript = this.renderer2.createElement('script')
            textScript.src = baseUrl + this.scriptUrl
            textScript.id = 'clickwrapscript'
            this.renderer2.appendChild(this.document.body, textScript)

            textScript.onload = () => {
                this.showClickWrap(clickwrap, baseUrl)
            }
        }
    }
}
