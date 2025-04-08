### A Pinterest-inspired app developed in ASP.NET/C#, using MVC architecture and a SQL Server database.
- It lets the authenticated user share photos, videos, GIFs, and comments, as well as like and save posts to personalized collections/boards on their profile. They can also edit or delete their previous posts, comments or collections/boards. 
- A guest user can see posts and comments from authenticated users, but they can't post, comment, like or have their own collections/boards. 
- The admin user can only delete posts or comments from any authenticated user.

### A demonstration provided for each type of user:
### Guest user
https://github.com/BiancaDogareci/Social_Bookmarking_Web_Application/assets/119197457/6cc14787-a29f-4b20-a0c3-d22f2622c8c8

### Authenticated user
https://github.com/BiancaDogareci/Social_Bookmarking_Web_Application/assets/119197457/bdb513b8-0673-4d5d-8705-c6712c2f565d

### Admin user
https://github.com/BiancaDogareci/Social_Bookmarking_Web_Application/assets/119197457/abe527ed-4ee7-4cf6-994b-708b4644c30f


## Definirea obiectivului

Proiectul are ca scop testarea unitară a serviciilor backend dezvoltate în cadrul unei aplicații de tip social bookmarking, similară cu Pinterest. Serviciile vizate sunt componente esențiale ale aplicației, precum: 
- autentificarea unui utilizator ca USER sau ADMIN;
- crearea, editarea, ștergerea pin-urilor;
- afișarea paginată a pin-urilor ordonate după like-uri, in funcție de search;
- afișarea paginată a pin-urilor ordonate după dată, in funcție de search;
- toggle pin like;
- crearea, editarea, ștergerea categoriilor;
- salvarea unui pin într-o categorie;
- adăugarea, editarea și ștergerea de comentarii.

Pentru implementarea și testarea proiectului, au fost utilizate tehnologii precum C#, ASP.NET Core pentru backend, xUnit ca framework de testare și Moq pentru simularea dependențelor.


## Alegerea framework-ului de testare C# - XUnit

Pentru testarea aplicației noastre, am ales framework-ul xUnit deoarece oferă o serie de avantaje adaptate proiectelor moderne dezvoltate în ASP.NET versiunea 6+.

### Avantajele:
1. Integrare excelentă cu .NET Core
- xUnit este dezvoltat de aceeași echipă care a lucrat la ASP.NET Core și .NET Core;
- Funcționează perfect cu SDK-urile moderne: .NET 6, .NET 7, .NET 8);
2. Design modern și clar
- Setup-ul testelor se face simplu;
- Codul de testare e mai curat și mai ușor de înțeles;
3. Teste rulate în paralel
- xUnit rulează testele în paralel automat, ceea ce ajută la reducerea timpului total de execuție;
- Este util pentru proiectul nostru, deoarece avem destul de multe teste;
4. Suport bun
- Este compatibil cu multe tool-uri moderne (ex: Moq, dotnet test etc.);

### De ce NU am ales NUnit?
- Este mai potrivit pentru aplicații legacy sau foarte complexe, care folosesc deja NUnit;
- Necesită mai multă configurare și oferă suport avansat pentru testare parametrică, de care nu aveam nevoie în acest proiect;
- Structura cu multe atribute face codul mai greu de citit comparativ cu xUnit;

### De ce NU am ales MSTest?
- Este mai vechi.
- Comparativ cu xUnit, este mai puțin flexibil și mai greu de extins;

## Dependențe

### Pinterest.csproj:
```
<!-- Pentru aplicatie ASP.NET Core -->
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UserSecretsId>aspnet-Pinterest-0870c269-c934-441d-a61d-2ff3c9d6286c</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <!-- Diagnostica erori EF Core in development -->
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="6.0.22" />
    
    <!-- ASP.NET Identity integrat cu EF Core (user management) -->
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="6.0.22" />
    
    <!-- Interfata grafica pentru autentificare (login/register UI) -->
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="6.0.22" />
    
    <!-- Conectare la SQL Server -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.22" />
    
    <!-- Unelte pentru migratii EF Core -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="6.0.22" />
  </ItemGroup>

</Project>
```

