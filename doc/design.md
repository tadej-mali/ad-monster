# Design

## Overview

The application is a single page HTML 5 web application. The client runs in a browser and is designed as a MVC application based on the 
[backbone.js](http://backbonejs.org/) framework. The framework is responsible for separating the presentation, data and application layers as well
as for application navigation and data synchronization between the client and server tiers.

The server is a simple REST data service, supporting basic operations. It is implemented using the [ASP.NET Web API](http://www.asp.net/web-api) framework for the application layer and the MS SQL Server as the underlying relational database.

## User interface

### Building blocks

The [jQuery](http://jquery.com/) libray was used to handle the dynamic DOM manipulation, event handling and AJAX calls.
It was chosen because it abstracts away the browser specifics, provides a powerful API for DOM manipulation and offers a high level AJAX call
interface. It is also a stable/mature library and integrates well with backbone.js.

Since the application has to display dynamic lists (list of directories and list of contents) I was looking for a simple templating library,
which would support the model-view-controller design pattern for HTML rendering. This was the primary reason for choosing the backbone.js.
The same functionality could be already achieved using the [underscore.js](http://underscorejs.org/), which is actually the default rendering 
engine of the the backbone.js. However, the backbone.js fetch/save/destroy methods of its Model class support the REST CRUD operations out of the 
box and this was the reason why I decided for the complete framework.

The page layout was styled using the [Twitter Bootstrap](http://getbootstrap.com/) framework version 3.0, which supports responsive design.
As someone said: "It makes it possible that even a backend developer can create a presentable page."

### Implementation details

The main page that gets loaded by the browser is index.html. It loads the stylesheets, defines the content placeholders for the dynamic content
and loads the javascript. The entry point into the application is the "classic" *onDocumentReady* jQuery event. At this point the page templates
are loaded:

* HomeView ... the default landing page, contains application description and short instructions
* DirectoryListView ... container of the directory list
* DirectoryListItemView ... one directory item
* DirectoryDetails ... list of directory contents

When the templates are loaded, bacbone.js routing is started (there are two routes: home and directoryContent), defaulting to home page.
From this point on, the models are fetched from the server, templates are rendered and using jQuery the rendered HTML is inserted into appropriate
placeholders.

### Testing

There are two Model implementations: in-memory and rest-api. In memory implementation was used during development for client side testing.
It is realized by overriding the backbone.js fetch method and reading the advertisements data from hardoded JSON values.

The client was tested manually, in browsers FireFox (desktop), Chrome (android) and Safari (iPad).

### Room for improvement
The following areas have not been addressed and should be improved for a real/larger production application

* There is not much of error handling implemented
* Automatic testing (using appropriate js testing framework and some additional model refactoring needed)
* Using a module loader like [require.js](http://requirejs.org/) to handle module dependencies (now all scripts are hardcoded in the index.html)
* Visual design
* Build automation (minification, compression)
* Service endpoint is hardcoded, should be more configurable

## Data service

### Building blocks

The .NET was chosen because of "time to market" factor. This is the framework that I am most familiar with and given the "use anyframwork you want" wildcard, the choice was obvious. However, I also used this opportunity to use the lastes versions, which were recently released:

* ASP.NET MVC 5.0 ... base web framework of the .NET web stack
* ASP.NET Web API 5.0 ... framework for building REST services
* Entity Framework 6.0 ... ORM
* MS SQL 2012 ... relational DB

The main component is teh Web API framework, which has out of the box support for the GET/POST/PUT/DELETE operations, routed to controller actions. Routing can be by convention (used in the application) or fully customized.

The data layer was relized with a combination of SQL relational database and object-relational-mapping framework.

A framework for Inversion of Control [Simple Injector](http://simpleinjector.codeplex.com/) was used to resolve component dependencies at runtime.
The reasons for its choice were speed and simplicity, [see here] (http://www.palmmedia.de/blog/2011/8/30/ioc-container-benchmark-performance-comparison).

Unit testing frameworks used are [NUnit](http://www.nunit.org/) (test definition and execution, mature and stable, widely used)
and [Moq](https://github.com/Moq/moq4/wiki/Quickstart) (mocking framework, simple to use, mature and stable).

### Implementation details

There are three layers on the server side:

* Controller ... entry point into the application, responsible for control flow
* Service ... responsible for business logic
* Model/Data ... responsible for data access

In order to decouple components and make their testing easier, the service and data access layer are always separated into interface and implementation. The interface can be mocked for testing and the inmplementation is resolved at runtime using the IoC.

The REST API supports the following operations:

* GET  api/directory ... lists all directories, data is returned as JSON
* GET  api/directory/{id} ... gets contents of directory by id, data is returned as JSON. Possible error results: HttpStatusCode.NotFound
* POST api/directory ... creates a new directory, directory data is posted as JSON in the request body
* PUT  api/directory/{id} ... updates an existing directory, directory data is posted as JSON in the request body. 
  Possible error results:
  HttpStatusCode.NotFound,
  HttpStatusCode.BadRequest (mismatch between id and JSON body),
  HttpStatusCode.Conflict (the directory has been modified)
* DELETE api/directory/{id}?version=ver ... deletes an existing directory, parameter version is used for concurrency control
  Possible error results:
  HttpStatusCode.NotFound  
  HttpStatusCode.Conflict (the directory has been modified)

All data modification operations (update and delete) are executed with optimistic concurrency check. Each entity is versioned withing the DB and
the version is incremeted on each change. A client always receives the current version together with the rest of the data.
When client requests data modification, the operation can only succeed when the version number in the DB has not been changed. In the opposite case
an error is reported.

### Testing

A dedicated test project has been added to the solution. The test project structure (namespace organization) reflects the implementation structure.
Each implemented class, being tested, has a corresponding \*Test class. This \*Test class contains a suite of automated tests.

There a two groups of tests. The controller tests are unit tests, with a mocked service layer.
The purpose of these tests is to ensure, that proper service methods are invoked by the controller and that proper HTTP result codes are returned
to the client, as specified above.

The second group of tests is testing the CRUD operations on the database by invoking appropriate service methods. These tests are executing
actual database operations. They always first prepare the test data, run some operation on the test data and finaly erase the test data, to keep the
database unchanged.

### Room for improvement

* Delete operation currently physically removes a record from the DB. In production system the deletion operation should probably be logical delete instead
* Only directory description edit operations are supported in current version, modifying the directory and ad content is not supported yet
* The service automated tests are more integration than unit tests (they need DB)
* Automated build and deploy procedure, using NAnt or similar tool
