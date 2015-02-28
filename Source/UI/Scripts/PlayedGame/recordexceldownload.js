//Usings
Namespace("Views._PlayedGamesPartial");

//Initialization
Views._PlayedGamesPartial.EventTracking = function () {
    this._googleAnalytics = null;
    this.$excelDownloadAnchor = null;
};

//Implementation
Views._PlayedGamesPartial.EventTracking.prototype = {
    
    //Method definitions
    init: function (gaObject) {
        var parent = this;
        this._googleAnalytics = gaObject;
        this.$excelDownloadAnchor = $("#playedGamesExcelDownload");

        //Event handlers
        this.$excelDownloadAnchor.on("click", function() {
            parent._googleAnalytics.trackGAEvent("PlayedGames", "ExcelFileExportClicked", "FromPlayedGamesPartial");
        });
    }
}; //end prototypes