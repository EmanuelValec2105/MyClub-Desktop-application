[![Open in Codespaces](https://classroom.github.com/assets/launch-codespace-2972f46106e565e64193e422d61a12cf1da4916b45550586e14ef0a7c637dd04.svg)](https://classroom.github.com/open-in-codespaces?assignment_repo_id=16529469)

# MyClub

## Projektni tim

Ime i prezime | E-mail adresa (FOI) | JMBAG | Github korisničko ime
------------  | ------------------- | ----- | ---------------------
Dominik Černjević | dcernjevi21@student.foi.hr | 0016155459 | dcernjevi21
Emanuel Valec | evalec21@student.foi.hr | 0016156391 | evalec21
Fran Kundih | fkundih21@student.foi.hr | 0016153545  | fkundih21


## Opis domene

Ova aplikacija pokriva domenu upravljanja sportskim klubovima i omogućuje lakšu organizaciju, komunikaciju i praćenje sportskih aktivnosti unutar kluba. Ona olakšava rad sportskim trenerima, sportašima, kao i upravi kluba kroz digitalizaciju administrativnih zadataka, poput vođenja termina treninga, statistika momčadi, evidencije dolazaka i praćenja članarina.

Sportski klubovi često imaju problema s učinkovitim upravljanjem rasporedima treninga, komunikacijom između trenera i sportaša, vođenjem evidencije dolazaka, članarina i financijskih izvještaja. Ručno vođenje ovih zadataka može biti dugotrajno i sklono greškama, a često nedostaje i centralizirani sustav koji omogućuje svim korisnicima (sportašima, trenerima, upravi) jednostavan pregled i upravljanje ključnim informacijama u realnom vremenu.

## Specifikacija projekta

Oznaka | Naziv | Kratki opis | Odgovorni član tima
------ | ----- | ----------- | -------------------
F01 | Registracija / Prijava | Kod prvog pristupa aplikaciji korisnik će morati poslati prijavnicu klubu. Korisnik će unijeti mail i lozinku te dodati kratak opis sebe te će to proslijediti klubu koji odlučuje o primitku ili odbijanju upisa u klub. Ako je korisnik primljen u klub pristupa aplikaciji pomoću e-mail adrese i lozinke koju je sam definirao prilikom registracije u aplikaciju. | Fran Kundih
F02 | Unos i uređivanje profila sportaša i trenera | Korisnici će upisati svoju dob i dodati sliku profila da bi stvorili svoj korisnički račun. Klub upravlja profilom trenera. | Dominik Černjević
F03 | Upravljanje terminima treninga | Treneri će moći zakazati treninge ili odgoditi iste. | Emanuel Valec
F04 | Prikaz treninga i utakmica | Korisnici kao i treneri imat će prikaz svih prošlih i nadolazećih treninga i utakmica. | Dominik Černjević
F05 | Evidencija dolazaka | Treneri će voditi evidenciju dolaska te će korisnici imati pregled svojih dolazaka | Emanuel Valec
F06 | Evidencija članarina i plaćanja | Klub će voditi evidenciju o plaćenim i neplaćenim članarinama te ostalim troškovima. | Fran Kundih
F07 | Izvještaji o financijskom stanju kluba | Klub će moći stvarati financijske izvještaje za željeno razdoblje. | Dominik Černjević
F08 | Statistike momčadi | Cijeli klub će imati pregled međusobne statistike momčadi (rezultati natjecanja). | Emanuel Valec
F09 | Notifikacije sportašima | Aplikacija će korisnicima slati obavijesti o nadolazećim treninzima ili utakmicama. | Fran Kundih

Arhitektura programskog proizvoda će biti troslojna. Slojevi su sljedeći:
* Korisnički sloj (UI - WPF)
* Sloj poslovne logike
* Sloj pristupa podacima (Data Access Layer)

Korisnički sloj će biti odgovoran za interakciju s korisnikom i omogućava unos podataka i pregleda različitih podataka. Forme će biti dizajnirane tako da budu prilagođene specifičnim korisnicima (sportaši, treneri i administracija kluba).

Sloj podatkovne logike će biti implementiran kroz odvojene klase, svaka sa specifičnom funkcijom. Tako će se upravljati glavni procesi.

Sloj pristupa podacima će osiguravati komunikaciju aplikacije i baze podataka. Definirat će se SQL upiti za dohvaćanje, dodavanje, ažuriranje i brisanje podataka.
 

## Tehnologije i oprema

Za implementaciju rješenja koristit ćemo sljedeće tehnologije i alate:

* Razvojno okruženje: Visual Studio
* Programsko okruženje: .NET Framework
* Korisničko sučelje: WPF
* Baza podataka: SqLite i MySQL Workbench
* Programski jezik: C#
* Verzioniranje koda: Git i GitHub
* Upravljanje zadacima: GitHub Projects
* Dokumentacija: GitHub Wiki

