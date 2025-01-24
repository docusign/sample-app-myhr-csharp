using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class TuitionReimbursementTemplateHandler : ITemplateHandler
    {
        private string _signerClientId = "1000";
        private string _templatePath = "/Templates/Tuition Reimbursement.docx";
        public string TemplateName => "Tuition Reimbursement";
        public EnvelopeTemplate BuildTemplate(string rootDir)
        {
            var envelopeTemplate = new EnvelopeTemplate
            {
                Name = "Tuition Reimbursement",
                EmailSubject = "Please sign this document",
                Documents = new List<Document> {
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(File.ReadAllBytes(rootDir + _templatePath)),
                        Name = "Tuition Reimbursement",
                        FileExtension = "docx",
                        DocumentId = "1"
                    }
                },

                Recipients = new Recipients
                {
                    Signers = new List<Signer>
                    {
                        new Signer
                        {
                            RecipientId = "1",
                            RoleName = "Employee",
                            RoutingOrder = "1",
                            Tabs = CreateTabs()
                        }
                    }
                }
            };

            return envelopeTemplate;
        }

        public EnvelopeDefinition BuildEnvelope(UserDetails currentUser, UserDetails additionalUser)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }  

            var env = new EnvelopeDefinition
            { 
                TemplateRoles = new List<TemplateRole>
                {
                    new TemplateRole
                    {
                        Email = currentUser.Email,
                        Name = currentUser.Name,
                        ClientUserId = _signerClientId,
                        RoleName = "Employee"
                    }
                },
                Status = "sent"
            };

            return env;
        }

        private Tabs CreateTabs()
        {
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere>
                {
                    new SignHere
                    {
                        XPosition = "158",
                        YPosition = "415",
                        Optional = "false",
                        StampType = "signature",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Name = "SignHere",
                    }
                },
                DateSignedTabs = new List<DateSigned>
                {
                    new DateSigned
                    {
                        Name = "DateSigned",
                        DocumentId = "1",
                        PageNumber = "1",
                        XPosition = "415",
                        YPosition = "435",
                        TemplateLocked = "false",
                        TemplateRequired = "false",
                        TabType = "datesigned"
                    }
                },
                DateTabs = new List<Date>
                {
                    new Date
                    {
                        TabLabel = "StartDate",
                        ValidationMessage = "Start date must be specified",
                        ValidationPattern = @"^(|by DocuSign)((|0)[1-9]|1[0-2])\/((|0)[1-9]|[1-2][0-9]|3[0-1])\/[0-9]{4}$",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "true",
                        MaxLength = "50",
                        XPosition = "374",
                        YPosition = "181",
                        Width = "151",
                        Height = "23",
                    },
                    new Date
                    {
                        TabLabel = "EndDate",
                        ValidationMessage = "End date must be specified",
                        ValidationPattern = @"^(|by DocuSign)((|0)[1-9]|1[0-2])\/((|0)[1-9]|[1-2][0-9]|3[0-1])\/[0-9]{4}$",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "true",
                        MaxLength = "50",
                        XPosition = "374",
                        YPosition = "214",
                        Width = "151",
                        Height = "23",
                    },
                },
                NumberTabs = new List<Number>
                {
                    new Number
                    {
                        TabLabel = "GradeInPercentage",
                        ValidationMessage = "Grade percentage is required",
                        Required = "true",
                        ValidationPattern = "^[1-9][0-9]?$|^100$",
                        MaxLength = "50",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        XPosition = "374",
                        YPosition = "247",
                        Width = "151",
                        Height = "23",
                     },
                    new Number
                    {
                        TabLabel = "Cost",
                        ValidationMessage = "Cost is required",
                        Required = "true",
                        MaxLength = "50",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        XPosition = "374",
                        YPosition = "360",
                        Width = "151",
                        Height = "23",
                    },
                },
                SignerAttachmentTabs = new List<SignerAttachment>
                {
                    new SignerAttachment
                    {
                        TabLabel = "RecordOfCompletion", 
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        XPosition = "420",
                        YPosition = "275",
                        Width = "151",
                        Height = "23",
                    }
                },
                TextTabs = new List<Text>
                {
                    new Text
                    {
                        TabLabel = "NameOfCourse",
                        ValidationMessage = "Name of course is required",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "true",
                        MaxLength = "100",
                        XPosition = "374",
                        YPosition = "116",
                        Width = "151",
                        Height = "23",
                    },
                    new Text
                    {
                        TabLabel = "NameOfInstitution",
                        ValidationMessage = "Name of institution is required",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "true",
                        MaxLength = "50",
                        XPosition = "374",
                        YPosition = "147",
                        Width = "151",
                        Height = "23",
                    },
                     
                },
            };
            return signer1Tabs;
        }
    }
}
