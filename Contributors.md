# Contributors

NemeStats is completely open source and 100% community driven. We are always looking for contributors with the following skills:

* Software Development - HTML/JavaScript/CSS/ASP.NET MVC and WebAPI, or quite frankly anything else! We could use API clients in various languages, integrations with other open source solutions, and an Android App :)
* Marketing / Analytics / Lean Startup Thinking - We really need help raising awareness of NemeStats, understanding what the community needs, measuring adoption, etc.
* UX / Graphic Design - We could always use help with improving the user experience of NemeStats.

If you would like contribute to the NemeStats code base please follow the steps below. If you have any questions or issues please contact us at nemestats@gmail.com.

## Instructions

* Pull down source code (e.g. ```git clone https://github.com/NemeStats/NemeStats.git``` in the directory where you want the NemeStats folder to get created).
* Make sure you have a SQL instance running that you have dbo access to. NemeStats uses Entity Framework Code First migrations to create and update the database.
* Run the GetStarted.ps1 PowerShell script in the root folder. You can enter your connection string from here, or use the default connection to .\SQLExpress using integrated windows authentication. This script will create a file called PrivateAppSettings.config in the root of the UI project. This file will contain settings that are sensitive and/or specific to your environment. This file is ignored in .gitignore so that it should never be checked in. Most notably, this file will contain the connection string to your SQL instance where the NerdScorekeeper database runs. If you want to be able to send emails during your testing (e.g. when inviting another Player to your Gaming Group), email us at nemestats@gmail.com and we'll give you a sendGridApiKey to add to your PrivateAppSettings.config.

* Make sure you have an instance of SQL Server or SQL Express running locally, then go to package manager console and run "update-database -ProjectName BusinessLogic". This should create your database and apply all of the migrations to get it current. If it doesn't create the database then just create a local database called "NerdScorekeeper" and then try running the command again.

## JavaScript guidelines

We are trying to keep things simple with our JavaScript. The idea is to use the "Namespace" pattern when writing new javascript files because of the clean syntax.

* Please use the "Namespace()" function from the "Namespace.js" library to define new namespaces. The namespace should be set to the location of the .js file. Please check the code for examples.
* The scripts that have a dependency on a *.cshtml file should be named after them. Scripts that are meant to be reusable and shared between multiple .cshtml files should be done in the form of jQuery plugins and the file name has to make sense based on the functionality intended.
* The following JavaScript example captures the essence of the pattern. It is for the Create.cshtml view of the GameDefinition controller.

```javascript
//Namespace
Namespace("Views.GameDefinition"); //A function that is able to generate or retrieve an existing namespace

//Initialization
Views.GameDefinition.Create = function () {
    var $form = null; //We use the $ character in front of variables to indicate that they are elements selected with jQuery
    var $gameNameInput = null;
    var $boardGameId = null;
};
//Implementation
Views.GameDefinition.Create.prototype = {
    init: function () {
        this.$form = $("form");
        this.$gameNameInput = $("#gameNameInput");
        this.$boardGameId = $("#boardGameId");
    
        var gameDefinitionAutoComplete = new Views.GameDefinition.GameDefinitionAutoComplete();
        gameDefinitionAutoComplete.init(this.$gameNameInput, this.$boardGameId);
    }
    //you can put more functions here ...
}

//Usage
//this is a simple example how  you can use the game definition "Create" script
$(document).ready(function () {
    var gameDefinitionCreator = new Views.GameDefinition.Create();

    gameDefinitionCreator.init();
});
```

* Please put all your jQuery plugins in the UI/Scripts/Plugins folder
* Try to keep separation of concerns in mind and put your JavaScript only in .js files
* Let us know if you have questions or you see code that doesn't conform to the conventions.
* Have fun!

## Compiling SASS

We're using Node Package Manager (npm) for the CSS frameworks. In the repo exists the compiled css and fonts but, if you want to change some style, you need to follow this steps.

### Using Console

* Install nodeJS in your computer: <https://nodejs.org/en/download/>
* Execute the node console and navigate to the UI project folder.
* Execute 'node install'
* To start watch and compile task: 'grunt'

### Using Visual Studio

* Install nodeJS in your computer: <https://nodejs.org/en/download/>
* Install "Task Runner Explorer" extension in your Visual Studio: <https://visualstudiogallery.msdn.microsoft.com/8e1b4368-4afb-467a-bc13-9650572db708/view/Discussions/2>
* Open Visual Studio and open "UI/package.json". Then change anything and save the file to start the dependencies download.
* To start watch and compile task, open the Task Runner Explorer view (View => Other Windows => Task Runner Explorer). In the Task Runner Explorer window execute (double click) the task "default". Now any changes you make to .scss files will automatically trigger css files to recompile.

Following both steps you have already installed all the dependencies in "node_components" folder (ignored in .gitignore) and executing the watch and compile sass grunt task.
Now, when you change any .scss file, the grunt task will detect it and compile the sass files to the /css destination folder.

## T4MVC

NemeStats uses [T4MVC](https://github.com/T4MVC/T4MVC/wiki/Documentation) to provide strong types and names for controllers, actions, and views. If you create new controllers or views, or modify the signature of controller actions then you'll need to go to the root of the UI folder, right click on "T4MVC.tt" => Run Custom Tool for the appropriate generated classes to update.

## What's next?

If you want to add or improve a feature, it would probably be best to run it by us via slack: <https://nemestats.slack.com/messages/feedback/> or email at nemestats@gmail.com. To get your code incorporated into the code base, you'll need to submit a pull request which will be checked out by the team. All business logic should be unit tested using nUnit and Rhino Mocks/Rhino Auto Mocker (if you need mocking).

If you run into any issues or need help getting NemeStats running locally or have other questions about contributing, just email us at nemestats@gmail.com and we'd be happy to help!
