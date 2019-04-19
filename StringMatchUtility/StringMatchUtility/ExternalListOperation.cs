using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StringMatchUtility
{
    public class ExternalListOperation
    {

        /// <summary>
        /// Read CSV file to Dictionary
        /// </summary>
        /// <param name="filePath">File path of external list</param>
        /// <returns>A Dictionary</returns>
        private static Dictionary<string, string> ImportCSVFileToDictionary(string filePath)
        {
            var dictionary = File.ReadLines(filePath).Select(line => line.Split(',')).ToDictionary(line => line[0], line => line[1]);
            return dictionary;
        }

        /// <summary>
        /// Get the best match result from an external file list
        /// </summary>
        /// <param name="listFilePath">External list file path (only 2 columns ID (unique), Value)</param>
        /// <param name="sourceValue">Value to compare against list</param>
        /// <param name="minimumMatchingRequired">Minimum matching reauirement needed</param>
        /// <returns>The best match, empty if nothing found</returns>
        public static string GetBestMatchResult(string listFilePath, string sourceValue, double minimumMatchingRequired)
        {
            Dictionary<string, double> matchingResultsDictionary = new Dictionary<string, double>();
            double matchingPercent = 0;
            var externalListDictionary = ImportCSVFileToDictionary(@listFilePath);

            //Looping on each value in external list
            foreach (KeyValuePair<string, string> externalEntry in externalListDictionary)
            {
                //Get matching percentage against source value
                matchingPercent = StringMatchUtility.LevenshteinDistance.CalculateSimilarity(sourceValue.ToLower(), externalEntry.Value.ToLower());

                if (matchingPercent >= minimumMatchingRequired)
                {
                    if (!matchingResultsDictionary.ContainsKey(externalEntry.Value))
                    {
                        //Adding external entry to results list
                        matchingResultsDictionary.Add(externalEntry.Value, matchingPercent);
                    }
                }
            }

            if (matchingResultsDictionary.Count != 0)
            {
                var sortedMatchingList = from pair in matchingResultsDictionary orderby pair.Value descending select pair;
                return sortedMatchingList.First().Key;
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Get the best match result from an external file list
        /// </summary>
        /// <param name="listFilePath">External list file path (only 2 columns ID (unique), Value)</param>
        /// <param name="sourceValue">Value to compare against list</param>
        /// <param name="minimumMatchingRequired">Minimum matching reauirement needed</param>
        /// <param name="differenceWithNextBest">Difference with next best entry</param>
        /// <returns>The best match, empty if nothing found</returns>
        public static string GetBestMatchResult(string listFilePath, string sourceValue, double minimumMatchingRequired, double differenceWithNextBest)
        {
            Dictionary<string, double> matchingResultsDictionary = new Dictionary<string, double>();
            double matchingPercent = 0;
            var externalListDictionary = ImportCSVFileToDictionary(@listFilePath);
            string returnResult = string.Empty;

            //Looping on each value in external list
            foreach (KeyValuePair<string, string> externalEntry in externalListDictionary)
            {
                //Get matching percentage against source value
                matchingPercent = StringMatchUtility.LevenshteinDistance.CalculateSimilarity(sourceValue.ToLower(), externalEntry.Value.ToLower());

                if (matchingPercent >= minimumMatchingRequired)
                {
                    if (!matchingResultsDictionary.ContainsKey(externalEntry.Value))
                    {
                        //Adding external entry to results list
                        matchingResultsDictionary.Add(externalEntry.Value, matchingPercent);
                    }
                }
            }

            if (matchingResultsDictionary.Count > 1)
            {
                var sortedMatchingList = from pair in matchingResultsDictionary orderby pair.Value descending select pair;

                if (sortedMatchingList.First().Value == 1)
                    returnResult = sortedMatchingList.First().Key;
                else
                {
                    if (sortedMatchingList.ElementAt(0).Value - sortedMatchingList.ElementAt(1).Value >= differenceWithNextBest)
                        returnResult = sortedMatchingList.First().Key;
                }
            }
            else if (matchingResultsDictionary.Count == 1)
            {
                returnResult = matchingResultsDictionary.First().Key;
            }
            else
                returnResult = string.Empty;

            return returnResult;
        }

        /// <summary>
        /// Get a result value from a list where searching value matches one item in specific column
        /// </summary>
        /// <param name="filePath">External list file path</param>
        /// <param name="searchingValue">String value to be found</param>
        /// <param name="searchingColumnNumber">In which column to search</param>
        /// <param name="targetColumnNumber">If searching value found column item to extract</param>
        /// <returns>Item value from list</returns>
        public static string GetItemFomCsvMatchingSpecificColumnValue(string filePath, string searchingValue, int searchingColumnNumber, int targetColumnNumber)
        {
            string returnValue = string.Empty;

            Dictionary<String, String[]> Externals = File
                .ReadLines(@filePath)
                .Select(line => line.Split(','))
                .ToDictionary(
                  items => items[0],
                  items => items
                );

            foreach (var item in Externals.Values)
            {
                if (item[searchingColumnNumber].ToString().ToLower() == searchingValue.ToLower())
                {
                    returnValue = item[targetColumnNumber].ToString();
                    break;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Get entire line value from a list where searching value matches one item in specific column
        /// </summary>
        /// <param name="filePath">External list file path</param>
        /// <param name="searchingValue">String value to be found</param>
        /// <param name="searchingColumnNumber">In which column to search</param>
        /// <returns>Entire line from list</returns>
        public static string GetLineFomCsvMatchingSpecificColumnValue(string filePath, string searchingValue, int searchingColumnNumber)
        {
            string returnValue = string.Empty;

            Dictionary<String, String[]> Externals = File
                .ReadLines(@filePath)
                .Select(line => line.Split(','))
                .ToDictionary(
                  items => items[0],
                  items => items
                );

            foreach (var item in Externals.Values)
            {
                if (item[searchingColumnNumber].ToString().ToLower() == searchingValue.ToLower())
                {
                    returnValue = string.Join(",", item);
                    break;
                }
            }

            return returnValue;
        }

    }
}
