## Manažerský kalendář

### Autoři

František Jeřábek (xjerab25)  
Damián Gladiš (xgladi00)  
Ondřej Šulc (xsulco01)  
Šimon Galba (xgalba03)

### Implementace

Webová aplikace je vytvořená čistě pomocí HTML CSS a JavaScript. Je zpřístupněná přes webový server projektu PIS. Po
prvotním načtení z webového serveru je již prezentační logika řešena na straně klienta. Ze serveru jsou poté data
získávána pomoci REST API. Zároveň klient může komunikovat se serverem pomocí SignalR. Přihlašování je na serverové
straně zajištěno knihovnou Identity. Na straně klienta je při přihlášení uložen cookie s identifikačním tokenem, který
je zaslán společně s každým dotazem.

### Testování

Testování je zahájeno voláním testovacího endpointu (/Test/CreateTestingUsers), který vytvoří testovací účty. Následně
je možné se do aplikace přihlásit a otestovat jednotlivé fuknce aplikace.

### Knihovny

SignalR - Knihovna společnosti Microsoft umožňující komunikaci s klientem v reálném čase.  
Bootstrap - Předdefinovaná sada CSS souborů.  
JQuery - Sada funkcí pro zjednodušení práce s DOM.  
tui calendar - Sada na zobrazení a prácí s kalendářem.

### Informační zdroje

[Microsoft dokumentace ke knihovně SignalR](https://docs.microsoft.com/cs-cz/aspnet/core/signalr/introduction?view=aspnetcore-6.0)  
[Bootstrap documentation](https://getbootstrap.com/docs/5.1)  
[TUI calendar documentation](https://nhn.github.io/tui.calendar/latest/)

