//Usings
Namespace("Views.Achievement");

//Initialization
Views.Achievement.Details = function () {
    
};

//Implementation
Views.Achievement.Details.prototype = {
    init: function () {
        this.initListJs();
    },
    initListJs: function () {

        var gamedefinitionsValues = [{ name: 'name-col', attr: 'data-name' }];
        var gameDefinitionTableId = "playedgameslist";

        if (ResponsiveBootstrapToolkit.is('>=md')) {
            new List(gameDefinitionTableId, { valueNames: gamedefinitionsValues, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
        } else {
            new List(gameDefinitionTableId, { valueNames: gamedefinitionsValues });
        }
    },
};