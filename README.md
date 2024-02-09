# AzureOpenAI
NB: Ta kontakt for ny og bedre versjon ved ordentlig frontend og backend.

#### Husk å kjøre kommandoene:
```
dotnet build
dotnet add package Azure.AI.OpenAI --prerelease

### For å sette systemvariabler
- setx AZURE_OPENAI_KEY "REPLACE_WITH_YOUR_KEY_VALUE_HERE" 
- setx AZURE_OPENAI_ENDPOINT "REPLACE_WITH_YOUR_ENDPOINT_HERE"
```

Programmet er ikke særlig brukervennlig og tar ikke for seg edgecaser eller feilhåndtering.
F.eks ikke spør oppfølgingsspørsmål når det ikke er en tidligere melding. 

### Funksjoner
1. Konvertere kode
2. Forklare kode
3. Forklare og konvertere kode
4. Oppfølgingsspørmsål til svaret du fikk av GPT
5. Se meldingene i den nåværende samtalen
6. Lagre den nåværende samtalen. (Lagres som .txt fil)
7. Les siste melding
8. Se alle lagrede samtaler.
9. Fortsette på en lagret samtale (finn id ved å bruke 8.)
10. Slett lagrede samtaler
