var gbl_playerRank = 1;
var gbl_playerIndex = 0;

$(document).ready(function () {
    $("#rankedPlayers").sortable({ stop: onReorder });
    $("#Players").change(addPlayer);
});

function onReorder()
{
    $("#rankedPlayers li").each(function (index, value) {
        var listItem = $(value);
        var playerId = listItem.attr("data-playerId");
        var rank = index + 1;
        $("#" + playerId).val(rank);

        var playerName = listItem.attr("data-playerName");
        listItem.html(generatePlayerRankListItemString(playerName, rank));
    });
}

function generatePlayerRankListItemString(playerName, playerRank) {
    return playerName + " - Rank: " + playerRank;
}

function addPlayer() {
    var selectedOption = $("#Players").find(":selected");

    if (selectedOption.text() == "Add A Player") {
        return alert("You must pick a player.");
    }
    var playerId = selectedOption.val();
    var playerName = selectedOption.text();
    var playerRow = "<input type='hidden' name='PlayerGameResults[" + gbl_playerIndex + "].PlayerId' value='" + playerId + "'/>"
        + "<input type='hidden' id='" + playerId + "' name='PlayerGameResults[" + gbl_playerIndex + "].GameRank' value='" + gbl_playerRank + "'/>";
    $("#playerFormData").append(playerRow);

    var playerItem = "<li id='li" + playerId + "' data-playerId='" + playerId + "' data-playerName='" + playerName + "'>" + generatePlayerRankListItemString(playerName, gbl_playerRank) + "</li>";
    $("#rankedPlayers").append(playerItem);

    gbl_playerIndex++;
    gbl_playerRank++
    selectedOption.remove();
}