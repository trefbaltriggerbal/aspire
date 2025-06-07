# Aspire Demo

This repository contains a minimal .NET 8 sample that demonstrates how to use the preview of **.NET Aspire** to orchestrate multiple services. The solution consists of:

- `Sender` – console app that sends an HTTP request to `Receiver`.
- `Receiver` – console app that hosts a small HTTP server and responds with `Pong`.
- `WebFrontend` – Blazor Web App that can trigger the `Sender` project.
- `AspireHost` – console project that will host the distributed application using the `Aspire.Hosting` package.
- `LCG API` – HTTP service exposing the linear congruential generator.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Install the Aspire workload:

```bash
dotnet workload install aspire
```

## Building the solution

```bash
dotnet build AspireDemo.sln
```

## Running the apps individually

Start the receiver in one terminal:

```bash
dotnet run --project Receiver
```

Then in another terminal start the sender:

```bash
dotnet run --project Sender
```

The sender will call `http://receiver/ping` using service discovery to reach the receiver.

Both console projects run through the `Host` builder and use
`AddServiceDefaults` so their logs are forwarded to Aspire's monitoring
pipeline via OpenTelemetry. The built‑in JSON console formatter still outputs
structured logs locally, and each message includes its log level, timestamp and
structured data to make log analysis easier.

`AddServiceDefaults` also wires up OpenTelemetry HTTP instrumentation so the
`traceparent` header is added automatically to requests made by `HttpClient`.
The receiver extracts that header using the default propagator so spans from
both services share the same TraceId in the Aspire dashboard.

## Configuring log levels

