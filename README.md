<h1>Akademia Tarnowska </h1>
<h1>Kurs</h1>
<h5>Testowanie  i jakość oprogramowiania / Projekt </h5>

<h1>Autor</h1>
<h5>Kamil Martyka</h5>

<h1>Temat projektu</h1>

<h5>Testowanie systemu komentarzy i połączonych systemów </h5>

<h2>Opis Projektu</h2>

Projekt ten testuje system komentarzy przeznaczony do większego projektu (przewodnika turystycznego). Projekt zawiera testowanie operacji na komentarzu, systemu powiadomień, systemu cenzorującego, oraz powiązanych funkcji systemu komentarzy używanych w innych częsciach projektu.<br>

Komentarz - wiadomośc pozostawiona przez użytkownika pod atrakcją turystyczną,  lub komentarzem innego użytkownika. Komentarz może być zostawiony tylko przez użytkownika zalogowanego.<br>

Powiadomienia - jest to wiadomośc wysyłana do użytkownika w momencie gdy:<br>
- komentarz został skomentowany przez innego użytkownika<br>
- komentarz został usunięty przez administratora<br>
- komentarz został zeedytowany/ocenzurowany przez administratora<br>

<h1>Uruchomienie projektu</h1>

<h4>API</h4>
Do pełnego i poprawnego działania projektu wymagane jest:<br>

- server sql (mssql)<br>
- plik bazydanych dodany do repozytorium.<br>

<h4>Aplikacja mobilna</h4>
Do pełnego i poprawnego działania projektu wymagane jest:<br>

- ngrock(bądź inny program, służący do upublicznienia localhost, w celu obejścia cors)<br>
- Do testowania użyto MStests C# .NET,<br>
- Do mockowania użyto pakietu MOQ,<br>
- front wykonano w React native na platformę android.<br>

Po zainstalowaniu bazy danych, należy uruchomić api (komenda nie jest wymagana). Api znajdować się bedzie pod adresem https://localhost:7225/swagger/index.html.

<h1>Uruchomienie testów jednostkowych i integracyjnych</h1>
testy można uruchamiać przy pomocy UI. Wymagany do uruchomienia testów jest Mstest oraz Mstet framework, powinny dodać się one automatycznie wraz z projektem, wrazie problemów należy dodać je za pomoca usługi nugget. Ui umożliwiające obsługę testów znajduję się w zakładce "Test". <br>

<h1>Dokumentacja API</h1>
  Każda z funkcji API zwraca wewnętzny obiekt serwera który zawiera status, wiadomość, obiekt. <br>

- Adres usługi: /Comment/AllComentsForSpot/{id},<br>
- Typ: Get<br>
- Przyjmuje: id - numer identyfikacyjny miejsca turystycznego dla którego chcemy pobrać komentarze.<br>
Zwraca:
```json[{
"Id":0,
"Title":"",
"Description":"",
"Score":0,
"UserId":,
"TouristSpotId":,
"ParentCommentId":,
"CreatedAt":"",
"UpdatedAt":"",
"Avatar":"",
"Username":"",
"CommentChildNumber":0}]
```

- Adres usługi: /Comment/AllComentsForParent/{id},<br>
- Typ: Get<br>
- Przyjmuje: id - numer identyfikacyjny kometarza dla którego chcemy pobrać podkomentarze.<br>
Zwraca:
```json[{
"Id":0,
"Title":"",
"Description":"",
"Score":0,
"UserId":,
"TouristSpotId":,
"ParentCommentId":,
"CreatedAt":"",
"UpdatedAt":"",
"Avatar":"",
"Username":"",
"CommentChildNumber":0}]
```

- Adres usługi: /Comment/remove/{userId}/{id},<br>
- Typ: Post<br>
- Przyjmuje: <br>
  - userId - numer identyfikacyjny użytkownika dalktórego usuwamy komentarz.<br>
  - id - numer identyfikacyjny kometarza.<br>
Zwraca: Status OK/Status błędu<br>

- Adres usługi: /Comment/add/{userId},<br>
- Typ: Post<br>
- Przyjmuje: <br>
  - userId id dodającego uzytkownika (potrzebne do dodatkowej obsługi autoryzacji)
  -  ```json{
      "id": 0,
      "title": "string",
      "description": "string",
      "score": 0,  "userId": 0,
      "touristSpotId": 0
     }
     ```
