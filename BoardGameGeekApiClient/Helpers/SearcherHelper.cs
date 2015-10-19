using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Models;

namespace BoardGameGeekApiClient.Helpers
{
    public static class SearcherHelper
    {
        public static IEnumerable<SearchBoardGameResult> SortSearchResults(this IEnumerable<SearchBoardGameResult> searchResults, string searchText, int maxResultsToReturn =10)
        {
            var queryable = searchResults.AsQueryable();
            var results = new List<SearchBoardGameResult>();
            //first add exact matches
            results.AddRange(queryable.Where(result => result.BoardGameName.Equals(searchText, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(result => result.YearPublished)
                .Take(maxResultsToReturn));

            if (results.Count >= maxResultsToReturn)
            {
                return results;
            }

            //then add anything that starts with but isn't an exact match
            results.AddRange(queryable.Where(result => result.BoardGameName.StartsWith(searchText, StringComparison.InvariantCultureIgnoreCase)
                    && !result.BoardGameName.Equals(searchText, StringComparison.InvariantCultureIgnoreCase))
                    .OrderBy(result => result.BoardGameName.Length)
                    .ThenByDescending(result => result.YearPublished)
                .Take(maxResultsToReturn));

            if (results.Count >= maxResultsToReturn)
            {
                return results;
            }
            //then add everything else
            results.AddRange(queryable.Where(result => !result.BoardGameName.StartsWith(searchText, StringComparison.InvariantCultureIgnoreCase)
                && !result.BoardGameName.Equals(searchText, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(result => result.YearPublished)
                .Take(maxResultsToReturn));

            return results;
        }
    }
}
