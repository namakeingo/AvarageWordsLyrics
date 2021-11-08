# Music Artist Data - CLI App

## Description
A program that, when given the name of an artist, will retrieve various statistical information about that artist (average number of words in lyrics, min and max length, the total number of songs, etc.)

It is written using .NET Core and it follows a service/client structure. 
The services are developed using gRPC the prototype for the services is defined in MusicSetvices.Proto and the implementation is done in MusicServices.

The services are used to communicate with two external APIs: https://musicbrainz.org/ws/2/ and https://api.lyrics.ovh/v1/

In addition to that, the LyricsOvcService makes use of a local database that stores the lyrics that were already searched. That is useful given that the https://api.lyrics.ovh/v1/ API is really slow. 
Given that the lyrics of a song is a constant in make sense to store the response for subsequent calls so that the retrieval will be much faster.

The app is coded following the TDD approach (Red/Green/Refactor).

## HOW TO RUN
### Option 1:
- Clone the repo to your drive.
- Run .bat script [1#-RunLocalServer.bat](1#-RunLocalServer.bat) and **keep the terminal window open**. The script will build the solution and it will host locally the gRPC services. If you close the terminal the server will also stop running.
- Run .bat script [2#-RunCLIApp.bat](2#-RunCLIApp.bat). This is the actual CLI App. You can now use the functions presented.

p.s. please be aware that given that they are *.bat files they could be wrongly detected as malicious by antivirus.

### Option 2:
- Clone the repo to your drive.
- Open project [Music_Artists_Data_CLI.sln](Music_Artists_Data_CLI.sln) in Visual Studio
- Open project properties
- `Common Properties / Startup Project` check radio button `Multiple startup projects`
- Select `Start` in dropdowns for `MusicServices.Client` and `MusicServices.Host` and Apply changes
- Start Project Execution

## Addidional Notes
This was the first time I used gRPC. I was already set on the idea of using services to isolate the API communication and I would normally use WCF. However, given that .NET Core does not support WCF I decided to take the opportunity to learn gRPC that is the official replacement for WCF.