Zwraca: Status OK/Status błędu<br>

- Adres usługi: /Comment/comment/{userId},<br>
- Typ: Post<br>
- Przyjmuje: <br>
  - userId id dodającego uzytkownika (potrzebne do dodatkowej obsługi autoryzacji)
  - ```json{
    "id": 0,
     "title": "string",
     "description": "string",
    "score": 0,
    "userId": 0,
     "touristSpotId": 0,
     "parentCommentId": 0}
    ```

- Adres usługi: /Comment/update/{userId},<br>
- Typ: Post<br>
- Przyjmuje: <br>
  - userId id dodającego uzytkownika (potrzebne do dodatkowej obsługi autoryzacji)
  -  ```json{
     "id": 0,
     "title": "string",
     "description": "string",
     "score": 0,
     "userId": 0,
     "touristSpotId": 0}```<br>

- Adres usługi: /Comment/AllComentsForUser/{userId},<br>
- Typ: Get<br>
- Przyjmuje: <br>
  - userId numer identyfikacyjny użytkownika 
Zwraca:  ```json[
{"Id":0,"Title":"",
"Description":"",
"Score":0,
"UserId":,
"TouristSpotId":,
"ParentCommentId":,
"CreatedAt":"",
"UpdatedAt":"",
"Avatar":"",
"Username":"",
"CommentChildNumber":0}]```<br>

- Adres usługi: /User/getUserNotifications/{UserId},<br>
- Typ: Get<br>
- Przyjmuje: <br>
  - userId numer identyfikacyjny użytkownika 
Zwraca:  ```json[{
"id": 0,
"userId": 0,
"description": null,
"createdOn": "0001-01-01T00:00:00",
"checked": false}]```<br>

- Adres usługi: /User/setUserNotification/{UserId}/{Nid},<br>
- Typ: Get<br>
- Przyjmuje: <br>
  - userId numer identyfikacyjny użytkownika 
  - Nid - numerIdentyfikacyjny powiadomienia
Zwraca:Status OK / Status Błedu<br>


<h2>Kroki testowe</h2>

