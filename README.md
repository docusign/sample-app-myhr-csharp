# The Docusign HR Sample Application

## Introduction
The HR Sample Application is a Docusign sample application demonstrating how the Docusign APIs can be used to construct applications for human resources. MyHR is written in C# using ASP.Net Core 3.1 (server) and Angular 9 (client). You can find a live instance running at [https://hr.sampleapps.docusign.com/](https://hr.sampleapps.docusign.com/).

The HR Sample Application demonstrates the following:
1. Authentication with two different methods
    * [Authentication Code Grant](https://developers.docusign.com/platform/auth/authcode/)
    * [JSON Web Token (JWT) Grant](https://developers.docusign.com/platform/auth/jwt/)

2. User information is shown from the Docusign account. This example demonstrates [Users API endpoint](https://developers.docusign.com/platform/auth/reference/user-info/) functionality.

3. Direct Deposit update. This example demonstrates filling in bank account information for direct deposit and submitting it to be processed by payroll.

4. W-4 Tax withholding. This example demonstrates filling in a standard W-4 form required by the IRS from all US employees.

5. Time tracking. This example shows how to use the Click API to create an elastic template programmatically, render it in your UI, and then submit it. It also tracks the submission event and, just after submission, redirects the user back to the start page.
   * [More information about the Click API](https://developers.docusign.com/docs/click-api/)
6.	Tuition reimbursement. To prove that a class was completed, users can also attach some written proof before submitting their request for reimbursement. Adding attachments lets users add additional documents for verification.
   * [More information about adding attachments](https://support.docusign.com/en/guides/signer-guide-signing-adding-attachments-new)
7. Send an offer letter to a job candidate. The offer is approved internally first by the user and then sent to a candidate for signing.

8. Send an I-9 verification request to a job candidate using IDV. This example demonstrates sending the Federal I-9 form to a new hire.
   * [More information about ID Verification](https://developers.docusign.com/docs/esign-rest-api/esign101/concepts/recipients/auth/#idv)

The examples with templates were created using these Docusign APIs and features:
   * The Docusign [Template API](https://developers.docusign.com/docs/esign-rest-api/how-to/create-template/) functionality.
   * The signing ceremony is implemented with [embedded signing](https://developers.docusign.com/docs/esign-rest-api/how-to/request-signature-in-app-embedded/) for a single signer.
   * The Docusign signing ceremony is initiated from your website.
   * [AutoPlace](https://developers.docusign.com/docs/esign-rest-api/esign101/concepts/tabs/auto-place/) (anchor text) is used to position the signing fields in the document.

## Installation

### Prerequisites
* A Docusign Developer demo account (email and password) on [demo.docusign.net](https://demo.docusign.net). If you don't already have a developer demo account, create a [free account](https://www.docusign.com/developers/sandbox).
* A Docusign integration key (a client ID) that is configured to use **JSON Web Token (JWT) Grant** authentication.
   You will need the **integration key** itself and its **RSA key pair**. To use this application, you must add your application's **Redirect URI** to your integration key. This [**video**](https://www.youtube.com/watch?v=GgDqa7-L0yo) demonstrates how to create an integration key (client ID) for a user application such as this example.
* C# .NET Core version 3.1 or later
* [Node.js](https://nodejs.org/) v10+

### Installation steps
**Manual**
1. Download or clone this repository to your workstation in a new folder named **MyHRSampleApp**.
2. The repository includes a Visual Studio 2019 solution file and NuGet package references in the project file.
3. Copy appsettings-example.json to create appsettings.json (the configuration file) and udate the integration key and other settings from your Docusign Developer demo account.
    > **Note:** Protect your integration key and client secret. You should make sure that the **.env** file will not be stored in your source code repository.
4. Navigate to that folder: **cd sample-app-myhr-csharp**
5. Install client-side dependencies using the npm package manager: **npm install**

## Running The HR Sample App
**Manual**
1. Navigate to **DocuSign.MyHR/DocuSign.MyHR/ClientApp** folder and run **npm start** command.
1. Build and then start the solution.
1. Your default browser will be opened to https://localhost:5001/ and you will see the application's home page.

## License information
This repository uses the MIT License. See the [LICENSE](./LICENSE) file for more information.
