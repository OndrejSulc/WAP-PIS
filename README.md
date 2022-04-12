# WAP - PIS projekt


>## Databáze
>Příkaz pro instalaci dockeru: (Debian)
>``` 
>sudo apt install docker
>sudo apt install docker-compose
>```
>
>Příkaz pro spuštění databáze v dockeru:
>``` 
>sudo docker-compose up
>```
>Údaje pro PhpMyAdmin
> - port: 8080
> - login: root
> - pw: example
>
> pokud nechcete využít docker databázi a máte něco svého, tak si upravte soubor *appsettings.Development.json* a *appsettings.json* 
>
>Testovací endpoint: https://localhost:7223/Test Vytvoří databázovou strukturu pokud není a vytvoří tam nového uživatele s custom údaji


>## Postup při spuštění:
> 1) spustit databázi: *sudo docker-compose up*
> 2) spustit aplikaci: *dotnet run*

