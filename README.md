# examination system
to operate the system

1-in "Web.config" file at line
```xml
<add name="Examination System" connectionString="Data Source=DESKTOP-DGNIPKK;Initial Catalog=Examination_System;Integrated Security=True" providerName="System.Data.SqlClient"></add>
```
put your Database conection string insted of "Data Source=DESKTOP-DGNIPKK;Initial Catalog=Examination_System;Integrated Security=True"

2-run the next commands in Package Manager Console in visual studio ```Add-Migration``` 
-it will create a class that will create the database after 

3-runing the next commands in Package Manager Console in visual studio ```Update-Database```
