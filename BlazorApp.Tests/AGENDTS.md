### 📜 Jouw **allereerste** prompt — letterlijk

> \*\*Denk er mee over naa hoe ik dit kan omzetten naar een test project, dat ik met dotnet test kan uitvoeren:
>
> > Wat ik eigenlijk wil is dat de testreuslatten om een meer native manier terugkomen. Allerliefst zou ik willen dat elke check een andere test wordt in mijn testproject en dat ik ze dynamisch kan aanvullen, maar ik weet niet of dat haalbaar is.
> > Alternatief zou zijn om in de testfunctie niks extra te importen, buiten de functies die ik toelaat, zoals FillAsync, Click enzo
> > FLOWS.JSON\*\*
> >
> > ```json
> > [
> >   {
> >     "name": "Login and Claims",
> >     "steps": [
> >       { "name": "Open login", "type": "GotoAsync", "data": "http://localhost:5228/Account/Login" },
> >       { "name": "Fill login username", "type": "FillAsync", "selector": "input[name='Input.UserName']", "data": "user" },
> >       … (meer stappen)
> >   }
> > ]
> > ```
> >
> > **=== namespace UiFlowRecorder; …**
> > *(daarna ± 800 regels C#-code met Config, FlowStepBuilder, ScreenshotService, Program.cs, Uploader, Markdown-builder, enz.)*

Je eindigde je eerste bericht dus met **de complete bestaande console-app** (Playwright flow-runner) plus de JSON-flow die je wilde ombouwen naar een *echt* **dotnet test** project.

---

## 🧠  Mijn *publieke* redenering — gedetailleerd chronologisch

> *(Dit is géén “innerlijke chain-of-thought”, maar een transparante reconstructie van de stappen en design-beslissingen die ik expliciet heb gedeeld of tot op hoofdlijnen heb gemaakt.)*

| #  | Stap / reflectie              | Wat ik besloot & waarom                                                                                                                                                                                   |
| -- | ----------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1  | **Doel scherpstellen**        | Je wilde: ① tests draaien via `dotnet test`; ② elke stap of check als aparte test; ③ Playwright blijven gebruiken; ④ screenshots + Markdown-rapport behouden; ⑤ minimale extra imports in de testmethode. |
| 2  | **Testframework kiezen**      | Tussen MSTest, NUnit, xUnit koos ik **xUnit**: modern, dependency-injection via `IClassFixture`, goed Playwright-compatibel, standaard in VS 2022.                                                        |
| 3  | **Structuur bedenken**        | Hiërarchie ➜ *Root → Group → Job/Flow → Step → Check*; zo kun je op elk niveau ✅/❌ bepalen en bubbelt `Fail()` omhoog.                                                                                    |
| 4  | **Screenshots automatiseren** | `UiStepAttribute` vóór iedere stap: screenshot **before** → actie (uit de `[Fact]` zelf) → screenshot **after**. Resultaat + checks in één `StepNode`.                                                    |
| 5  | **Boom in geheugen**          | `ResultNode`‐basisklasse met `Children`, `Parent`, `Success`; concrete records `RootNode`, `GroupNode`, `JobNode`, `StepNode`.                                                                            |
| 6  | **Contextobject**             | `UiRunState` (static) houdt vast: huidige Job, Step-naam, MD5-before, lijst met `CheckResult`, singleton `Root`.                                                                                          |
| 7  | **Markdown-rapport**          | `ReportWriter` (collection-fixture) schrijft na test-run een `.md` waarin via recursie de boom wordt uitgelogd, met voor/na-screenshots en mislukte checks.                                               |
| 8  | **Async-naar-sync helper**    | `ScreenshotService.Capture()` = sync wrapper rond `CaptureAsync`, zodat we screenshots kunnen nemen in attribute-hooks (die niet async mogen zijn).                                                       |
| 9  | **FLOWS.JSON ↔ tests**        | Elke stap wordt één `[Fact]` in een klasse (Job). Je kunt die handmatig schrijven of met een Roslyn Source-generator uit de JSON laten genereren.                                                         |
| 10 | **Iteratief compileren**      | Bij elke build-error die jij meldde heb ik: *missing using*, *toegankelijkheid*, *dubbele Node-definitie*, *MD5-veldnaam*, *Microsoft.NET.Test.Sdk* toegevoegd.                                           |
| 11 | **Playwright browsers**       | `Playwright.InstallAsync()` bleek obsoleet → instructie “run `playwright install`” of pre-build stap.                                                                                                     |
| 12 | **Testhost-fout**             | Bij “Could not find testhost” → uitleg dat `Microsoft.NET.Test.Sdk` ontbreekt; waarom versie 17.9 nodig is bij `net8.0`.                                                                                  |
| 13 | **Laatste polijsten**         | LINQ-usings toegevoegd, `AsyncLocal<List<CheckResult>>` pattern, `CurrentStepChecks.Value ??=` fix, rapportcode omgezet naar `ResultNode`.                                                                |
| 14 | **Beheer-tips**               | Voor de “AI-bot” die jouw tests aanpast: grep op ❌ in Markdown; voor jou: Git diff op JSON; waarom xUnit-fixtures mooi parallel/sérieel te schakelen zijn.                                                |

---

## 📂 Overzicht van de uiteindelijke onderdelen

1. **UiFlow\.Tests.csproj**

   * `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Microsoft.Playwright`
2. **TestInfra/**

   * `PlaywrightFixture.cs`
   * `ScreenshotService.cs`
   * `ResultNode.cs` (Root/Group/Job/Step) + `CheckResult`
   * `UiRunState.cs`
   * `UiStepAttribute.cs` (+ optioneel `UiCheckAttribute.cs`)
   * `ReportWriter.cs`
3. **Flows/**

   * Eén klasse per Job, bv. `LoginFlow.cs` met `[Fact]`-methoden
4. **UiFlowRecorder** (class-library)

   * Bevat je oorspronkelijke Flow-builder, uploader, enz.
5. **Output**

   * `UiReport.md` + PNG’s in artifacts-map → CI artefact.

---

## 🚀 Resultaat

* `dotnet test` meldt gewone TRX/JUnit én schrijft Markdown.
* VS Test Explorer toont per stap een test-case.
* Screenshots before/after + mislukte checks zichtbaar in rapport.
* Structuur leent zich voor een AI-loop die JSON uitbreidt totdat root ✅ is.

---

### 🔚 TL;DR

Je eerste prompt bestond uit een grote Playwright-console-app + JSON-flow;
mijn redenering was: **xUnit - fixtures - attribute hooks - in-memory boom - Markdown - iteratief bouwfouten fixen**, totdat jij een werkend `.NET 8` testproject met before/after-screenshots en hiërarchisch rapport had.

---

### 🚀 Wat hebben we nu precies in handen — en hoe toon je dat aan?

| 🔧 Artefact                | Functie                                                                                                                          | Hoe verifiëren?                                                                                       |
| -------------------------- | -------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- |
| **`UiFlow.Tests` project** | Volledig compileerbaar .NET 8 /xUnit-testproject met Playwright-fixture, `UiStepAttribute`, hiërarchische boom & Markdown-export | `dotnet build` → 0 errors<br>`dotnet test --list-tests` → toont al je stappen als afzonderlijke tests |
| **Screenshots (PNG’s)**    | *before* & *after*-beeld per stap                                                                                                | Na testrun in `UiFlow.Tests/bin/Debug/net8.0/` of je ingestelde artifacts-map                         |
| **`UiReport.md`**          | Visueel eindrapport met ✅/❌ op elk niveau + screenshots                                                                          | Open het gegenereerde Markdown-bestand in VS Code / GitHub / Markdown-viewer                          |
| **TRX / JUnit log**        | Standaard `dotnet test` output voor CI                                                                                           | `TestResults/XXXX.trx` → importeerbaar in Azure DevOps, GitHub Actions, Jenkins…                      |

---

## 1️⃣ Build-en & testen

```bash
# in de solution-root
dotnet restore           # haalt alle NuGets binnen
dotnet build             # alles compileert
dotnet test UiFlow.Tests # voert Playwright-stappen uit
```

**Te zien in de console:**

```
Test Run Successful.
Total tests: 2
     Passed: 2
 Total time: 7,8169s

📝 UiReport.md geschreven.
📸 Login_Open login_before.png (MD5 …)
📸 Login_Open login_after.png  (MD5 …)
…
```

---

## 2️⃣ Test Explorer-view (VS 2022)

```
 🧪 UiFlow.Tests
 └─ LoginFlow
    ├─ Open login           ✔️
    └─ Assert page change   ✔️
```

Elke stap is klik-baar; bij falen zie je de exception (bijv. selector niet gevonden).

---

## 3️⃣ Inhoud UiReport.md (voorbeeld)

```markdown
# 🧪 UI Flow Verslag

**✅ Alles geslaagd**

## ✅ Login

### ✅ 1. Open login

**Voor:**
![before](Login_Open login_before.png)

**Na:**
![after](Login_Open login_after.png)

### ✅ 2. Assert page change

**Voor:**
![before](Login_Assert page change_before.png)

**Na:**
![after](Login_Assert page change_after.png)
```

Open dit bestand → meteen visueel bewijs (inclusief screenshots) dat elke stap gelukt is.

---

## 4️⃣ Screenshot-bestanden

Bestandsstructuur:

```
artifacts/
└─ Login/
   ├─ Open login_before.png
   ├─ Open login_after.png
   ├─ Assert page change_before.png
   ├─ Assert page change_after.png
   └─ UiReport.md
```

Je kunt iedere PNG openen ter handmatige verificatie of in CI uploaden als artifact.

---

## 5️⃣ CI-gebruiks­scenario

```yaml
- name: build & test
  run: dotnet test UiFlow.Tests --logger "trx"

- name: upload artifacts
  uses: actions/upload-artifact@v4
  with:
    name: ui-report
    path: |
      UiFlow.Tests/**/UiReport.md
      UiFlow.Tests/**/*.png
      UiFlow.Tests/TestResults/*.trx
```

* In CI-console zie je het xUnit-resultaat.
* Via de artifacts kun je het Markdown-rapport en screenshots downloaden.

---

### 🎯 Eindresultaat — kort samengevat

1. **Compilatie-clean** .NET 8-oplossing.
2. **Playwright-gestuurde UI-tests** als first-class xUnit-cases.
3. **Automatische screenshots & Markdown-rapport** per stap.
4. **Standaard Test SDK-uitvoer** voor elke CI-tool.

Zo kun je zwart-op-wit én visueel aantonen dat de flow werkt — of exact zien waar hij faalt. Veel testplezier! 🎉
