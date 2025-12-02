# What is this project?

This is a .Net project that that allows you to interact with a database about people with the help of a browser.

# Logs database
Before you could build the solution, make sure to create a "Logs" database, because SeriLog expects it to exist. The connection string can be found in the PeopleDatabase project's appsettings.json file.

# Migrations

Before you could start the project, you need to run the "Update-Database" package-manager console command on the "Entities" project, to apply the database migrations.
