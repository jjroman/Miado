# Miado
Miado is a data-access layer with an intuitive and easy to use API which wraps around and improves upon the usage of straight ADO.Net code.


#What is it?
Miado is not yet-another-ORM library. If you are looking for an ORM tool and/or SQL generator, look at LINQ, NHibernate, Subsonic, etc.

However, if you need or want to use the power of straight-up SQL, Stored Procedures, and ADO.Net, Miado offers:
A much easier API to program to compared with standard ADO.Net
Intuitive usability since it is designed with a Fluent Interface
A vendor-neutral ADO.Net interface with the ability to easily swap out DbProvider implementations
Automation of repetitive, error-prone, boiler-plate ADO.Net code
Easy integration with Inversion-of-Control containers
Ability to easily create custom business objects from ADO.Net result sets
Support for easy unit testing and mocking
Simple integration in PowerShell scripts since data access code is much less verbose

#Purpose:
I migrated it from its archive in Codeplex: https://archive.codeplex.com/?p=miado, and I created a .Net standard library to use it.
The main purpose is to migrate systems that are using this library to .net core.

I don't know who was the original author of this library, if anyone requires this, let me know and I can share it.
