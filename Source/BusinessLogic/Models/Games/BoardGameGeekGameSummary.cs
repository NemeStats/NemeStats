﻿namespace BusinessLogic.Models.Games
{
    public class BoardGameGeekGameSummary
    {
        public BoardGameGeekInfo BoardGameGeekInfo { get; set; }
        public UniversalGameStats UniversalGameStats { get; set; }
        public GameDefinitionSummary GamingGroupGameDefinitionSummary { get; set; }
    }
}