Each project uses a root namespace starting with `Projects`, such as `Projects.Sender`. The logging configuration leverages this prefix so you can control the verbosity of your own code separately from framework components. An example `appsettings.json` section looks like:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "Projects": "Debug"
  }
}
```

Because the `Projects` category matches each project's root namespace, adjusting this setting filters user-generated logs without affecting messages from ASP.NET Core or other libraries.


## Running with Aspire

The `AspireHost` project references the `Aspire.Hosting` package. When executed it will start the other projects and manage them as a distributed application. Run it with:

```bash
dotnet run --project AspireHost
```

This will launch the receiver and the web frontend as part of the distributed app. The sender project can be triggered from the web frontend or run separately.

## Next steps

This repository serves as a starting point for experimenting with .NET Aspire. Feel free to extend the projects, add more services or containers and explore the orchestration capabilities provided by Aspire.

For a step-by-step walkthrough see [the tutorial](TUTORIAL.md).

## Planned improvements

The following GitHub issues track upcoming features for logging and observability:
- [#9](https://github.com/trefbaltriggerbal/aspire/issues/9) Enable console logging
- [#10](https://github.com/trefbaltriggerbal/aspire/issues/10) Add structured logging
- [#11](https://github.com/trefbaltriggerbal/aspire/issues/11) Implement distributed tracing
- [#12](https://github.com/trefbaltriggerbal/aspire/issues/12) Add metrics
- [#16](https://github.com/trefbaltriggerbal/aspire/issues/16) Structured logs for console apps


# GitHub Issues Set voor Klantenportaal Verzekeringsmakelaar

Om de ontwikkeling van het klantenportaal gestructureerd aan te pakken, zijn de gebruikersvereisten onderverdeeld in Epics met onderliggende Features. Elke **Epic** is een hoofdcategorie (gemarkeerd met het label `Epic`) en bevat een checklist van **Feature**-issues (gemarkeerd met `Feature`) die in sprints uitgevoerd kunnen worden. Waar van toepassing zijn labels als `Security`, `Design`, `Notification`, `Performance`, `Payment` toegevoegd voor de aard van de feature, en prioriteitslabels `MVP` (essentieel voor een Minimale Levensvatbare Productversie) of `Nice to have` (niet cruciaal voor MVP) om de belangrijkheid aan te geven.

## Epic: Authenticatie & Security

**Labels:** `Epic`, `Security`  
**Omschrijving:** Deze epic omvat alle functionaliteiten rond gebruikersauthenticatie, autorisatie en beveiliging van accounts in het portaal. Denk aan inlog- en registratieprocessen, wachtwoordbeheer en extra beveiligingsmaatregelen.  
**Sub-features:**

-  **Feature: Basis authenticatie en autorisatie** (MVP)
    
-  **Feature: Wachtwoordherstel en e-mailverificatie** (MVP)
    
-  **Feature: Tweefactor-authenticatie (2FA)** (Nice to have)
    

### Feature: Basis authenticatie en autorisatie

**Omschrijving:** Implementeer een basis authenticatiesysteem voor alle gebruikersrollen. Klanten kunnen inloggen met hun klantnummer of e-mailadres en een wachtwoord. _Ouder_-gebruikers krijgen toegang via een eenmalige token-link die per e-mail wordt gedeeld door de klant, en kunnen desgewenst later een eigen wachtwoord instellen. Beheerders (administrators) hebben volledige toegang tot het portaal en hun accounts worden vooraf aangemaakt. Deze feature omvat ook het opzetten van gebruikersrollen en -rechten zodat gevoelige pagina’s alleen door de juiste rol benaderd kunnen worden.  
**Labels:** `Feature`, `Security`, `MVP`

### Feature: Wachtwoordherstel en e-mailverificatie

**Omschrijving:** Voorzie functionaliteit voor gebruikers om veilig hun wachtwoord te herstellen indien ze het vergeten zijn. Wanneer een gebruiker aangeeft zijn wachtwoord vergeten te zijn, moet het systeem een e-mail met een reset-link versturen. Na klikken kunnen ze een nieuw wachtwoord instellen (met de nodige validatie, bv. minimaal aantal tekens). Ook nieuwe accounts of e-mailwijzigingen vereisen een e-mailverificatie: stuur een bevestigingslink die geactiveerd moet worden voordat het account volledig toegang krijgt. Hiermee verbeteren we de accountveiligheid en voorkomen we ongeautoriseerde toegang.  
**Labels:** `Feature`, `Security`, `MVP`

### Feature: Tweefactor-authenticatie (2FA)

**Omschrijving:** Implementeer optioneel een tweefactor-authenticatie voor extra beveiliging. Bij het inloggen wordt, naast het wachtwoord, een tweede verificatiestap gevraagd (bijvoorbeeld een eenmalige code via een authenticator-app of per SMS/E-mail). Deze feature verkleint de kans op ongeoorloofde toegang zelfs al zouden inloggegevens uitgelekt zijn. **Opmerking:** introductie van 2FA is een _Nice to have_ en kan geblokkeerd zijn tot de basisauthenticatie stabiel werkt en er besluit is genomen over het type tweede factor.  
**Labels:** `Feature`, `Security`, `Nice to have`

---

## Epic: Client Functionaliteiten

**Labels:** `Epic`, `Design`  
**Omschrijving:** Deze epic groepeert alle functies die beschikbaar zijn voor een ingelogde **klant** in het portaal. Het betreft gebruiksvriendelijke pagina’s en acties voor klanten, zoals inzicht in eigen polissen, schadeclaims indienen en opvolgen, documenten aanleveren en persoonlijke gegevens beheren.  
**Sub-features:**

-  **Feature: Polisoverzicht voor klant** (MVP)
    
-  **Feature: Schadeclaims overzicht & status** (MVP)
    
-  **Feature: Schadeclaim indienen (met documentupload)** (MVP)
    
-  **Feature: Persoonlijke gegevens beheren** (MVP)
    

### Feature: Polisoverzicht voor klant

**Omschrijving:** Ontwikkel een overzichtspagina waar de klant al zijn/haar verzekeringspolissen kan bekijken. Voor elke polis worden basisgegevens getoond, zoals polisnummer, type verzekering, dekking, premie en geldigheidsduur. De klant moet kunnen doorklikken naar details van een polis, waar uitgebreidere informatie zichtbaar is (bijv. verzekerde objecten of personen, voorwaarden en documenten gekoppeld aan de polis). Dit geeft de gebruiker een duidelijk beeld van zijn verzekeringspakket binnen het portaal.  
**Labels:** `Feature`, `Design`, `MVP`

### Feature: Schadeclaims overzicht & status

**Omschrijving:** Bied de klant een pagina om alle schadeclaims (schadedossiers) in te zien die bij de makelaar zijn ingediend. Toon per polis welke claims ervoor zijn, opgesplitst in **lopende** (open) claims en **afgehandelde** (gesloten) claims. Voor elke schadeclaim is de status zichtbaar (bijv. _In Behandeling_, _Goedgekeurd_, _Uitbetaald_ of _Afgewezen_) samen met relevante details zoals datum van indiening. De klant kan doorklikken voor meer details over een specifieke claim, inclusief de beschrijving van het schadegeval, ingediende documenten en een historie van statusupdates. Zo behoudt de klant overzicht en duidelijkheid over de afhandeling van zijn claims.  
**Labels:** `Feature`, `Design`, `MVP`

### Feature: Schadeclaim indienen (met documentupload)

**Omschrijving:** Maak het mogelijk dat een klant online een nieuw schadegeval kan **indienen** via het portaal. De klant doorloopt een formulier waarbij hij de betreffende polis selecteert (alleen mogelijk als de polis actief is binnen de dekkingsperiode) en details over het voorval invult (datum, beschrijving, betrokken partijen, enz.). Tijdens het meldproces kan de gebruiker meteen ondersteunende **documenten uploaden** (bijv. foto’s van de schade, politieverslag, bewijsstukken). Na indiening ontvangt de klant een bevestiging (bijvoorbeeld via e-mail, zie Notificatiesysteem) en wordt de nieuwe claim zichtbaar in het claims-overzicht met status _Ingediend/In Behandeling_. Deze feature digitaliseert het klassieke schadeaangifteproces en versnelt de communicatie.  
**Labels:** `Feature`, `Design`, `MVP`

### Feature: Persoonlijke gegevens beheren

**Omschrijving:** Voorzie een _Profiel_pagina waar de klant zijn persoonlijke gegevens kan bekijken en bijwerken. Denk aan contactinformatie (adres, telefoonnummer, e-mail) en mogelijk voorkeuren (taalvoorkeur voor communicatie, notificatie-instellingen). Wijzigingen die de klant maakt (zoals een nieuw adres of een aangepast telefoonnummer) moeten worden opgeslagen na bevestiging en eventueel ter controle gemeld aan een beheerder. Ook moet de klant hier zijn wachtwoord kunnen wijzigen voor het portaal. Deze zelfbedieningsfunctie zorgt ervoor dat gebruikersgegevens up-to-date blijven zonder tussenkomst van de makelaar.  
**Labels:** `Feature`, `Design`, `MVP`

---

## Epic: Parent Functionaliteiten

**Labels:** `Epic`, `Security`  
**Omschrijving:** In dit deel verzamelen we functies voor **Ouder-accounts** (ouders of voogden) die door een klant gemachtigd zijn om beperkte inzage te krijgen in bepaalde informatie, bijvoorbeeld inzage in de schadeclaims van een minderjarige. Deze functies zorgen dat ouders op een veilige manier toegang krijgen zonder dat ze zelf een volledige klantenaccount hoeven te hebben bij aanvang.  
**Sub-features:**

-  **Feature: Ouder uitnodigen & token-inlog** (MVP)
    
-  **Feature: Gedeelde schadeclaim bekijken (ouder)** (MVP)
    
-  **Feature: Ouder permanent account (wachtwoord instellen)** (Nice to have)
    

### Feature: Ouder uitnodigen & token-inlog

**Omschrijving:** Implementeer een mechanisme waarmee een klant een ouder/gevolmachtigde kan **uitnodigen** om een specifiek schadegeval in te zien. Het systeem genereert een unieke **token** en verstuurt een e-mailuitnodiging naar de ouder met een eenmalige inloglink. Via deze link krijgt de ouder toegang tot het portaal (beperkte rechten) zonder initieel wachtwoord. De token-login is beveiligd en slechts geldig voor beperkte tijd en/of voor de gespecificeerde claim. Dit stelt ouders in staat betrokken te raken bij de afhandeling van een schadegeval (bijvoorbeeld van hun kind) op een gecontroleerde manier.  
**Labels:** `Feature`, `Security`, `MVP`

### Feature: Gedeelde schadeclaim bekijken (ouder)

**Omschrijving:** Zorg dat een ouder die is ingelogd via een uitnodiging alleen de informatie ziet waartoe hij/zij gemachtigd is. Concreet betekent dit dat de ouder een **gedeeld schadeclaim-dossier** kan bekijken inclusief de status en details van die claim. De ouder kan de beschrijving van het schadegeval, de statusupdates, en alle bijhorende documenten en informatie inzien (inclusief bijvoorbeeld eventuele slachtoffergegevens of uitkeringsdetails, voor zover toegestaan). Ook moeten eventueel gekoppelde **betalingsbewijzen** of afhandelingsdocumenten voor die claim beschikbaar zijn als download voor de ouder. De ouder heeft **alleen-lezen** rechten en ziet geen andere polis- of claiminformatie van de klant.  
**Labels:** `Feature`, `Security`, `MVP`

### Feature: Ouder permanent account (wachtwoord instellen)

**Omschrijving:** Bied ouders de mogelijkheid om na de eerste token-login een volwaardig account aan te maken met een eigen wachtwoord voor toekomstig gebruik. Deze feature komt van pas als ouders regelmatig of voor meerdere claims toegang nodig hebben. Na inloggen met de eenmalige link kan de ouder een gebruikersnaam/e-mail koppelen en een wachtwoord instellen voor vervolglogins. Het account van de ouder blijft echter beperkt tot de gedeelde informatie die relevant is (hij/zij kan bijvoorbeeld meerdere gedeelde claims van één of meerdere kinderen zien indien van toepassing). **Opmerking:** Dit is een uitbreiding voor gebruiksgemak op langere termijn en is gemarkeerd als _Nice to have_, mogelijk geblokkeerd tot de basis ouder-toegang goed functioneert.  
**Labels:** `Feature`, `Nice to have`

---

## Epic: Administrator Functionaliteiten

**Labels:** `Epic`  
**Omschrijving:** Deze epic omvat alle beheerfuncties voor **administrators** (beheerders bij de verzekeringsmakelaar). Via een admin-gedeelte van het portaal kunnen zij klantgegevens beheren, claims opvolgen en de algemene instellingen van het systeem controleren. Dit is essentieel voor interne medewerkers om het klantenportaal te ondersteunen en modereren.  
**Sub-features:**

-  **Feature: Beheer van klanten en ouderaccounts** (MVP)
    
-  **Feature: Polis- en contractbeheer (admin)** (MVP)
    
-  **Feature: Schadeclaims afhandelen & toewijzen** (MVP)
    
-  **Feature: Beveiligingslogboek inzage (admin)** (Nice to have)
    

### Feature: Beheer van klanten en ouderaccounts

**Omschrijving:** Ontwikkel een beheerderspagina waar alle **klantaccounts** en **ouderaccounts** beheerd kunnen worden. Beheerders kunnen in een lijst alle gebruikers zien, zoeken op naam/nummer en individuele profielen openen. Ze hebben de mogelijkheid om klantgegevens te wijzigen (bijvoorbeeld foutieve info corrigeren), nieuwe klanten of ouders aan te maken (of uit te nodigen) en indien nodig accounts te deactiveren of verwijderen. Ook is een specifieke actie voorzien om voor een gebruiker een wachtwoordreset in gang te zetten of een nieuwe uitnodigingsmail (voor ouders) te versturen. Dit biedt de makelaar controle over de gebruikersadministratie van het portaal.  
**Labels:** `Feature`, `MVP`

### Feature: Polis- en contractbeheer (admin)

**Omschrijving:** Voorzie functionaliteit voor beheerders om verzekeringspolissen en contracten in te zien en te beheren via het portaal. In het admin-gedeelte moet men per klant de lopende polissen kunnen bekijken en nieuwe polissen kunnen toevoegen of bestaande aanpassen (bijv. dekking wijzigen, een polis verlengen of stopzetten). Deze feature kan inhouden: een formulier om een nieuwe polis aan een klant toe te voegen (met alle relevante details zoals type verzekering, voorwaarden, premie, geldigheidsduur) en mogelijkheden om polisdocumenten te uploaden (zie Documentbeheer). Dit stelt de makelaar in staat om wijzigingen of toevoegingen in het verzekeringspakket van de klant direct via het portaal door te voeren.  
**Labels:** `Feature`, `MVP`

### Feature: Schadeclaims afhandelen & toewijzen

**Omschrijving:** Maak een admin-interface voor **schadebeheer**. Beheerders moeten alle ingediende schadeclaims kunnen inzien en filteren op status, klant, datum, etc. Binnen een claimdossier kan de beheerder de details bekijken en de claim verder **afhandelen**: dit omvat het bijwerken van de status (bijv. van _In Behandeling_ naar _Goedgekeurd_ of _Afgewezen_), het toevoegen van interne notities, en het uploaden van eventuele aanvullende documenten of correspondentie. Daarnaast moet het mogelijk zijn een claim toe te **wijzen** aan een bepaalde behandelaar (medewerker of externe expert) voor verdere opvolging – dit kan gewoon middels een veld of dropdown in het dossier. Automatische notificaties (zie Notificatiesysteem) worden uitgestuurd bij belangrijke wijzigingen. Deze feature zorgt ervoor dat de volledige levenscyclus van een schadeclaim binnen het portaal beheerd kan worden door de makelaar.  
**Labels:** `Feature`, `MVP`

### Feature: Beveiligingslogboek inzage (admin)

**Omschrijving:** Integreer een pagina in het admin-gedeelte waarin beheerders het **beveiligingslogboek** en andere audit-logs kunnen inzien (zie Logging & Auditing epic voor het registreren ervan). Deze pagina toont een lijst van belangrijke acties en gebeurtenissen, zoals inlogpogingen (gelukt of mislukt) van gebruikers, uitgevoerde wachtwoordresets, wijzigingen in gebruikersprofielen, en wijzigingen in polissen of claims. Voor elke logregel is detailinformatie beschikbaar (datum/tijd, betrokken gebruiker of systeemproces, omschrijving van de actie). Indien nodig kan de beheerder filters toepassen (bijv. alle gebeurtenissen van een bepaalde gebruiker of een bepaalde datum) en exportfuncties gebruiken om een audit-rapportage te genereren. **Opmerking:** Deze beheerfunctie is gemarkeerd als _Nice to have_, omdat het mogelijk is om in eerste instantie logbestanden enkel op serverniveau te monitoren. Uiteindelijk verhoogt dit de transparantie en traceerbaarheid voor audits en compliance.  
**Labels:** `Feature`, `Nice to have`

---

## Epic: Documentbeheer

**Labels:** `Epic`  
**Omschrijving:** Deze epic behandelt het **uploaden, opslaan en beschikbaar stellen van documenten** binnen het portaal. Denk hierbij aan verzekeringspolissen (contractdocumenten), schadedocumenten (foto’s, bewijsstukken) en andere bijlagen. Een goed documentbeheer zorgt dat gebruikers de juiste bestanden kunnen aanleveren en raadplegen, terwijl de makelaar ze veilig kan beheren.  
**Sub-features:**

-  **Feature: Polisdocumenten beschikbaar voor klanten** (MVP)
    
-  **Feature: Documenten uploaden bij schademelding** (MVP)
    
-  **Feature: Documentbeheer voor beheerders** (MVP)
    

### Feature: Polisdocumenten beschikbaar voor klanten

**Omschrijving:** Maak het mogelijk dat voor elke verzekeringspolis de bijbehorende **polisdocumenten** digitaal beschikbaar zijn in het portaal. De beheerder kan bij het invoeren of updaten van een polis (zie Polisbeheer) een PDF of ander document uploaden met de polisvoorwaarden of contract. De klant ziet in het polisdetail een sectie “Documenten” waar hij deze bestanden kan **downloaden**. Zo heeft de klant altijd toegang tot zijn officiële verzekeringsdocumenten (polisblad, algemene voorwaarden, etc.) zonder papieren te hoeven raadplegen. Het systeem moet zorgen voor veilige opslag van deze bestanden en alleen de betreffende klant (en bevoegde admins) toegang geven.  
**Labels:** `Feature`, `MVP`

### Feature: Documenten uploaden bij schademelding

**Omschrijving:** Tijdens of na het indienen van een schadeclaim (zie feature _Schadeclaim indienen_) moet de klant **documenten kunnen uploaden** die relevant zijn voor de claim. Dit kunnen foto’s van de schade zijn, PDFs van offertes of rekeningen, scans van formulieren, enzovoort. De uploadfunctie ondersteunt gangbare bestandstypen en toont progressie en bevestiging zodra een bestand succesvol is opgeladen. Na upload zijn de bestanden gekoppeld aan het schadedossier en zichtbaar voor de behandelaars/administrators in de admin-omgeving. Deze documenten kunnen door de beheerder bekeken en later eventueel gedownload worden ter dossiervorming. Hiermee wordt het proces van schademelding volledig digitaal en hoeven klanten geen fysieke documenten meer op te sturen.  
**Labels:** `Feature`, `MVP`

### Feature: Documentbeheer voor beheerders

**Omschrijving:** Voorzie beheerders van een centrale plek om alle **geüploade documenten** te beheren. In de admin-omgeving komt een documentbeheer-sectie of functionaliteit waarmee men documenten kan opzoeken op klant, polis of claim. Beheerders kunnen hier nieuwe documenten uploaden (bijvoorbeeld een extra brief toevoegen aan een claimdossier), documenten verwijderen of vervangen (bijv. als een verkeerd bestand is geüpload), en eventueel tags of beschrijvingen aan bestanden toevoegen voor makkelijk terugvinden. Belangrijk is dat documenten die persoonlijke of gevoelige informatie bevatten veilig worden opgeslagen (bv. versleuteld waar nodig) en dat rechten worden gehandhaafd (een beheerder ziet alle documenten, een klant alleen eigen documenten). Dit vereenvoudigt het interne beheer van alle bestanden die door klanten en medewerkers in het portaal worden gebruikt.  
**Labels:** `Feature`, `MVP`

---

## Epic: Notificatiesysteem

**Labels:** `Epic`, `Notification`  
**Omschrijving:** Deze epic omvat het **meldingen-systeem** van het portaal. Het doel is gebruikers (klanten, ouders, en eventueel beheerders) op de hoogte te houden van belangrijke gebeurtenissen via e-mail en/of binnen de portal zelf. Automatische notificaties verhogen de betrokkenheid en zorgen dat iedereen tijdig actie kan ondernemen wanneer nodig.  
**Sub-features:**

-  **Feature: E-mail notificaties bij statusupdates** (MVP)
    
-  **Feature: In-portal notificaties voor gebruikers** (Nice to have)
    
-  **Feature: Notificatievoorkeuren instellen** (Nice to have)
    

### Feature: E-mail notificaties bij statusupdates

**Omschrijving:** Implementeer automatische **e-mailmeldingen** naar klanten (en eventueel ouders) voor belangrijke gebeurtenissen. Voorbeelden van triggers zijn: een ingediende schadeclaim verandert van status (bv. _Goedgekeurd_ of _Afgewezen_), een nieuwe factuur wordt aangemaakt, een betaling is geregistreerd, of een polis wordt verlengd. Bij zo’n gebeurtenis verstuurt het systeem direct een netjes geformatteerde e-mail naar de betrokken gebruiker(s) met relevante details (bijv. claimnummer, nieuwe status, eventuele vervolgstappen of contactinformatie). Zo blijft de klant op de hoogte zonder steeds te moeten inloggen. Voor ouders moet het systeem ook mails sturen indien zij betrokken zijn (bv. wanneer een claim die zij inzien is geüpdatet). Alle e-mails dienen gepersonaliseerd (met naam, polis/claim details) en in de juiste taal (zie Meertaligheid) verzonden te worden.  
**Labels:** `Feature`, `Notification`, `MVP`

### Feature: In-portal notificaties voor gebruikers

**Omschrijving:** Voeg een **intern notificatiesysteem** toe in de klantenportaal interface. Dit kan bijvoorbeeld in de vorm van een notificatiebel-icoon of meldingenpagina waar nieuwe berichten/statusupdates verschijnen. Wanneer er een relevante gebeurtenis plaatsvindt (zoals bij de e-mailnotificaties beschreven), krijgt de ingelogde klant bovenin beeld een melding, en kan hij in het portal de lijst met ongelezen notificaties bekijken. Bijvoorbeeld: “Uw schadeclaim #12345 is _Goedgekeurd_ op [datum].” of “Er staat een nieuwe factuur voor u klaar.”. De gebruiker kan deze meldingen markeren als gelezen. Dit is vooral handig voor frequente gebruikers van het portaal die real-time op de hoogte willen blijven terwijl ze ingelogd zijn. **Opmerking:** Deze feature is gemarkeerd als _Nice to have_ – e-mail heeft prioriteit; de in-app notificaties kunnen eventueel in een latere fase toegevoegd worden voor extra gemak.  
**Labels:** `Feature`, `Notification`, `Nice to have`

### Feature: Notificatievoorkeuren instellen

**Omschrijving:** Bied gebruikers de mogelijkheid hun **meldingsvoorkeuren** te beheren. Op de profiel- of instellingenpagina kan een klant aangeven welke notificaties hij wil ontvangen en via welk kanaal. Bijvoorbeeld: een klant kan e-mailmeldingen voor elke statuswijziging van een claim **aan of uit** zetten, of kiezen om ook een SMS te krijgen als dat in de toekomst wordt ondersteund. Standaard staan alle essentiële notificaties aan, maar de gebruiker krijgt hiermee controle over de frequentie en het kanaal. Deze voorkeuren moeten uiteraard gerespecteerd worden door het notificatiesysteem (d.w.z. geen mails voor zaken die de klant heeft uitgezet). **Opmerking:** Dit is een _Nice to have_ toevoeging voor gebruiksvriendelijkheid; initieel kan het systeem uitgaan van vaste notificaties voor iedereen.  
**Labels:** `Feature`, `Nice to have`

---

## Epic: Meertaligheid

**Labels:** `Epic`, `Design`  
**Omschrijving:** Deze epic betreft de **meertalige ondersteuning** van het portaal, zodat gebruikers kunnen kiezen in welke taal ze de interface en communicatie willen gebruiken. Aangezien de verzekeringsmakelaar bijvoorbeeld zowel Nederlandstalige als Franstalige klanten heeft, is het cruciaal dat het systeem content in meerdere talen kan weergeven.  
**Sub-features:**

-  **Feature: Meertalige gebruikersinterface (NL/FR)** (MVP)
    
-  **Feature: Taalkeuze en voorkeur per gebruiker** (MVP)
    
-  **Feature: Vertaling van e-mails en notificaties** (MVP)
    
-  **Feature: Ondersteuning extra taal (Engels)** (Nice to have)
    

### Feature: Meertalige gebruikersinterface (NL/FR)

**Omschrijving:** Zorg dat de volledige UI van het portaal beschikbaar is in **twee talen**: Nederlands en Frans (de voornaamste talen van de klanten). Dit houdt in dat alle teksten, labels, knoppen, foutmeldingen en instructies in het portaal vertaald kunnen worden. Gebruik een strategie voor internationalisatie (i18n), bijvoorbeeld resource-bestanden of een database met vertaalde termen, zodat de applicatie eenvoudig kan schakelen tussen talen. De standaardtaal kan Nederlands zijn, maar Franstalige klanten moeten via hun instellingen of voorkeur direct de Frans-talige interface te zien krijgen. Het ontwerp van pagina’s moet bovendien rekening houden met langere of kortere tekst bij vertaling.  
**Labels:** `Feature`, `Design`, `MVP`

### Feature: Taalkeuze en voorkeur per gebruiker

**Omschrijving:** Implementeer een mogelijkheid voor de gebruiker om zijn **taalvoorkeur** in te stellen en te wisselen. Bijvoorbeeld via een taalkeuzemenu (NL/FR vlaggetje of dropdown) ergens in de navigatie of profielinstellingen. De gekozen voorkeur wordt opgeslagen per gebruiker, zodat bij volgende logins automatisch de juiste taal geladen wordt. Nieuwe gebruikers kunnen standaard de taal krijgen op basis van hun browserinstelling of tijdens onboarding een keuze maken. Deze feature verhoogt het gebruiksgemak, omdat elke klant het portaal in de gewenste taal kan ervaren zonder telkens handmatig te moeten schakelen.  
**Labels:** `Feature`, `Design`, `MVP`

### Feature: Vertaling van e-mails en notificaties

**Omschrijving:** Naast de gebruikersinterface zelf moeten ook alle **automatische communicatie** meertalig zijn. Dit betekent dat e-mailtemplates en notificatieberichten (zie Notificatiesysteem) in de taal van de ontvanger verstuurd worden. Voor deze feature worden de teksten van e-mails (bv. bevestiging schadeclaim, notificatie van nieuwe factuur, enz.) vertaald en op basis van de taalinstelling van de klant dynamisch gekozen. Ouders die uitgenodigd worden, krijgen de mail in de taal die de klant opgeeft of een standaardtaal met optie om te wisselen. Hierdoor is niet alleen het portaal zelf, maar ook alle uitgaande communicatie eenduidig voor de gebruiker.  
**Labels:** `Feature`, `Notification`, `MVP`

### Feature: Ondersteuning extra taal (Engels)

**Omschrijving:** Breid de meertaligheidsfunctie uit met ondersteuning voor een **derde taal**, namelijk Engels. Hoewel Nederlands en Frans de primaire talen zijn, kan een Engelse interface nuttig zijn voor expats of internationale klanten. Deze feature omvat het vertalen van alle bestaande content naar het Engels en het toevoegen van “Engels” als optie in de taalkeuze. Dit is aangeduid als _Nice to have_, aangezien de focus eerst op de twee hoofdalen ligt; Engels kan eventueel in een latere fase toegevoegd worden om het klantenbestand uit te breiden of internationale partners toegang te geven tot het portaal.  
**Labels:** `Feature`, `Nice to have`

---

## Epic: Logging & Auditing

**Labels:** `Epic`, `Security`  
**Omschrijving:** In deze epic vallen alle taken rond het **loggen van systeemactiviteiten** en het opzetten van een audit trail. Hiermee kunnen we belangrijke gebeurtenissen in het systeem achteraf traceren, wat zowel nuttig is voor foutopsporing, beveiliging als voor compliance (bv. GDPR-verantwoording).  
**Sub-features:**

-  **Feature: Loggen van beveiligings- en systeemacties** (MVP)
    
-  **Feature: Audit trail voor gegevenswijzigingen** (MVP)
    
-  **Feature: Admin interface voor logbestanden** (Nice to have)
    

### Feature: Loggen van beveiligings- en systeemacties

**Omschrijving:** Implementeer een centraal **loggingsysteem** waarin alle belangrijke acties en gebeurtenissen worden geregistreerd. Voorbeelden zijn: inlogpogingen (geslaagd of mislukt) van gebruikers, uitlog-acties, wachtwoordwijzigingen/resets, aanmaak of verwijdering van gebruikers, veranderingen in polissen (toevoegen, wijzigen, beëindigen) en updates in schadeclaims status. Elk log-item bevat relevantie details (timestamp, gebruikers-ID of systeemaccount, type actie, en eventuele context zoals betrokken polis- of claimnummer). De logs dienen veilig opgeslagen te worden en alleen toegankelijk voor geautoriseerde personen. Real-time logging maakt het mogelijk om eventuele verdachte activiteiten vroegtijdig op te merken en vormt de basis voor auditing.  
**Labels:** `Feature`, `Security`, `MVP`

### Feature: Audit trail voor belangrijke wijzigingen

**Omschrijving:** Breid het logging-systeem uit met een specifieke **audit trail** functie, gericht op het bijhouden van historische wijzigingen in gevoelige gegevens. Dit betekent dat bij elke wijziging van bijvoorbeeld klantgegevens (adres, contactinfo), polisvoorwaarden, of claimuitkeringen, er niet alleen gelogd wordt _dat_ er iets is veranderd, maar ook **wat** de oude en nieuwe waarden zijn en **wie** de wijziging heeft doorgevoerd. Deze audit trail maakt het mogelijk om achteraf precies te reconstrueren welke acties zijn ondernomen en om te voldoen aan compliance-eisen (zoals bewijs van toestemming of wijzigingslog voor auditdoeleinden). In combinatie met de logging van acties geeft dit een volledig controlespoor van gebruik en wijzigingen binnen het systeem.  
**Labels:** `Feature`, `Security`, `MVP`

### Feature: Admin interface voor logbestanden

**Omschrijving:** Ontwikkel een beheerdersweergave (zie ook Administrator Functionaliteiten) voor het **bekijken en doorzoeken van logs en audit trails**. In deze interface kan de beheerder filteren op type gebeurtenis (bijv. alleen beveiligingsincidenten, of alleen gegevenswijzigingen), op datum, of op specifieke gebruikers. De resultaten worden leesbaar gepresenteerd, met mogelijkheid om details van een logregel uit te klappen. Eventueel kan de beheerder vanuit hier ook een **export** of rapportage genereren voor audits. Deze feature sluit aan op de eerdergenoemde _Beveiligingslogboek inzage (admin)_ en zorgt ervoor dat de rijke logging die in de achtergrond is verzameld ook daadwerkelijk nuttig gebruikt kan worden door het bedrijf. Omdat dit meer een luxe is voor interne efficiëntie en audits, is deze aangemerkt als _Nice to have_.  
**Labels:** `Feature`, `Nice to have`

---

## Epic: Performance & Hosting

**Labels:** `Epic`, `Performance`  
**Omschrijving:** Deze epic richt zich op de **prestaties, schaalbaarheid en hosting** van het klantenportaal. Het omvat activiteiten om de applicatie vlot en betrouwbaar te laten werken voor alle gebruikers, en om een robuuste infrastructuur neer te zetten die productiegebruik aankan. Denk aan server-setup, optimalisaties en monitoring.  
**Sub-features:**

-  **Feature: Infrastructuur & hosting inrichting** (MVP)
    
-  **Feature: Prestatie-monitoring (metrics & tracing)** (MVP)
    
-  **Feature: Belastingstesten en optimalisatie** (Nice to have)
    
-  **Feature: Schaalbaarheid & auto-scaling** (Nice to have)
    

### Feature: Infrastructuur & hosting inrichting

**Omschrijving:** Zet de benodigde **hosting-infrastructuur** op voor het draaien van het portaal in productie. Kies een geschikte omgeving (bv. een cloudplatform als Azure/AWS of on-premise server) en configureer alle componenten: webserver/backend, database, opslag voor documenten, etc. Hierbij horen ook het opzetten van omgevingsspecifieke configuraties (voor test, acceptatie, productie), beveiligingsinstellingen op serverniveau (firewalls, SSL-certificaten voor HTTPS), en het inrichten van een CI/CD-pijplijn voor automatische deployment. Deze feature is cruciaal (MVP), want het bepaalt hoe en waar de applicatie zal draaien en zorgt dat updates snel en betrouwbaar live kunnen.  
**Labels:** `Feature`, `Performance`, `MVP`

### Feature: Prestatie-monitoring (metrics & tracing)

**Omschrijving:** Implementeer voorzieningen voor **monitoring** van de applicatieperformantie en -gezondheid. Dit houdt in: het verzamelen van **metrics** (zoals responstijden van pagina’s/API-calls, server CPU/geheugenverbruik, aantal actieve gebruikers, enz.) en het opzetten van **distributed tracing** voor belangrijke transacties (bijvoorbeeld een volledige schademelding flow traceren over verschillende services). Gebruik eventueel bestaande APM-tools of frameworks (OpenTelemetry, Application Insights, etc.) om deze data te verzamelen en visualiseren. Door monitoring vanaf de start in te bouwen (MVP) kan het team proactief problemen opsporen, bottlenecks identificeren en zorgen voor een soepel draaiend portaal. Bovendien helpt het bij het aantonen dat SLA’s behaald worden.  
**Labels:** `Feature`, `Performance`, `MVP`

### Feature: Belastingstesten en optimalisatie

**Omschrijving:** Voer periodieke **load- en stresstesten** uit op het portaal om te verzekeren dat het ook bij hogere gebruikersaantallen en dataverkeer responsief blijft. Simuleer met testscenario’s bijvoorbeeld een piek waarbij tientallen of honderden klanten gelijktijdig inloggen of een schadeclaim indienen. Analyseer de resultaten om eventuele performance knelpunten op te sporen (bijv. trage database queries, onvoldoende server resources). Optimaliseer waar nodig de code en infrastructuur: caching implementeren voor veelgebruikte data, database-indexen toevoegen, compressie van data inschakelen, etc. Deze continue verbeteringen vallen onder _Nice to have_ – ze zijn minder direct zichtbaar voor de gebruiker, maar zeer waardevol voor de stabiliteit op lange termijn.  
**Labels:** `Feature`, `Nice to have`

### Feature: Schaalbaarheid & auto-scaling

**Omschrijving:** Bereid het systeem voor op **schaalvergroting**. Deze feature omvat het configureren van **auto-scaling** op de hostingomgeving zodat bij groeiend gebruik extra resources automatisch toegewezen worden (bijv. extra webserver instances bij hoge load). Ook hoort hierbij het gebruik van een load balancer om verkeer te verdelen en eventueel het opsplitsen van diensten (microservices of aparte componenten) als dat de performance ten goede komt. Hoewel het portaal in eerste instantie wellicht op één server draait, is het toekomstbestendig maken ervan middels schaalbaarheid een _Nice to have_ die grote waarde krijgt zodra het klantenbestand groeit. Het voorkomt tevens downtime tijdens piekbelasting en draagt bij aan een hoog beschikbare dienst.  
**Labels:** `Feature`, `Nice to have`

---

## Epic: Facturatie en betalingen

**Labels:** `Epic`, `Payment`  
**Omschrijving:** Deze epic beslaat alle functies gerelateerd aan **facturen en betalingen** binnen het portaal. Klanten moeten inzage krijgen in openstaande en betaalde facturen en de mogelijkheid hebben om online betalingen te verrichten. De makelaar (admin) moet op zijn beurt facturen kunnen beheren en de betalingen kunnen opvolgen.  
**Sub-features:**

-  **Feature: Factuuroverzicht voor klanten** (MVP)
    
-  **Feature: Online betaling integratie** (Nice to have, **Blocked**)
    
-  **Feature: Betaalstatus en bevestiging** (Nice to have)
    
-  **Feature: Factuurbeheer (admin)** (MVP)
    

### Feature: Factuuroverzicht voor klanten

**Omschrijving:** Integreer een **Facturen**-pagina in het klantenportaal waar de klant al zijn facturen kan zien. Voor elke factuur wordt weergegeven: factuurnummer, datum, beschrijving (bijv. “Premie verzekering X - periode Y”), bedrag, vervaldatum en **status** (open, betaald, te laat). De klant moet de factuur kunnen openen/bekijken voor meer details en eventueel een PDF-download (officieel factuurdocument) kunnen ophalen. Dit overzicht geeft klanten transparantie over wat ze verschuldigd zijn of al betaald hebben en vervangt papieren facturen of aparte e-mails.  
**Labels:** `Feature`, `Payment`, `MVP`

### Feature: Online betaling integratie

**Omschrijving:** Maak het voor klanten mogelijk om openstaande facturen **online te betalen** via het portaal. Bij elke openstaande factuur in het overzicht komt een “Betaal nu” knop, die de klant naar een beveiligde betaalpagina leidt. Integreer een betalingsprovider (bijv. iDEAL, Bancontact, creditcard via een PSP als Mollie/Stripe) om transacties af te handelen. Na een geslaagde betaling wordt de factuurstatus automatisch bijgewerkt naar _Betaald_ en ontvangt de klant een bevestiging (zie ook notificaties). **Opmerking:** Deze feature is gemarkeerd als _Blocked_ totdat de keuze en configuratie van de betaalprovider is afgerond (externe afhankelijkheid). Het is functioneel een _Nice to have_ voor de eerste versie – initieel kunnen klanten eventueel nog via traditionele methoden betalen, maar het is gepland om snel toe te voegen voor gemak en tijdige betaling.  
**Labels:** `Feature`, `Payment`, `Nice to have`, `Blocked`

### Feature: Betaalstatus en bevestiging

**Omschrijving:** Zorg voor automatische verwerking van **betaalstatussen** en het genereren van bevestigingen. Wanneer een klant via het portaal heeft betaald (zie online betaling integratie), moet het systeem de terugkoppeling van de payment provider verwerken: is de betaling gelukt of mislukt? Bij succesvolle betaling: markeer de factuur als betaald, leg de betaaldatum vast en genereer een **betalingsbevestiging** (bijv. een e-mail of een PDF-ontvangstbewijs in het portaal). Bij mislukte betaling of annulering: laat de factuur open staan maar noteer de poging in het systeem en toon eventueel een melding aan de klant dat de betaling niet is doorgegaan. Deze feature sluit aan bij de betalingsintegratie en zorgt voor een sluitende administratie en feedback naar de gebruiker.  
**Labels:** `Feature`, `Payment`, `Nice to have`

### Feature: Factuurbeheer (admin)

**Omschrijving:** Ontwikkel functies voor beheerders om **facturen te beheren** in het portaal. Dit omvat het aanmaken van nieuwe facturen voor klanten (bijvoorbeeld wanneer een nieuwe polis of verlenging wordt gestart, of periodiek premies in rekening worden gebracht). De beheerder voert factuurdetails in: klant/polis koppeling, bedrag, omschrijving, vervaldatum, etc., en kan de factuur publiceren naar het portaal (waardoor de klant hem ziet). Ook kunnen beheerders de status van facturen inzien (bijv. welke zijn al betaald, welke zijn nog open of verlopen) en handmatig aanpassen indien nodig (bijvoorbeeld een betaling registreren die offline binnenkwam of een factuur annuleren). Tenslotte kunnen ze via het systeem een factuur opnieuw versturen per e-mail of een herinnering uitsturen voor openstaande betalingen. Met dit beheeronderdeel kan de makelaar de financiële kant binnen het klantenportaal bijhouden en ingrijpen waar nodig.  
**Labels:** `Feature`, `Payment`, `MVP`

---

**Opmerking:** Bovenstaande opsomming van issues zorgt voor een modulaire structuur. Elke _Epic_ kan ingepland worden in een of meerdere sprints en de onderliggende _Feature_-issues zijn klein genoeg om afzonderlijk opgepakt te worden. Dit maakt het ontwikkelproces beheersbaar en het voortschrijdend inzicht eenvoudig te verwerken (er kan per epic of feature geschoven en geprioriteerd worden). Labels als `MVP` helpen om te focussen op de minimaal vereiste functionaliteit voor een eerste release van het portaal, terwijl `Nice to have` aangeeft welke uitbreidingen later opgepakt kunnen worden. `Blocked` markeert afhankelijkheden of externe voorwaarden die opgelost moeten worden voordat die feature uitgevoerd kan worden. Deze structuur dient als leidraad om alle gebruikersvereisten traceerbaar om te zetten in development taken.
