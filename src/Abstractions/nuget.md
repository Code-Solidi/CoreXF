# NuGet CLI
To publish use the command
<pre>dotnet nuget push -s=http://baget.local/ -k=123456 CoreXF.Abstractions.2.0.0.nupkg</pre>
for the local baget server.
To Delete
<pre>dotnet nuget delete -s=http://baget.local -k=123456 CoreXF.Abstractions 2.0.0</pre>

Clearing the cache:
<pre>dotnet nuget locals all --clear</pre>

