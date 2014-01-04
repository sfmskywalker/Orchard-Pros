using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Indexing;
using OrchardPros.Models;

namespace OrchardPros {
    public class Migrations : DataMigrationImpl {
        private readonly IRepository<Country> _countryRepository;

        public Migrations(IRepository<Country> countryRepository) {
            _countryRepository = countryRepository;
        }

        public int Create() {
            // Country
            SchemaBuilder.CreateTable("Country", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(64).NotNull())
                .Column<string>("Code", c => c.WithLength(2)));

            // Subscription
            SchemaBuilder.CreateTable("Subscription", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("SubscriptionSourceId", c => c.NotNull())
                .Column<int>("UserId", c => c.NotNull())
                .Column<DateTime>("CreatedUtc", c => c.NotNull()));

            SchemaBuilder.CreateTable("SubscriptionSourcePartRecord", table => table
                .ContentPartRecord());

            ContentDefinitionManager.AlterPartDefinition("SubscriptionSourcePart", part => part
                .Attachable()
                .WithDescription("Turns your content into a source of notifications that users can subscribe to."));

            // Position
            SchemaBuilder.CreateTable("Position", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("UserId")
                .Column<string>("CompanyName", c => c.WithLength(64))
                .Column<string>("Title", c => c.WithLength(64))
                .Column<string>("Location", c => c.WithLength(64))
                .Column<int>("PeriodStartYear")
                .Column<int>("PeriodStartMonth")
                .Column<int>("PeriodEndYear")
                .Column<int>("PeriodEndMonth")
                .Column<bool>("IsCurrentPosition", c => c.NotNull())
                .Column<string>("Description", c => c.Unlimited())
                .Column<bool>("IsArchived")
                .Column<DateTime>("ArchivedUtc"));

            // Skill
            SchemaBuilder.CreateTable("Skill", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("UserId")
                .Column<string>("Name", c => c.WithLength(64))
                .Column<int>("Rating", c => c.NotNull()));

            // Experience
            SchemaBuilder.CreateTable("Experience", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("UserId")
                .Column<int>("PositionId")
                .Column<string>("Description", c => c.Unlimited())
                .Column<DateTime>("CreatedUtc"));

            // Recommendation
            SchemaBuilder.CreateTable("RecommendationPartRecord", table => table
                .ContentPartRecord()
                .Column<int>("UserId"));

            ContentDefinitionManager.AlterPartDefinition("RecommendationPart", part => part
                .Attachable(false)
                .WithDescription("Used for the Recommendation content type"));

            ContentDefinitionManager.AlterTypeDefinition("Recommendation", type => type
                .WithPart("CommonPart")
                .WithPart("IdentityPart")
                .WithPart("BodyPart")
                .WithPart("RecommendationPart")
                .Creatable(false)
                .Draftable(false));

            // Profile
            SchemaBuilder.CreateTable("UserProfilePartRecord", table => table
                .ContentPartRecord()
                .Column<int>("CountryId"));

            ContentDefinitionManager.AlterPartDefinition("UserProfilePart", part => part
                .WithDescription("Stores professional background information about a user"));

            ContentDefinitionManager.AlterPartDefinition("UserProfilePart", part => part
                .WithField("Avatar", field => field
                    .OfType("MediaLibraryPickerField"))
                .WithField("Wallpaper", field => field
                    .OfType("MediaLibraryPickerField"))
                .WithDescription("Provides additional information about the user."));

            // User
            ContentDefinitionManager.AlterTypeDefinition("User", type => type
                .WithPart("UserProfilePart")
                .WithPart("UserProfilePart")
                .Indexed("Users"));
            
            // Voteable
            SchemaBuilder.CreateTable("Vote", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ReplyId", c => c.NotNull())
                .Column<int>("UserId", c => c.NotNull())
                .Column<int>("Points", c => c.NotNull())
                .Column<DateTime>("CreatedUtc", c => c.NotNull()));

            ContentDefinitionManager.AlterPartDefinition("VotablePart", part => part
                .Attachable()
                .WithDescription("Enables users to vote a content item up or down."));

            // Statistics
            SchemaBuilder.CreateTable("StatisticsPartRecord", table => table
                .ContentPartRecord()
                .Column<int>("ViewCount"));

            ContentDefinitionManager.AlterPartDefinition("StatisticsPart", part => part
                .Attachable()
                .WithDescription("Stores statistical information about your content, such as view count."));

