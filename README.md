# Quotidian Diagnostics

> Everyday diagnostics for every day

qdx is a modular, package style dependency that provides a simple, injectable interface for logging in a language agnostic way.

## Another dx library?

Diagnostics seems like it should be a solved™ problem. After all, it's one of the most basic and ubiquitous tasks in programming. Yet, there remains an active level of debate and "churn" as new logging frameworks are born and die every day.

qdx is based on the idea that an application developer wants a simple interface to signal trouble, confusion or resolution with a reusable package, with an injectable interface, and without taking on a large dependency. In this way, the package was influenced by [this answer on stack overflow](https://stackoverflow.com/questions/5646820/logger-wrapper-best-practice/5646876#5646876). Steven's advice was enlightening the first time I read it. In my own words:

> logging has two facets:
>
> 1. *signaling*, done by the domain code, is about pushing information.
> 2. *recording*, done by the logging framework, is about persisting information.
>
> A domain module's point of view of logging is just about signaling and this requires such a simple interface it's almost never worth taking a dependency

If you subscribe to the ideas of dependency injection, the entry point takes on the responsibility of adapting a specific logging framework to the various module's logging interface; a small job that frees the other modules from a direct dependency on any specific logging framework.

What I think was missing from this discussion though, is consistency and its effect on readability. I felt there was still some value in sharing just this "simple interface".

qdx is **just** about signaling. It doesn't do formatting. It doesn't actually write anything. that work is left to you, likely as an adapter to an existing framework. You can feel confident about taking this dependency on because it's trivial to reimplement if you ever decide you need to drop it.

## External dependencies

None

## Installation

qdx is provided as a nuget package.
Use the nuget package manager in Visual Studio to install package qdx
**or** from the package manage console
    install-package qdx

## Examples

The basic interfaces in qdx are ITrace and ILogEntry. A module that needs to signal diagnostics should have an ITrace injected, and send an ILogEntry.
ILogEntry has a TraceLevel property which is yet another enum containing the typical logging levels: Critical, Error, Warning, Information, Verbose, Off. The list is well understood as part of a logging strategy and reimplemented in qdx to avoid taking a dependency on any other diagnostics framework.

LogEntry is a simple implementation of ILogEntry to facilitate sending messages.

There are also a few extension classes which add syntactic sugar for signaling an ILogEntry. These extension classes are segregated for their application to levels of modules: application level and domain level. In this context, an application level module is any module responsible for presentation to the user. Domain level is everything else. Distinguishing which level module code is in informs when to use logging and when to use errors (e.g., Exceptions).

For application level code, qdx provides Error and Critical level signaliing.

    using Quotidian.Diagnostics.Source
    public void DoActivity(ITrace trace)
    {
        trace.Warning("Activity cannot be done" )
    }

## API

## Contribute

I consider qdx "feature complete". Remember, it's supposed to be dead simple.  However, if you find a defect, have some additional insight, or an idea for a change in the API, any of the usual channels is fine: send a private message, log an issue, or send a pull request.

## License

MIT