# Introduction
Lexical.FileSystem is a virtual filesystem class libraries for .NET.

NuGet Packages:
* Lexical.FileSystem.Abstractions
* Lexical.FileSystem

IFileSystem is accesses a file-system through abstraction.

```csharp
IFileSystem filesystem = new FileSystem(AppDomain.CurrentDomain.BaseDirectory);
```

Files can be browsed.

```csharp
foreach (var entry in filesystem.Browse(""))
    Console.WriteLine(entry.Path);
```

Files can be opened for reading.

```csharp
using (Stream s = filesystem.Open("file.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
{
    Console.WriteLine(s.Length);
}
```

Files can be opened for writing.

```csharp
using (Stream s = filesystem.Open("somefile.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
{
    s.WriteByte(32);
}
```

Files and directories can be observed for changes.

```csharp
IObserver<FileSystemEntryEvent> observer = new Observer();
using (IDisposable handle = filesystem.Observe("", observer))
{
}
```

<details>
  <summary><b>Observer.cs</b> above. (<u>Click here</u>)</summary>

```csharp

```
</details>

Directories can be created.

```csharp
filesystem.CreateDirectory("dir");
```

Directories can be deleted.

```csharp
filesystem.Delete("dir", recursive: true);
```

Files and directories can be renamed.

```csharp
filesystem.CreateDirectory("dir");
filesystem.Move("dir", "new-name");
```


**Links**
* [Website](http://lexical.fi/FileSystem/docs/index.html)
* [Github](https://github.com/tagcode/Lexical.FileSystem)
* [Nuget](https://www.nuget.org/packages/Lexical.FileSystem/)

