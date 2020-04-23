# Quotidian Diagnostics

> Everyday diagnostics for every day

qdx is a modular, package style dependency that provides a simple, injectable interface for logging in a language agnostic way.

## Another dx library?

Diagnostics seems like it should be a solved™ problem. After all, it's one of the most basic and ubiquitous tasks in programming. Yet, there remains an active level of debate and "churn" as new logging frameworks are born and die every day.

Another lot for the fire, qdx is based on the idea that an application developer wants a simple interface to signal trouble, confusion or resolution with a reusable package, with an injectable interface, and without taking on a large dependency. In this way, the package was influenced by [this answer on stack overflow](https://stackoverflow.com/questions/5646820/logger-wrapper-best-practice/5646876#5646876). Steven's advice was enlightening the first time I read it. In my own words:

> logging has two facets:
>
> 1. *signaling*, done by the domain code, is about pushing information.
> 2. *recording*, done by the logging framework, is about persisting information.
>
> A domain module's point of view of logging is just about signaling and this requires such a simple interface it's almost never worth taking a dependency

If you subscribe to the ideas of dependency injection, the entry point takes on the responsibility of adapting a specific logging framework to the various module's logging interface; a small job that frees the other modules from a direct dependency on any specific logging framework.

One of the Steven's main points is that taking a dependency is expensive and should be avoided. Since diagnostics interfaces are so simple, there's little harm in just writing your own for each project. I can't disagree with wanting to avoid dependencies, but what I think was missing from this discussion is consistency and its effect on readability. Recreating the diagnostics interface each time will inevitable lead to small differences between projects. I felt there was still some value in sharing this "simple interface". Thus, qdx.

qdx is **just** about signaling. It doesn't do formatting. It doesn't actually write anything. that work is left to you, likely as an adapter to an existing framework. You can feel confident about taking this dependency on because it's trivial to reimplement if you ever decide you need to drop it.

## External dependencies

None

## Installation

qdx is provided as a nuget package.
Use the nuget package manager in Visual Studio to install package qdx
**or** from the package manage console
    install-package qdx

## API

The basic interfaces in qdx are ITrace and ILogEntry. A module that needs to signal diagnostics should have an ITrace injected, and send an ILogEntry through the Log method.
ILogEntry works as something of a "wrapper" object with a number of properties which are ubiquitous to existing logging frameworks. The properties are well understood as part of a logging strategy and reimplemented in qdx to avoid taking a dependency on any other diagnostics framework. Because ITrace.Log accepts a generic ILogEntry<T>, the payload of the log entry can itself be your own wrapper object to pass additional information.

The "built-in" properties of ILogEntry are

- TraceLevel is an enum containing the typical logging levels: Critical, Error, Warning, Information, Verbose, Off.
- CorrelationId is a string meant to uniquely identify a module or operation to help with tracing multiple actions, agents or threads.
- Timestamp is a DateTimeOffset meant to capture the moment in time the Log was created, not necessarily when the logging framework received or recorded it.
- Code is an enum meant to uniquely identify the error encountered, allowing discussion of errors across languages without translations. Integers are perhaps a more common approach to identifying errors, but I preferred enums because they:
  1. are easier to manage in code
  2. are easier for a user to remember and type and
  3. can be converted to integers before passing them to your logging framework of choice.

LogEntry is a simple implementation of ILogEntry to facilitate sending messages.

There are also a few extension classes which add syntactic sugar for signaling an ILogEntry. These extension classes are segregated for their application to levels of modules: application level and domain level. In this context, an application level module is any module responsible for presentation to the user. Domain level is everything else. Distinguishing which level module code is in informs when to use logging and when to use errors (e.g., Exceptions).

The extension methods are added to ITrace, so signals can be made directly to the trace object.

The Domain level extension classes provide extension methods to create Verbose, Information and Warning level LogEntry objects.

- Verbose entries are made often when the code is taking a routine action. They are only useful when tracing down a difficult to reproduce bug.
- Information entries are made when the code is starting or ending a major routine, or making an important decision.
- Warning entries are made when an unexpected situation or error was encountered, but the code made a reasonable recovery and is continuing.

If domain level code encounters an error it cannot recover from, *it should not be logging*. Instead, throw an exception, and allow the upstream code to either recover from the exception -- at which point a Warning entry would be sensible -- or pass the exception, allowing it to propagate further upstream.

Eventually, domain code returns control to the Application level. The Application level extension classes provide extension methods to create Error and Critical level LogEntry objects.

- Error entries are made when a command or activity requested by the user is being aborted. The action is not going to complete, so the user should be notified.
- Critical entries are made when the application is deeply diseased and is about to die. Nearly all applications should wrap their entry point with a Pokémon exception handler, which simply signals a Critical entry and then shuts the application down.

## Examples

### Receive an ITrace dependency

    #using Quotidian.Diagnostics.Source;

    namespace qdx.Example
    {
      public class Foo
      {
        public Foo(ITrace trace)
        {
          Trace = trace ?? throw new ArgumentNullException(nameof(trace));
        }

        ITrace Trace { get; }
      }
    }

### Use an ITrace dependency in a domain object

    #using Quotidian.Diagnostics.Source;
    #using Quotidian.Diagnostics.Source.Domain;

    namespace qdx.Example.Domain
    {
      public class Customer
      {
        // ...

        public void AddOrder(Order order)
        {
          Trace.Information($"Adding order {order.Id} to customer {this.Id}")

          if (Orders.Contains(order))
          {
            Trace.Warning($"Customer already has an order with Id {order.Id}. Updating existing order instead of adding new order.");
            ReplaceOrder(order);
            return;
          }

          Orders.Add(order);

          Trace.Information($"Done adding order {order.Id}");
        }
      }
    }

### Use an ITrace dependency in an application entry point

    #using Quotidian.Diagnostics.Source;
    #using Quotidian.Diagnostics.Source.Application;

    namespace qdx.Example.Application
    {
      public class EntryPoint
      {
        private static ITrace Trace { get; }
        private static MyFavContainer Container { get; }

        static EntryPoint()
        {
            // Adding logging to AppDomain.UnhandledException helps when troubleshooting startup errors
            Trace = new MyCompany.Qdx.AdapterToFavLoggingFramework();
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Container = new MyFavContainer();
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Trace.Critical(ex);
            Environment.FailFast(ex.Message, ex);
        }
      }

        private static void Main(string[] args)
        {
          try
          {
            Container.Register<(Component
              .For<ITrace>()
              .ImplementedBy<MyCompany.Qdx.AdapterToFavLoggingFramework>());
            // Container.RegisterOtherTypes();

            var app = Container.Resolve<App>();
            app.Start();
          }
          catch (Exception e)
          {
            Trace.Critical(e);
            Environment.FailFast(ex.Message, e);
          }
        }
    }

### Use an ITrace dependency in an application object

    #using Quotidian.Diagnostics.Source;
    #using Quotidian.Diagnostics.Source.Application;

    namespace qdx.Example.Application.Commands
    {
      public CreateCustomerCommandHandler : ICommandHandler<CreateCustomer>
      {
        public CreateCustomerCommandHandler(ICustomerService customerService, ITrace trace)
        {
          Service = customerService ?? throw new ArgumentNullException(nameof(customerService));
          Trace = trace ?? throw new ArgumentNullException(nameof(trace));
        }

        ICustomerService Service { get; }
        ITrace Trace { get; }
        ITrace SecureTrace { get; set; }

        public void Execute(CreateCustomer command)
        {
          try
          {
            Service.DoTheThing(command.Property);
          }
          catch (SensitiveException e)
          {
            var c = Guid.NewGuid();
            Trace.Error($"The customer create command failed.", correlation: c);
            SecureTrace?.Error(new ApplicationException("The customer create command failed.", e), correlation: c);
          }
          catch (DidntWorkException e)
          {
            Trace.Error(new ApplicationException("The customer create command failed.", e));
          }
        }
      }
    }

For more examples, please clone the git repository and inspect the qdx.Samples.

## Contribute

I consider qdx "feature complete". Remember, it's supposed to be dead simple.  However, if you find a defect, have some additional insight, or an idea for a change in the API, I'm happy to hear from you! Any of the usual channels is fine: send a private message, log an issue, or send a pull request.

## License

MIT