### Pinterest.Tests.csproj:
```
<!-- Pentru proiect de testare unitara -->
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <!-- Ruleaza testele .NET -->
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    
    <!-- Framework de testare xUnit -->
    <PackageReference Include="xunit" Version="2.5.3" />
    
    <!-- Runner pentru xUnit in Visual Studio / CLI -->
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
    
    <!-- Code coverage collector (coverlet) -->
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
    
    <!-- Mocking library pentru testare unitara -->
    <PackageReference Include="Moq" Version="4.20.72" />
  </ItemGroup>

  <ItemGroup>
    <!-- Leaga proiectul de testare de aplicatia principala -->
    <ProjectReference Include="..\Pinterest\Pinterest.csproj" />
  </ItemGroup>

</Project>
```


## Configurarea și rularea testelor unitare
### 1. Rularea testelor din terminal cu "dotnet test"
- Deschide terminalul în folderul soluție.
- Rulează comanda:
```
dotnet test
```
- Vei vedea in consolă:
    - Testele care au trecut sau eșuat;
    - Timpul de execuție;

### 2. Rularea testelor din IDE
- Deschide fișierul de test în Visual Studio sau Rider.
- Lângă fiecare [Fact] (test individual) sau clasă, apare un buton verde.
- Apasă-l pentru a rula un test individual sau toate testele din clasă.
- Poți folosi și panoul de testare (Test Explorer în Visual Studio, Unit Tests în Rider) pentru:
    - Vizualizare rapidă a rezultatelor;
    - Re-run, debug sau filtrare a testelor.

### Rularea Testelor Demo:

## Strategii de testare - Teorie + Exemple

Funcția GetPins(string search, int page, int perPage) - returnează o listă paginată de pin-uri, filtrată opțional după search, ordonată descrescător după LikesCount.

### 1. Testare funcțională

### a) Partiționare de echivalență
Ideea de bază este de a partiționa domeniul problemei (datele de intrare) în clase de echivalență astfel încât, din punctul de vedere al specificației datele dintr-o clasă sunt tratate în mod identic.

| Parametru |   Clasă validă   | Clasă invalidă |
|-----------|------------------|----------------|
| search    | "", "text", null |       X        |
| page      |       > 0        |      <= 0      |
| perPage   |       > 0        |      <= 0      |


### b) Analiza valorilor de frontieră
Analiza valorilor de frontieră este folosită de obicei împreuna cu partiționarea de echivalență. Ea se concentrează pe examinarea valorilor de frontieră ale claselor, care de obicei sunt o sursă importanta de erori.

Spre exemplu:
- avem m pin-uri în total;
- folosim ```perPage``` pentru a decide câte pin-uri apar pe o pagină;
- numărul total de pagini este ```n = ceil(m / perPage)```;

#### Parametrul "page":

| Caz testat                         | Valoare   | Este valid? | Observație                                                                 |
|-----------------------------------|-----------|-------------|----------------------------------------------------------------------------|
| Valoare imediat sub limita minimă | `0`       | Nu          | Index invalid – paginile încep de la 1                                     |
| Valoare minimă validă             | `1`       | Da          | Prima pagină                                                               |
| Valoare maximă validă             | `n`       | Da          | Ultima pagină (ex: dacă m = 5 și perPage = 2, atunci n = 3)               |
| Valoare imediat peste maxim       | `n + 1`   | Nu          | Pagină inexistentă – nu ar trebui să returneze rezultate                   |


#### Parametrul "perPage":

| Caz testat        | Valoare | Este valid? | Observație                                             |
|-------------------|---------|-------------|--------------------------------------------------------|
| Valoare negativă  | < 0     | Nu          | Nu se pot cere pagini cu număr negativ de elemente     |
| Valoare zero      | 0       | Nu          | Ar produce o diviziune la 0 sau rezultat gol           |
| Valoare pozitivă  | > 0     | Da          | Comportament așteptat                                  |



### c) Partiționarea în categorii
Această metodă se bazează pe cele două anterioare. Ea caută să genereze date de test care “acoperă" funcționalitatea sistemului și maximizeaza posibilitatea de găsire a erorilor.