            // Attachment
            ContentDefinitionManager.AlterPartDefinition("AttachmentPart", part => part
                .Attachable(false)
                .WithDescription("Stores information about an attachment."));

            ContentDefinitionManager.AlterTypeDefinition("Attachment", type => type
                .WithPart("CommonPart")
                .WithPart("IdentityPart")
                .WithPart("AttachmentPart")
                .Creatable(false)
                .Draftable(false));

            ContentDefinitionManager.AlterPartDefinition("AttachmentsHolderPart", part => part
                .Attachable()
                .WithDescription("Turns your content into a dossier capable of holding attachments."));


            // Ticket
            SchemaBuilder.CreateTable("TicketPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Type", c => c.WithLength(32))
                .Column<decimal>("Bounty")
                .Column<DateTime>("DeadlineUtc")
                .Column<int>("ExperiencePoints")
                .Column<DateTime>("SolvedUtc")
                .Column<int>("AnswerId")
                .Column<string>("Categories", c => c.WithLength(512))
                .Column<string>("Tags", c => c.WithLength(512)));

            ContentDefinitionManager.AlterPartDefinition("TicketPart", part => part
                .WithField("Categories", field => field
                    .OfType("TaxonomyField")
                    .WithSetting("TaxonomyFieldSettings.Taxonomy", "Category"))
                .WithField("Tags", field => field
                    .OfType("TaxonomyField")
                    .WithSetting("TaxonomyFieldSettings.Taxonomy", "Tag")
                    .WithSetting("TaxonomyFieldSettings.AllowCustomTerms", "True")
                    .WithSetting("TaxonomyFieldSettings.Autocomplete", "True"))
                .Attachable(false)
                .WithDescription("Stores expert information about a ticket."));

