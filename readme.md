# Injecterize .NET Core 
### Add Services for Dependency Injection using .NET Core

---


Features:

- **Simple Implementation** - Simple Attribute on your classes as you code.
- **Generic Support** - Supports Scoped , Singleton and Transient types.
- **Custom Implementation** -Target the Service Type \ Interface you want.


### References 
* [Microsoft.Extensions.Logging](https://github.com/aspnet/Logging/tree/master/src/Microsoft.Extensions.Logging)  - For Logging during Scanning


### Getting Started

``TODO``



### Usage

In your Startup class (Usually Startup.cs) : 
```c#
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
     .....
            
     services.AddInjecterize(new InjecterizeOptions()
     {
         TryRegisterWithFirstInterface = true
     });
            
     .....
}
```


Then in your classes you want to inject:

```c#
    [Injecterize]
    public class StudentDao : IStudentDao
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly string _databaseName;
        
        .....
     
```

 And then Inject it when needed.
 
```c#
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentDao _dal;


        public StudentController(IStudentDao studentDao)
        {
            _dal = studentDao;
        }
```

#### Different Scopes

You can set the Scope type as part of the Attribute

```c#
  [Injecterize(TargetScope = InstanceScope.Singleton)]
  public class StudentDao : IStudentDao
    ...

```


####

Multiple Interfaces , define what one you want

```c#
    [Containerize(InterfaceToUse = typeof(IStudentDao))]
    public class StudentDao : IStudentDao , ISomeOtherInterface
       ...
    
```


Scopes supported are 
* Singleton
* Scoped
* Transient 

----


#### Other Options

| Option                        | Description                                                                                                                                                                                                                                                               | Default |
|-------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------|
| TryRegisterWithFirstInterface | Will look at implementation class and use the first interface it finds as the Service Interface. You can then inject using that Service Interface  if you specified an interface as part of the InjecterizeAttribute it will bypass this option and use what you provided | false   |



