# AzureOpenAI
#### Husk å kjøre kommandoene:
```
dotnet build
dotnet add package Azure.AI.OpenAI --prerelease

### For å sette systemvariabler
- setx AZURE_OPENAI_KEY "REPLACE_WITH_YOUR_KEY_VALUE_HERE" 
- setx AZURE_OPENAI_ENDPOINT "REPLACE_WITH_YOUR_ENDPOINT_HERE"
```


### Funksjoner
1. Konvertere kode
2. Forklare kode
3. Forklare og konvertere kode
4. Oppfølgingsspørmsål til svaret du fikk av GPT
5. Se meldingene i den nåværende samtalen
6. Lagre den nåværende samtalen. (Lagres som .txt fil)
8. Se alle lagrede samtaler.
9. Fortsette på en lagret samtale (finn id ved å bruke 8.)
10. Slett lagrede samtaler
