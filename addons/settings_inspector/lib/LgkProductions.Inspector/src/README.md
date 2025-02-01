# LgkProductions.Inspector
Abstract backend for object inspection.

## Getting Started
> [!TIP]
> Implement [`INotifyPropertyChanged`](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged) to prevent polling.

```cs
MyObject instance = new();

// Attach inspector using "tickProvider" used for polling
var inspector = Inspector.Attach(instance, tickProvider);

// Get notified on change
inspector.Elements[i].ValueChanged
  += (instance, member, newValue) => Console.WriteLine(newValue);

// Modify value through inspector
inspector.Elements[i].Value = 42;
```

## License
This project is licensed under the [MIT License](LICENSE.txt).