| Parametru | Categorie                     | Alternative (valori reprezentative)                                                                 |
|-----------|-------------------------------|------------------------------------------------------------------------------------------------------|
| `search`  | conținutul filtrului          | 1. gol (`""`) <br> 2. valid (e.g. `"test"`) <br> 3. nu există rezultate                              |
| `page`    | validitatea paginii           | 1. `1` (minim valid) <br> 2. `n` (ultimul valid) <br> 3. `n+1` (invalid peste limită) <br> 4. `0`    |
| `perPage` | validitatea numărului pe pagină | 1. `> 0` (valid) <br> 2. `= 0` (invalid) <br> 3. `< 0` (invalid)                                     |
| DB        | numărul total de pinuri (`m`) | 1. `0` (empty) <br> 2. `< perPage` <br> 3. exact multiplu de `perPage` <br> 4. non-multiplu de `perPage` |


#### Constrângeri:
- dacă ```m = 0```, pagina trebuie să fie 1, ```search``` poate fi orice;
- dacă ```perPage <= 0```, rezultatul este listă goală indiferent de ```page```;
- dacă ```page > ceil(m/perPage)```, se returnează lista goală;
- daca ```pag <= 0```, se returnează lista goală;

#### Testele:

| Nr.  | `search`     | `page` | `perPage` | `m` | Așteptare            |
|------|--------------|--------|-----------|-----|-----------------------|
| T1   | `""`         | 1      | 2         | 0   | pagină goală          |
| T2   | `""`         | 1      | 2         | 5   | 2 elemente            |
| T3   | `""`         | 3      | 2         | 5   | 1 element             |
| T4   | `""`         | 4      | 2         | 5   | listă goală           |
| T5   | `""`         | 0      | 2         | 5   | invalid, listă goală  |
| T6   | `""`         | 1      | 0         | 5   | invalid, listă goală  |
| T7   | `""`         | 1      | -2        | 5   | invalid, listă goală  |
| T8   | `"test"`     | 1      | 2         | 4   | rezultate filtrate    |
| T9   | `"noresult"` | 1      | 2         | 0   | listă goală           |


### 2. Testare structurală

### Graful de flux de control (Control Flow Graph - CFG)

```
public (IEnumerable<Pin> Pins, int LastPage, string PaginationUrl) GetPins(string search, int page, int perPage)
{
    IQueryable<Pin> pins = string.IsNullOrWhiteSpace(search)
        ? _repo.GetAllPinsOrderedByLikes()
        : _repo.GetPinsBySearch(search);

    int totalItems = pins.Count();

    if (perPage <= 0 || page <= 0)
    {
        return (new List<Pin>(), 0, string.IsNullOrWhiteSpace(search)
            ? "/Pins/Index/?page"
            : $"/Pins/Index/?search={search}&page");
    }

    int offset = (page - 1) * perPage;

    var paginatedPins = pins.Skip(offset).Take(perPage).ToList();
    int lastPage = (int)Math.Ceiling((float)totalItems / perPage);
    string url = string.IsNullOrWhiteSpace(search)
        ? "/Pins/Index/?page"
        : $"/Pins/Index/?search={search}&page";

    return (paginatedPins, lastPage, url);
}
```

![CFG](assets/CFG.png)

### Instrucțiuni cheie:

- **0**: `start`
- **1**: `if (string.IsNullOrWhiteSpace(search))`
- **2**: `GetAllPinsOrderedByLikes`
- **3**: `GetPinsBySearch`
- **4**: `totalItems = pins.Count`
- **5**: `if (perPage <= 0 || page <= 0)`
- **6**: `return early (empty list, page = 0)`
- **7**: `offset = (page - 1) * perPage`
- **8**: `paginatedPins = Skip + Take`
- **9**: `lastPage = ceiling(totalItems / perPage)`
- **10**: `if (string.IsNullOrWhiteSpace(search))`
- **11**: `return final tuple`
- **12**: `end`

### a) Acoperire la nivel de instrucțiune (Statement coverage)

