# Dirtybase

[![Build status](https://ci.appveyor.com/api/projects/status/c0h9y0pn0rumkj0v/branch/master?svg=true)](https://ci.appveyor.com/project/Pieter/dirtybase/branch/master)

Still Under Construction

---
Dirtybase is a persistence version/migration command line tool.

Dirtybase will compare `version` files in a folder to the version of a data store, then apply the version files in order to migrate the data store to the latest version. 

This is useful for keeping data stores on different environments up to date and automating deployments. 

Supported Data stores

* SQLite
* MS SQL (in development)

###Usage

**Init**

Initializes Dirtybase on the data store. This action is only required once.
 
`> init -db {database type} -cs {connection string}`

**Migrate**

Compares version files to applied data store versions, and applies version files not yet applied to the data store.
 
`> migrate -db {database type} -cs {connection string} -vf {path to version script folder}`

**Help**

`> help`

###Database Types

* sqlite
* sql

###Version Naming

Version files should follow the convention `v{version}_{filename}.{fileExtension}.`

Example: `v1.5_CreateFooSchema.sql`.

Any version text is allowed but should not include `_`.

Any folder structure in the version folder is allowed. 

###Version Sorting

Version are sorted using a natural sorting order.

* a before b
* a before 1
* 22 before 113
* 1.1.15 before 1.1.115
* a.2.b before a.3.b

###Contribution
To add a new data store type just add to the enum `DatabaseType` in the `Options\DatabaseType.cs` file. This will cause the unit tests to fail, detailing which files you need to add and implement.

Please write sufficient unit tests for new behavior.

