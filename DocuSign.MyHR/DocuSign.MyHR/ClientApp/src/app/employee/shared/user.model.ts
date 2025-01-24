export interface IUser {
    id: number
    loginType: number
    name: string
    firstName: string
    lastName: string
    hireDate: string
    email: string
    profileImage: string
    profileId: string
    address?: {
        address1: string
        address2: string
        city: string
        country: string
        fax: string
        phone: string
        postalCode: string
        stateOrProvince: string
    }
}