| Test | Input                                      | Descriere                                 |
|------|--------------------------------------------|--------------------------------------------|
| TC1  | `search = null, page = 1, perPage = 2`     | Ramura normală fără search                |
| TC2  | `search = "tag", page = 1, perPage = 2`    | Ramura normală cu search                  |
| TC3  | `search = null, page = 0, perPage = 2`     | `page <= 0 → return empty`                |
| TC4  | `search = null, page = 1, perPage = 0`     | `perPage <= 0 → return empty`             |


### b) Acoperire pe ramuri (Branch coverage)
#### Ramuri:
- ```if (perPage <= 0 || page <= 0)``` → true și false
- ```if (string.IsNullOrWhiteSpace(search))``` → true și false

### c) Acoperire pe condiții (Condition coverage)

Pentru ```if (perPage <= 0 || page <= 0)```, sunt două condiții:
- ```perPage <= 0```;
- ```page <= 0```;

Trebuie:
- ```perPage = 0```, ```page = 1``` → true
- ```perPage = 1```, ```page = 0``` → true
- ```perPage = 1```, ```page = 1``` → false

Pentru string.IsNullOrWhiteSpace(search):
- ```search = null``` → true
- ```search = ""``` → true
- ```search = "abc"``` → false

### d) Acoperire la nivel de condiție/decizie (Condition/Decision Coverage - C/DC)
Fiecare condiție și decizie acoperită:
- TC1: ```search = null```, valid ```page/perPage``` -> true
- TC2: ```search = “tag"``` -> true
- TC3: ```page = 0``` -> false
- TC4: ```perPage = 0``` -> false
- TC5: ```search = ""``` (string goală) -> true

### e) Acoperirea la nivel de condiții multiple (Multiple Condition Coverage)

Pentru ```if (perPage <= 0 || page <= 0)```:
| perPage | page | Rezultat |
|---------|------|----------|
| 0       | 1    | TRUE     |
| 1       | 0    | TRUE     |
| 1       | 1    | FALSE    |

### f) Modified Condition/Decision Coverage - MC/DC

- ```perPage = 0```, ```page = 1``` → afectează decizia
- ```perPage = 1```, ```page = 0``` → afectează decizia
- ```perPage = 1```, ```page = 1``` → niciuna nu declanșează decizia → false

Similar pentru ```string.IsNullOrWhiteSpace(search)```.

### g) Testarea circuitelor independente + Complexitate ciclomatică (McCabe)

Formulă: V(G) = E - N + 2
- Noduri: 8;
- Muchii: 10;

V(G) = 10 - 8 + 2 = 4

Avem 3 circuite independente:
- ```perPage``` invalid;
- ```page``` invalid;
- ```search``` valid + paginare validă;


### h) Acoperire pe căi (Path Coverage)

| Cale | Descriere                                               |
|------|----------------------------------------------------------|
| P1   | `perPage = 0` → return early                             |
| P2   | `page = 0` → return early                                |
| P3   | `search` valid → pins by search → paginare ok            |
| P4   | `search = null` → pins by likes → paginare ok            |
| P5   | `search = ""` → tratat ca null → pins by likes           |

## Raport despre folosirea unui tool AI (ChatGPT)

## Referințe:

[1] Robert Dennyson, [Choosing the Right Testing Framework for .NET Applications](https://medium.com/@robertdennyson/xunit-vs-nunit-vs-mstest-choosing-the-right-testing-framework-for-net-applications-b6b9b750bec6), Data ultimei accesări: 6 aprilie 2025

[2] Tong Eric, [Guide to Implementing xUnit Tests in C# .NET](https://medium.com/bina-nusantara-it-division/a-comprehensive-guide-to-implementing-xunit-tests-in-c-net-b2eea43b48b), Data ultimei accesări: 6 aprilie 2025

[3] [XUnit Guide Official Documentation](https://xunit.net/)

[4] [Understanding Functional Testing](https://www.opentext.com/what-is/functional-testing)

[5] [Understanding The Basic Concept Of Structural Testing](https://unstop.com/blog/structural-testing)


