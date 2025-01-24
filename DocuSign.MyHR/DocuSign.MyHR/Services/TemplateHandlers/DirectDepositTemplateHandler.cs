using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class DirectDepositTemplateHandler : ITemplateHandler
    {
        private string _signerClientId = "1000";
        private string _templatePath = "/Templates/Direct Deposit.docx";
        public string TemplateName => "Direct Deposit";

        public EnvelopeTemplate BuildTemplate(string rootDir)
        {
            var envelopeTemplate = new EnvelopeTemplate
            {
                Name = "Direct Deposit",
                EmailSubject = "Please sign this document",
                Documents = new List<Document> {
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(File.ReadAllBytes(rootDir + _templatePath)),
                        Name = "Direct Deposit Update",
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
                            Tabs = CreateTabs(),
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
                        XPosition = "170",
                        YPosition = "418",
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
                        TabLabel="DateSigned",
                        DocumentId = "1",
                        PageNumber = "1",
                        XPosition = "419",
                        YPosition = "426",
                        TemplateLocked = "false",
                        TemplateRequired = "false",
                        TabType = "datesigned"
                    }
                },
                NumberTabs = new List<Number>
                {
                    new Number
                    {
                        TabLabel = "PrimaryRoutingNumber", 
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "true",
                        MaxLength = "100",
                        XPosition = "374",
                        YPosition = "106",
                        Width = "151",
                        Height = "23",
                        TabType = "number"
                    },
                    new Number
                    {
                        TabLabel = "PrimaryAccountNumber", 
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "true",
                        MaxLength = "50",
                        XPosition = "374",
                        YPosition = "137",
                        Width = "151",
                        Height = "23",
                        TabType = "number"
                    },
                    new Number
                    {
                        TabLabel = "PercentageFirstAccount",
                        ValidationPattern = "^[1-9][0-9]?$|^100$", 
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "false",
                        MaxLength = "3",
                        XPosition = "374",
                        YPosition = "171",
                        Width = "151",
                        Height = "23",
                        TabType = "number"
                    },
                    new Number
                    {
                        TabLabel = "SecondaryRoutingNumber",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "false",
                        MaxLength = "50",
                        XPosition = "374",
                        YPosition = "204",
                        Width = "151",
                        Height = "23",
                        TabType = "number",
                    },
                    new Number
                    {
                        TabLabel = "SecondaryAccountNumber",
                        ValidationMessage = "Secondary account number is required",
                        Required = "false",
                        MaxLength = "50",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        XPosition = "374",
                        YPosition = "237",
                        Width = "151",
                        Height = "23",
                        TabType = "number"
                    },
                    new Number
                    {
                        TabLabel = "PercentageSecondAccount",
                        ValidationPattern = "^[1-9][0-9]?$|^100$", 
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "false",
                        MaxLength = "3",
                        XPosition = "374",
                        YPosition = "275",
                        Width = "151",
                        Height = "23",
                        TabType = "number"
                    }
                },
                NoteTabs = new List<Note>
                {
                    new Note
                    {
                        Value = "Total Percentage must be equal to hundred",
                        Name = "Note to recipient",
                        TabLabel = "ValidationMessage",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        XPosition = "85",
                        YPosition = "370",
                        Width = "300",
                        Height = "20",
                        FontColor = "BrightRed",
                        ConditionalParentLabel = "NumberValidation",
                        ConditionalParentValue = "0",
                    }
                },
                FormulaTabs = new List<FormulaTab>
                {
                    new FormulaTab
                    {
                        Formula = "[PercentageFirstAccount] + [PercentageSecondAccount]",
                        Name = "Formula Tab",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        XPosition = "378",
                        YPosition = "322",
                        RoundDecimalPlaces = "0",
                        Locked = "true",
                        ConcealValueOnDocument = "false",
                        TabLabel = "TotalPercentage",
                        TabType = "formula",
                        Hidden =  "false",
                        Width =  "42",
                        TemplateLocked = "false",
                        TemplateRequired = "false",
                        DisableAutoSize = "false",
                        Required = "true",
                    },

                    new FormulaTab
                    {
                        Formula = "[PercentageFirstAccount] + [PercentageSecondAccount] = 100",
                        Name = "Formula Tab",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        RoundDecimalPlaces = "0",
                        Locked = "true",
                        ConcealValueOnDocument = "true",
                        TabLabel = "NumberValidation",
                        TabType = "formula",
                        Required = "true",
                    }
                },
            };
            return signer1Tabs;
        }
    }
}
