If you would like contribute to the NemeStats code base please follow the steps below. If you have any questions or issues please contact us at nemestats@gmail.com.

* Pull down source code
* Make sure you have an instance of SQL Server or SQL Express running locally, then go to package manager console and run "update-database -ProjectName BusinessLogic". This should create your database and apply all of the migrations to get it current. If it doesn't create the database then just create a local database called "Nerdscorekeeper" and then try running the command again.
* You'll have to create a file in the root of your UI project called PrivateAppSettings.config that has settings for keys that we don't want checked into source control. If you are serious about contributing something, we can send you this file to include locally. If you are just playing around then below is a version that should let you at least run the project.
PrivateAppSettings.config example settings

```
<appSettings>
  <add key="Rollbar.AccessToken" value="f7d858b2d08a47b98f9ab19f9e27516c" />
  <add key="Rollbar.Environment" value="development" />
  <add key="UniversalAnalytics.TrackingId" value="UA-52982625-4" />
  <add key="googleAppId" value="260011030385-4651b4m7fsn28tc9vj13remofgji8m3l.apps.googleusercontent.com" />
  <add key="googleClientSecret" value="rEWsFL0AKxKODzJpNR6X4A2f" />
  <add key="emailServiceUserName" value="not provided -- emails wont send when running this locally" />
  <add key="emailServicePassword" value="not provided -- emails wont send when running this locally" />
  <add key="emailConfirmationCallbackUrl" value="https://localhost:44300/Account/ConfirmEmail" />
  <add key="authTokenSalt" value="A3BCDF34-BA8C-4A02-92CA-AAACDF7993F3"/>
</appSettings>
```
