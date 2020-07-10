# Git Module for .NET
Copy from gogs git module. [https://github.com/gogs/git-module](https://github.com/gogs/git-module)
![nuget version](https://img.shields.io/badge/Nuget-0.8.3.2-green) 

Package git-module is a Go module for Git access through shell commands.

## Requirements
* C# version must be at least 8.0.
* Git version must be no less than 1.8.3.
License
This project is under the MIT License. See the LICENSE file for the full license text.

## How to use?



### Installation

```bash
dotnet add package cassie-git-module --version 0.8.3
```

### Initial a project

```c#
    var repo = new Repository();
    await repo.Init(path,new InitOptions{Timeout=20,Bare=true});
```

### Clone a project

```c#
    var repo = new Repository();
    await repo.Clone(url,dest,new CloneOptions{Timeout=20});
```

### Fetch

```c#
    var repo = new Repository([Your Repo Path]);
    await repo.Fetch(new FetchOptions{Timeout=20});
```

### Pull

```c#
    var repo = new Repository([Your Repo Path]);
    await repo.Pull(new PullOptions{Timeout=20});
```

### Push

```c#
    var repo = new Repository([Your Repo Path]);
    await repo.Push(new PullOptions{Timeout=20});
```

### Push

```c#
    var repo = new Repository([Your Repo Path]);
    await repo.Push(remote,branch,new PushOptions{Timeout=20});
```

### Checkout

```c#
    var repo = new Repository([Your Repo Path]);
    await repo.Checkout(branch,new CheckoutOptions{Timeout=20});
```
...


For details, You can reference the assie.git.module.test project to use this SDK.

