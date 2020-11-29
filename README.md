## What is docman?

Docman is the sample project written in .NET Core 3.1 to demonstrate how main functional programming and DDD techniques can be combined in the C# code.
The domain tackles a simplified document management which revolves around the changing document statuses.

## Architecture overview

The referencing application uses monolith architecture model. Underneath it relies heavily on the following patterns:
[CQRS](https://microservices.io/patterns/data/cqrs.html) and [Event sourcing](https://microservices.io/patterns/data/event-sourcing.html).
Write (command) side uses [EventStore DB](https://www.eventstore.com) to store events. Read (query) side uses [PostgreSQL DB](https://www.postgresql.org) to return data to the client.

### Handling commands

Commands are the earliest source of data. Commands are sent to the application by users and are handled by the command side, which does the following:  

1. When a command is received, the current state of the affected entity is computed from its history.
2. The command is turned into an event and applied to the entity’s state, obtaining its new state.
3. The event is persisted and published to interested parties for further processing.

## How to start the solution?
From the root directory execute:

`docker-compose up -d`

This command will start required infrastructure. Then you can start Docman application using `dotnet run` command from the `src/Docman.API` folder.

## What HTTP requests can be sent to the docman application?
You can find the list of all HTTP requests in [Docman.rest](https://github.com/GPAshka/docman/blob/master/Docman.rest) file placed in the root folder of the repository. 
This file is compatible with [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) plugin for [Visual Studio Code](https://code.visualstudio.com).

Document requests should be sent in the following order:
1. Create document
2. Update document or add file
3. Send for approval
4. Approve document or reject document