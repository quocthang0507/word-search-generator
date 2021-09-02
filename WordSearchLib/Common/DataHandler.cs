using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordSearchLib.Common
{
	public static class DataHandler
	{
		/// <summary>
		/// Read from file and return word list randomly with specific size. 
		/// Return null if the file is not found or the size is greater than length of original word list.
		/// </summary>
		/// <param name="filename">The path of file which containing words</param>
		/// <param name="size">How many words do you want to use?</param>
		/// <returns></returns>
		public static string[] GetWordsFromFile(string filename, int size, LangCharsets lang)
		{
			string[] wordList = ReadWordList(filename);
			if (wordList == null || size > wordList.Length)
				return null;
			string[] wordsSelected = GetRandomWords(wordList, size);
			string[] words = Helper.CapitalizeAll(wordsSelected);
			if (lang == LangCharsets.Vi)
				return words.Select(word => Helper.RemoveDiacriticsInVietnameseString(word)).ToArray();
			return words;
		}

		/// <summary>
		/// Read all text in the file and seperate it into array of string. If the file is not found, this method will return null
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		private static string[] ReadWordList(string filename)
		{
			if (File.Exists(filename))
			{
				char[] delimiters = new char[] { ',', ' ', ';', '\t', '\n', '\r' };
				string[] words = File.ReadAllText(filename).Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
				return words;
			}
			return null;
		}

		/// <summary>
		/// Get random words from the array with specific size
		/// </summary>
		/// <param name="arrayWords"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		private static string[] GetRandomWords(string[] arrayWords, int size)
		{
			List<string> listWords = new();

			for (int counter = 0; counter < size; counter++)
			{
				bool wordAdded = false;
				while (!wordAdded)
				{
					// Select word from random integer
					string wordSelected = arrayWords[Helper.Random(0, arrayWords.Length - 1)];
					// if word not already added to listWords, add
					if (!listWords.Contains(wordSelected))
					{
						listWords.Add(wordSelected);
						wordAdded = true;
					}
				}
			}
			return listWords.ToArray();
		}
	}
}
