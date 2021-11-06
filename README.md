# Avarage Words Lyrics - CLI App

# Description
A program that, when given the name of an artist, will produce the average of words per lyric of that author. 

It is written using .NET Core and it follows a service/client structure. 
The services are developed using gRPC the prodotypo for the services is defined in MusicSetvices.Proto and the implementetion is done in MusicServices.

The services are a used to comunicates with two external APIs: https://musicbrainz.org/ws/2/ and https://api.lyrics.ovh/v1/

In addition to that the LyricsOvcService makes use of a local database that stores the lyrics that were alredy serched. That is usufull given that the https://api.lyrics.ovh/v1/ API is really slow. 
Given that the lyrics of a song is a constant in make sense to store the response for subsequent calls so that the retrival will be much faster.

The app is coded following the TDD approach (Red/Green/Refactor).

# HOW TO RUN
- Clone the repo to your drive.

- Run .bat script [1#-RunLocalServer.bat](1#-RunLocalServer.bat) and **keep the terminal window open**.
The script will build the solution and the it will host locally the gRPC services. If you close the terminal the server will also stop running.

- Run .bat sript [2#-RunCLIApp.bat](2#-RunCLIApp.bat). 
This is the actual CLI App. You can now use the functions presented.

p.s. please be aware that bieig .bat file they could be wrongly detected as malicios by antivirus.
