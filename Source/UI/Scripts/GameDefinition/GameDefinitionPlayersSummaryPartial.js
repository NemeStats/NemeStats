//Usings
Namespace("Views.GameDefinition");

//Initialization
Views.GameDefinition.GameDefinitionPlayersSummaryPartial = function () {
    
};

//Implementation
Views.GameDefinition.GameDefinitionPlayersSummaryPartial.prototype = {
    initListJs: function () {
        var valueNames = [{ name: 'game-name', attr: 'data-name' }, { name: 'totalpoints-col', attr: 'data-nemepoints' }, 'plays-col', 'avgpoints-col', { name: 'percentage-won', attr: 'data-winpercentage' }];
        var tableId = "gameplayerssummary";


        if (ResponsiveBootstrapToolkit.is('>=md')) {
            new List(tableId, { valueNames: valueNames, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
        } else {
            new List(tableId, { valueNames: valueNames });
        }
    }
}