| Id    | Opis                                                         | Kroki Testowe                                                                                                                                                                                                                                                                                                                                                                                                  | Oczekiwany Wynik                                                                                                                            |
|-------|--------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------|
| TC_01 | Dodanie Komentarza                                           | 1.Zaloguje się na konto <br> 2.Wejdź w jedno z miast <br> 3.Wybierz jedno z miejsc <br> 4.Przesuń na dół okna <br> 5.Kliknij „Dodaj komentarz” <br> 6.Uzupełnij pola pamiętając o ocenie <br> 7.Zatwierdź dodanie komentarza <br>                                                                                                                                                                              | Po odświeżeniu nowo utworzony komentarz powinien być widoczny, pod miejscem do którego został dodany                                        |
| TC_02 | Edycja komentarza                                            | 1.Zaloguje się na konto <br> 2.Wejdź w jedno z miast <br> 3.Wybierz jedno z miejsc <br> 4.Przesuń na dół okna <br> 5.Zlokalizuj wcześniej dodany  komentarz  <br> 6.Użyj przycisku menu w prawym górnym rogu <br> 7.Wybierz edytuj z menu <br> 8.Zmień dane w tytule, opisie oraz zmień ocenę komentarza <br> 9.Odśwież listę komentarzy  <br>                                                                 | Po odświeżeniu treść komentarza powinna ulec zmianie.                                                                                       |
| TC_03 | Usuwanie komentarza                                          | 1.Zaloguje się na konto <br> 2.Wejdź w jedno z miast <br> 3.Wybierz jedno z miejsc <br> 4.Przesuń na dół okna <br> 5.Zlokalizuj wcześniej dodany  komentarz  <br> 6.Użyj przycisku menu w prawym górnym rogu <br> 7.Wybierz usuń z menu <br> 8.Potwierdź usuwanie <br> 9.Odśwież listę komentarzy  <br>                                                                                                        | Po odświeżeniu komentarz powinien zostać usunięty i nie być dłużej widoczny na liście komentarzy.                                           |
| TC_04 | Sprawdzenie ograniczonego dostępu do edycji/usuwania         | 1.Zaloguje się na konto <br> 2.Wejdź w jedno z miast <br> 3.Wybierz jedno z miejsc <br> 4.Przesuń na dół okna <br> 5.Zlokalizuj wcześniej dodane komentarze <br> 6.Upewnij się iż znajdują się tam komentarze dodane przez różnych użytkowników <br> 7. 	Sprawdź czy menu usuwania występuje jedynie dla użytkownika dla którego jesteś zalogowany <br>                                                         | Przycisk menu powinien być widoczny tylko dla użytkownika do którego należy komentarz.                                                      |
| TC_05 | Powiadomienie po skomentowaniu komentarza innego użytkownika | 1.Zaloguje się na konto użytkownika 1 <br> 2.Przejdź do komentarzy 3.Dodaj komentarz do miejsca <br> 4.Przełącz się na konto użytkownika 2 <br> 5.Przejdź ponownie do sekcji komentarzy tej samej dla której właśnie dodałeś komentarz <br> 6.Skomentuj komentarz użytkownika 1 <br> 7.Wyloguj się z konta użytkownika 2 <br> 8.Zaloguj się na konto użytkownika 1 <br> 9.Wejdź w zakładkę powiadomnienia <br> | W zakładce powiadomienia powinno istnieć powiadomienie informujące o tym iż komentarz użytkownika 1 został skomentowany przez użytkownika 2 |
| TC_06 | Dostęp do komentarza administratora                          | 1.Zaloguje się na konto administratora <br> 2.Przejdź do komentarzy <br> 3.Upewnij się że administrator ma możliwość otwarcia menu edycji/usuwania na komentarzu każdego użytkownika. <br>                                                                                                                                                                                                                     | Administrator powinien mieć dostęp do edycji/usuwania każdego z komentarzy.                                                                 |
| TC_07 | Powiadomienie po usunięciu komentarza przez administratora   | 1.Zaloguje się na konto użytkownika 1 <br> 2.Przejdź do sekcji komentarzy jednego z miejsc <br> 3.Dodaj komentarz  <br> 4.Zmień  konto na konto administratora <br> 5.Prejdź do dodanego komentarza <br> 6.Usuń komentarz  <br> 7.Wróć na konto użytkownika 1 <br> 8.Wejdź w powiadomienia  <br>                                                                                                               | W oknie powiadomień powinno znajdować się nowe powiadomienie informujące, iż komentarz użytkownika 1 został usunięty przez administratora   |
| TC_08 | Dodanie komentarza z pustym tytułem                          | 1.Zaloguje się na konto użytkownika  <br> 2.Przejdź do sekcji komentarzy jednego z miejsc <br> 3.Otwórz dodawanie komentarza <br> 4.Uzupełnij pola opis oraz ocena nie uzupełniając tytułu <br> 5.zatwierdź dodawanie komentarza <br>                                                                                                                                                                          | Komentarz nie powinien zostać dodany, informacja o błędzie przy wypełnianiu powinna być widoczna u góry ekranu.                             |
| TC_09 | Dodanie komentarza z oceną równą 0                           | 1.Zaloguje się na konto użytkownika  <br> 2.Przejdź do sekcji komentarzy jednego z miejsc <br> 3.Otwórz dodawanie komentarza <br> 4.Uzupełnij pola tytuł, opis. Ocena powinna pozostać na 0. <br>  5.zatwierdź dodawanie komentarza <br>                                                                                                                                                                       | Komentarz nie powinien zostać dodany, informacja o błędzie przy wypełnianiu powinna być widoczna u góry ekranu. Minimalna ocena wynosi 1.   |
| TC_10 | Edycja komentarza na pusty                                   | 1.Zaloguje się na konto użytkownika <br> 2.Przejdź do sekcji komentarzy jednego z miejsc <br> 3.Znajdź jeden z dodanych komentarzy lub utwórz nowy komentarz <br> 4.Przejdź do edycji komentarza <br> 5.Usuń tekst z pola „Tytuł” <br> 6.Zatwierdź edycję <br>                                                                                                                                                 | Komentarz nie powinien zostać ze edytowany, informacja o błędzie po zatwierdzeniu zmiany powinna pojawić się u góry ekranu.                 |

<h1>Technologie użyte w projekcie</h1>
- .NET 7.0 <br>
- Swagger <br>
- MSSQL <br>
- React Native <br>
- CSS <br>
- Android <br>
- C# <br>
