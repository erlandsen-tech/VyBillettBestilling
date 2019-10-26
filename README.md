# VyBillettBestilling
Skoleoppgave billettbestillingssystem
## Innlogging
#### adminbruker
bruker: Admin@test.no   

pass:  123Test!
#### vanlig bruker
bruker: john@test.no

pass: 123Test!

Da dette er en rollebasert innloggingsmodell er det mulig å legge til nye
vanlige brukere. Adminbrukere må defineres i databasen.
Alle endringer til datbasen logges til egen tabell i de respektive basene.
Feillogging skjer via ELMAH rammeverket og kan 
nås via /Elmah.adx