            ContentDefinitionManager.AlterTypeDefinition("Ticket", type => type
                .WithPart("CommonPart", p => p
                    .WithSetting("DateEditorSettings.ShowDateEditor", "true"))
                .WithPart("TitlePart")
                .WithPart("AutoroutePart", builder => builder
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "false")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'ID', Pattern: 'tickets/{Content.Id}', Description: 'tickets/123'}]")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("BodyPart")
                .WithPart("TicketPart")
                .WithPart("AttachmentsHolderPart")
                .WithPart("CommentsPart")
                .WithPart("StatisticsPart")
                .WithPart("SubscriptionSourcePart")
                .Creatable(false)
                .Draftable());

            // Reply
            ContentDefinitionManager.AlterPartDefinition("ReplyPart", part => part
                .Attachable(false)
                .WithDescription("Turns your content type into a Reply."));

            ContentDefinitionManager.AlterTypeDefinition("Reply", type => type
                .WithPart("CommonPart", part => part
                    .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false")
                    .WithSetting("DateEditorSettings.ShowDateEditor", "false"))
                .WithPart("IdentityPart")
                .WithPart("TitlePart")
                .WithPart("ReplyPart")
                .WithPart("BodyPart")
                .WithPart("AttachmentsHolderPart")
                .WithPart("VotablePart"));

            CreateCountries();
            return 1;
        }

        private void CreateCountries() {
            #region Countries
            var countries = new[] {
                "Afghanistan",
                "Albania",
                "Algeria",
                "American Samoa",
                "Andorra",
                "Angola",
                "Anguilla",
                "Antigua and Barbuda",
                "Argentina",
                "Armenia",
                "Aruba",
                "Australia",
                "Austria",
                "Azerbaijan",
                "Bahamas",
                "Bahrain",
                "Bangladesh",
                "Barbados",
                "Belarus",
                "Belgium",
                "Belize",
                "Benin",
                "Bermuda",
                "Bhutan",
                "Bolivia",
                "Bosnia-Herzegovina",
                "Botswana",
                "Bouvet Island",
                "Brazil",
                "British Virgin Islands",
                "Brunei",
                "Bulgaria",
                "Burkina Faso",
                "Burundi",
                "Cambodia",
                "Cameroon",
                "Canada",
                "Cape Verde",
                "Cayman Islands",
                "Central African Republic",
                "Chad",
                "Channel Islands",
                "Chile",
                "China",
                "Christmas Island",
                "Cocos (Keeling) Islands",
                "Colombia",
                "Comoros",
                "Congo (Brazzaville)",
                "Congo (Democratic Republic)",
                "Cook Islands",
                "Costa Rica",
                "Cote d'Ivoire",
                "Croatia",
                "Cuba",
                "Cyprus",
                "Czech Republic",
                "Denmark",
                "Djibouti",
                "Dominica",
                "Dominican Republic",
                "Ecuador",
                "Egypt",
                "El Salvador",
                "England",
                "Equatorial Guinea",
                "Eritrea",
                "Estonia",
                "Ethiopia",
                "Falkland Islands (Islas Malvinas)",
                "Faroe Islands",
                "Federated States of Micronesia",
                "Fiji",
                "Finland",
                "France",
                "French Guiana",
                "French Polynesia",
                "French Southern Territories",
                "Gabon",
                "Gambia",
                "Georgia (Country)",
                "Germany",
                "Ghana",
                "Gibraltar",
                "Greece",
                "Greenland",
                "Grenada",
                "Guadeloupe",
                "Guam",
                "Guatemala",
                "Guinea",
                "Guinea-Bissau",
                "Guyana",
                "Haiti",
                "Heard Island",
                "Honduras",
                "Hong Kong",
                "Hungary",
                "Iceland",
                "India",
                "Indonesia",
                "Iran",
                "Iraq",
                "Ireland",
                "Isle of Man",
                "Israel",
                "Italy",
                "Jamaica",
                "Japan",
                "Johnston Island",
                "Jordan",
                "Kazakhstan",
                "Kenya",
                "Kiribati",
                "Korea, North",
                "Korea, South",
                "Kuwait",
                "Kyrgyzstan",
                "Laos",
                "Latvia",
                "Lebanon",
                "Lesotho",
                "Liberia",
                "Libya",
                "Liechtenstein",
                "Lithuania",
                "Luxembourg",
                "Macao",
                "Macedonia",
                "Madagascar",
                "Malawi",
                "Malaysia",
                "Maldives",
                "Mali",
                "Malta",
                "Marshall Islands",
                "Martinique",
                "Mauritania",
                "Mauritius",
                "Mayotte",
                "McDonald Islands",
                "Mexico",
                "Moldova",
                "Monaco",
                "Mongolia",
                "Montserrat",
                "Morocco",
                "Mozambique",
                "Myanmar",
                "Namibia",
                "Nauru",
                "Nepal",
                "Netherlands Antilles",
                "Netherlands",
                "New Caledonia",
                "New Zealand",
                "Nicaragua",
                "Niger",
                "Nigeria",
                "Niue",
                "Norfolk Island",
                "Northern Ireland",
                "Northern Mariana Islands",
                "Norway",
                "Oman",
                "Pakistan",
                "Palau",
                "Palestinian Territory",
                "Panama",
                "Papua New Guinea",
                "Paraguay",
                "Peru",
                "Philippines",
                "Pitcairn",
                "Poland",
                "Portugal",
                "Puerto Rico",
                "Qatar",
                "Reunion",
                "Romania",
                "Russia",
                "Rwanda",
                "Saint Helena",
                "Saint Kitts and Nevis",
                "Saint Lucia",
                "Saint Pierre and Miquelon",
                "Saint Vincent and the Grenadines",
                "Samoa",
                "San Marino",
                "Sao Tome and Principe",
                "Saudi Arabia",
                "Scotland",
                "Senegal",
                "Serbia and Montenegro",
                "Seychelles",
                "Sierra Leone",
                "Singapore",
                "Slovakia",
                "Slovenia",
                "Solomon Islands",
                "Somalia",
                "South Africa",
                "Spain",
                "Sri Lanka",
                "Sudan",
                "Suriname",
                "Svalbard and Jan Mayen",
                "Swaziland",
                "Sweden",
                "Switzerland",
                "Syria",
                "Taiwan",
                "Tajikistan",
                "Tanzania",
                "Thailand",
                "Timor Leste",
                "Togo",
                "Tokelau",
                "Tonga",
                "Trinidad and Tobago",
                "Tunisia",
                "Turkey",
                "Turkmenistan",
                "Turks and Caicos",
                "Tuvalu",
                "Uganda",
                "Ukraine",
                "United Arab Emirates",
                "United Kingdom",
                "United States",
                "Uruguay",
                "USSR (former)",
                "Uzbekistan",
                "Vanuatu",
                "Vatican City (Holy See)",
                "Venezuela",
                "Vietnam",
                "Virgin Islands of the United States",
                "Wales",
                "Wallis & Futuna Islands",
                "Western Sahara",
                "Yemen",
                "Yugoslavia (former)",
                "Zambia",
                "Zimbabwe"
            };
            #endregion

            foreach (var country in countries) {
                _countryRepository.Create(new Country { Name = country });
            }
        }
    }
}