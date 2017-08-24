[![Stories in Ready](https://badge.waffle.io/nemestats/nemestats.png?label=ready&title=Ready)](https://waffle.io/nemestats/nemestats)
NemeStats  [![Build status](https://ci.appveyor.com/api/projects/status/q5d26a5d8v7occ16?svg=true)](https://ci.appveyor.com/project/cracker4o/nemestats) [![Code Triagers Badge](https://www.codetriage.com/nemestats/nemestats/badges/users.svg)](https://www.codetriage.com/nemestats/nemestats)
===============

NemeStats.com was created as a fun and completely free website for tracking games played and won among a fairly stable group of players. Recording your games will reveal each player's Nemesis (and their Minions), will assign Champions to games (and other badges), will award [Achievements](https://nemestats.com/achievements) and will provide many other interesting statistics.

NemeStats has [a full-featured REST API](http://docs.nemestatsapiversion2.apiary.io/#) which you are completely entitled to use as a back-end for your own site or application.
The API is currently at version 2. For more information on the REST API, check out the [Apiary documentation](http://docs.nemestatsapiversion2.apiary.io/#).

The site is actively being developed by a number off contributors. The code base is being constantly refactored in an effort to maintain a clean and sustainable project. We encourage constructive feedback, feature requests, or even pull requests if you'd like to add a feature yourself (but please TDD or at least unit test it :)).

The following GitHub projects and corresponding NuGet packages were spawned as a result of building NemeStats.com:
* [Universal Analytics for DotNet](https://github.com/RIDGIDSoftwareSolutions/Universal-Analytics-For-DotNet) - This is a .NET wrapper over Google's Universal Analytics Measurement Protocol and is used to push custom events to Universal Analytics from the server side.
* [Versioned REST API](https://github.com/RIDGIDSoftwareSolutions/versioned-rest-api) - This simple project allows us to easily handle versioning of the REST API via the url (e.g. /api/v2/Players).

If you are interested in contributing, check out [the contributors readme](https://github.com/NemeStats/NemeStats/blob/master/Contributors.md)

Catch us @nemestats or @jakejgordon on Twitter, or via nemestats@gmail.com.

### Manually Regenerating Sitemaps
To manually regenerate sitemap.xml files, run the RegenerateSitemapsIntegrationTests.It_Regenerates_All_Of_The_Sitemaps_And_The_Sitemap_Index_Files() integration test with the BusinessLogic.Tests app.config pointed to the production database. Files will be placed in the designated location from the app.config sitemapLocationFilePath app setting. These files then need to be copied into the Web.UI/sitemaps/ folder to overwrite that is there. Make sure you don't check in the production database connection string!
