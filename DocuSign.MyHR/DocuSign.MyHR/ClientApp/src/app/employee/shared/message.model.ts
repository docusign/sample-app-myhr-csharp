import { TemplateRef } from '@angular/core'

export interface IMessage {
    header: string
    body: TemplateRef<unknown> | string
}
