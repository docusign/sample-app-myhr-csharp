export function popSavedDataFromStorage(key: string): string {
    const savedData = sessionStorage.getItem(key)

    if (savedData) {
        sessionStorage.removeItem(key)
    }
    return savedData
}
