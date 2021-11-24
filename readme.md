# Injecterize .NET Core 
### Add Services for Dependency Injection using .NET Core

---


Features:

- **Simple Implantation** - Annotate your classes as you code.
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
            
            services.AddContainerized(new ContainerizeOptions()
            {
                TryRegisterWithFirstInterface = true
            });
            
            .....

```


Then in your classes you want to inject:

```c#

  
    [Containerize]
    public class StudentDao : IStudentDao
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly string _databaseName;

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

That's it..
