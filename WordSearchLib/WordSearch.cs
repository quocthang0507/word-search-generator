using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WordSearchLib.Common;

namespace WordSearchLib
{
	public class WordSearch
	{
		private const char NULL = '\0';

		/// <summary>
		/// The word list is used for current game
		/// </summary>
		public string[] Words { get; private set; }
		/// <summary>
		/// The grid for finding words
		/// </summary>
		public char[,] Grid { get; private set; }
		/// <summary>
		/// The grid with only answers
		/// </summary>
		public List<char[,]> GridAnswers { get; private set; }
		/// <summary>
		/// Vietnamese or English charset
		/// </summary>
		public LangCharsets LangCharset { get; private set; }

		public WordSearch(string filename, int size, LangCharsets charset)
		{
			Words = DataHandler.GetWordsFromFile(filename, size, charset);
			if (Words == null)
				throw new Exception();
			LangCharset = charset;
			ProcessingWordGrids();
		}

		public void ExportToFile(string filename, bool answers = false)
		{
			StringBuilder builder = new();
			builder.AppendLine("WORD SEARCH PUZZLE\n");
			builder.AppendLine("Word List:");
			foreach (var word in Words)
			{
				builder.Append(word + '\t');
			}
			builder.AppendLine("\n\nYour puzzle:\n");
			builder.AppendLine(GetAsString(Grid));
			if (answers)
			{
				builder.AppendLine("\nAnswers:\n");
				foreach (var ans in GridAnswers)
				{
					builder.AppendLine(GetAsString(ans) + "\n");
				}
			}
			ExportString(filename, builder.ToString());
		}

		/// <summary>
		/// Handle setup Grid size, populate
		/// </summary>
		private void ProcessingWordGrids()
		{
			InitializeEmptyGrids();
			PlaceWordsIntoGrid(Words);
			PlaceRandomChars(LangCharset);
		}

		/// <summary>
		/// Initialize an empty Grid
		/// </summary>
		private void InitializeEmptyGrids()
		{
			// Initialize variables
			int numChars = Helper.CountAllCharacters(Words);
			int longestWordLen = Helper.GetLongestWord(Words).Length;
			int numRows = GetGridSize(numChars, longestWordLen);

			// Initialize grids
			Grid = new char[numRows, numRows];
			GridAnswers = new List<char[,]>();
			for (int i = 0; i < numChars; i++)
			{
				GridAnswers.Add(new char[numRows, numRows]);
			}
		}

		private static int GetGridSize(int numChars, int longestWordLen)
		{
			int minGridArea = longestWordLen * longestWordLen;

			// add extra Grid elements to ensure enough space for non-wordCurrent characters
			int totalElementsGrid = numChars * 3;

			int totalElementsGridSquare = (int)Math.Sqrt(totalElementsGrid);

			// increase current number of Grid elements until reaches next root of square (e.g. 5, 6, 7)
			while (Math.Sqrt(minGridArea) != totalElementsGridSquare + 1)
			{
				minGridArea++;
			}

			// get number of rows/cols
			int numRowsCols = (int)Math.Sqrt(minGridArea);
			return numRowsCols;
		}

		/// <summary>
		/// Place all words into Grid
		/// </summary>
		/// <param name="words"></param>
		private void PlaceWordsIntoGrid(string[] words)
		{
			// iterate Words to place
			for (int word_idx = 0; word_idx < Words.Length; word_idx++)
			{
				bool wordPlaced = false;
				while (!wordPlaced)
				{
					// Get random starting point for word
					GridCell pos = new(Helper.Random(0, Grid.GetLength(0) - 1), Helper.Random(0, Grid.GetLength(1) - 1));
					if (PlaceWordIntoGrid(Grid, pos, words[word_idx]))
					{
						wordPlaced = true;
						PlaceWordIntoGrid(GridAnswers[word_idx], pos, words[word_idx]);
					}
				}
			}
		}

		private bool PlaceWordIntoGrid(char[,] gridAnswer, GridCell position, string word)
		{
			int x = position.RowIndex;
			int y = position.ColIndex;

			// elements represent placements options, 0 == left->right, 1 = right->left, etc. (in order presented below)
			int[] placementOptions = new int[8] { 9, 9, 9, 9, 9, 9, 9, 9 };
			int placementOption = 9;
			bool haveOptions = false;

			for (int counter = 0; counter < word.Length; counter++)
			{
				// If point empty or point contains same letter word's current character
				if (Grid[x, y] == NULL | Grid[x, y] == word[0])
				{
					if (SpaceRight(word, position))
					{
						placementOptions[0] = 1;
						haveOptions = true;
					}
					if (SpaceLeft(word, position))
					{
						placementOptions[1] = 2;
						haveOptions = true;
					}
					if (SpaceDown(word, position))
					{
						placementOptions[2] = 3;
						haveOptions = true;
					}
					if (SpaceUp(word, position))
					{
						placementOptions[3] = 4;
						haveOptions = true;
					}
					if (SpaceUpRight(word, position))
					{
						placementOptions[4] = 5;
						haveOptions = true;
					}
					if (SpaceDownRight(word, position))
					{
						placementOptions[5] = 6;
						haveOptions = true;
					}
					if (SpaceUpLeft(word, position))
					{
						placementOptions[6] = 7;
						haveOptions = true;
					}
					if (SpaceDownLeft(word, position))
					{
						placementOptions[7] = 8;
						haveOptions = true;
					}

					if (haveOptions)
					{
						while (placementOption == 9)
						{
							placementOption = placementOptions[Helper.Random(0, placementOptions.Length - 1)];
						}

						switch (placementOption)
						{
							case 1:
								PlaceWordRight(Grid, word, position);
								PlaceWordRight(gridAnswer, word, position);
								break;
							case 2:
								PlaceWordLeft(Grid, word, position);
								PlaceWordLeft(gridAnswer, word, position);
								break;
							case 3:
								PlaceWordDown(Grid, word, position);
								PlaceWordDown(gridAnswer, word, position);
								break;
							case 4:
								PlaceWordUp(Grid, word, position);
								PlaceWordUp(gridAnswer, word, position);
								break;
							case 5:
								PlaceWordUpRight(Grid, word, position);
								PlaceWordUpRight(gridAnswer, word, position);
								break;
							case 6:
								PlaceWordDownRight(Grid, word, position);
								PlaceWordDownRight(gridAnswer, word, position);
								break;
							case 7:
								PlaceWordUpLeft(Grid, word, position);
								PlaceWordUpLeft(gridAnswer, word, position);
								break;
							case 8:
								PlaceWordDownLeft(Grid, word, position);
								PlaceWordDownLeft(gridAnswer, word, position);
								break;
						}
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Place random characters into empty Grid cell
		/// </summary>
		private void PlaceRandomChars(LangCharsets lang)
		{
			for (int i = 0; i < Grid.GetLength(0); i++)
			{
				for (int j = 0; j < Grid.GetLength(1); j++)
				{
					if (Grid[i, j] == NULL)
					{
						Grid[i, j] = Helper.Random(lang, 'a', NULL);
					}
				}
			}
		}

		/// <summary>
		/// Export a string to a file and can append if this file is existed
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="grid"></param>
		private void ExportString(string filename, string text)
		{
			StreamWriter writer = new(filename, true);
			writer.Write(text);
			writer.Close();
		}

		/// <summary>
		/// Export a grid to a file and can append if this file is existed
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="grid"></param>
		private static string GetAsString(char[,] grid)
		{
			StringBuilder builder = new();
			for (int i = 0; i < grid.GetLength(0); i++)
			{
				for (int j = 0; j < grid.GetLength(1); j++)
				{
					if (grid[i, j] == NULL)
						builder.Append('_');
					else
						builder.Append(grid[i, j]);
					builder.Append('\t');
				}
				builder.AppendLine();
			}
			return builder.ToString();
		}

		#region Check words fit in Grid

		/// <summary>
		/// Check space left -> right
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		/// <returns></returns>
		private bool SpaceRight(string word, GridCell pos)
		{
			if ((Grid.GetLength(0)) - pos.ColIndex >= word.Length)
			{
				// iterate right in row, checking each successive element empty or same as current char
				for (int counter = 0; counter < word.Length; counter++)
				{
					if (Grid[pos.RowIndex, pos.ColIndex + counter] != NULL && Grid[pos.RowIndex, pos.ColIndex + counter] != word[counter])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check space right -> left
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		/// <returns></returns>
		private bool SpaceLeft(string word, GridCell pos)
		{
			if (pos.ColIndex >= word.Length - 1)
			{
				// iterate left in row, checking each successive element empty or same as current char
				for (int counter = 0; counter < word.Length; counter++)
				{
					if (Grid[pos.RowIndex, pos.ColIndex - counter] != NULL && Grid[pos.RowIndex, pos.ColIndex - counter] != word[counter])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check space up -> down
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		/// <returns></returns>
		private bool SpaceDown(string word, GridCell pos)
		{
			if ((Grid.GetLength(0)) - pos.RowIndex >= word.Length)
			{
				// iterate right in row, checking each successive element empty or same as current char
				for (int counter = 0; counter < word.Length; counter++)
				{
					if (Grid[pos.RowIndex + counter, pos.ColIndex] != NULL && Grid[pos.RowIndex + counter, pos.ColIndex] != word[counter])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check space down -> up
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		/// <returns></returns>
		private bool SpaceUp(string word, GridCell pos)
		{
			if (pos.RowIndex >= word.Length - 1)
			{
				// iterate left in row, checking each successive element empty or same as current char
				for (int counter = 0; counter < word.Length; counter++)
				{
					if (Grid[pos.RowIndex - counter, pos.ColIndex] != NULL && Grid[pos.RowIndex - counter, pos.ColIndex] != word[counter])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check space diagonal left -> up right
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		/// <returns></returns>
		private bool SpaceUpRight(string word, GridCell pos)
		{
			if ((Grid.GetLength(0)) - pos.ColIndex >= word.Length && // if space right
				(pos.RowIndex >= word.Length - 1)) // if space up
			{
				// iterate right in row, checking each successive element empty or same as current char
				for (int counter = 0; counter < word.Length; counter++)
				{
					if (Grid[pos.RowIndex - counter, pos.ColIndex + counter] != NULL && Grid[pos.RowIndex - counter, pos.ColIndex + counter] != word[counter])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check space diagonal left -> down right
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		/// <returns></returns>
		private bool SpaceDownRight(string word, GridCell pos)
		{
			if ((Grid.GetLength(0)) - pos.ColIndex >= word.Length && // if space right
				(Grid.GetLength(1)) - pos.RowIndex >= word.Length) // if space down
			{
				// iterate right in row, checking each successive element empty or same as current char
				for (int counter = 0; counter < word.Length; counter++)
				{
					if (Grid[pos.RowIndex + counter, pos.ColIndex + counter] != NULL && Grid[pos.RowIndex + counter, pos.ColIndex + counter] != word[counter])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check space diagonal left -> up right
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		/// <returns></returns>
		private bool SpaceUpLeft(string word, GridCell pos)
		{
			if (pos.RowIndex >= word.Length - 1 && // if space up
				pos.ColIndex >= word.Length - 1) // if space left
			{
				// iterate right in row, checking each successive element empty or same as current char
				for (int counter = 0; counter < word.Length; counter++)
				{
					if (Grid[pos.RowIndex - counter, pos.ColIndex - counter] != NULL && Grid[pos.RowIndex - counter, pos.ColIndex - counter] != word[counter])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check space diagonal left -> up right
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		/// <returns></returns>
		private bool SpaceDownLeft(string word, GridCell pos)
		{
			if ((Grid.GetLength(0)) - pos.RowIndex >= word.Length && // if space down
				pos.ColIndex >= word.Length - 1) // if space left
			{
				// iterate right in row, checking each successive element empty or same as current char
				for (int counter = 0; counter < word.Length; counter++)
				{
					if (Grid[pos.RowIndex + counter, pos.ColIndex - counter] != NULL && Grid[pos.RowIndex + counter, pos.ColIndex - counter] != word[counter])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		#endregion

		#region Word placement in Grid

		/// <summary>
		/// Place word left -> right
		/// </summary>
		/// <param name="Grid"></param>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		private static void PlaceWordRight(char[,] grid, string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				grid[pos.RowIndex, pos.ColIndex + counter] = word[counter];
			}
		}

		/// <summary>
		/// Place word right -> left
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private static void PlaceWordLeft(char[,] grid, string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				grid[pos.RowIndex, pos.ColIndex - counter] = word[counter];
			}
		}

		/// <summary>
		/// Place word up -> down
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private static void PlaceWordDown(char[,] grid, string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				grid[pos.RowIndex + counter, pos.ColIndex] = word[counter];
			}
		}

		/// <summary>
		/// Place word down -> up
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private static void PlaceWordUp(char[,] grid, string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				grid[pos.RowIndex - counter, pos.ColIndex] = word[counter];
			}
		}

		/// <summary>
		/// Place word diagonal left -> up right
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private static void PlaceWordUpRight(char[,] grid, string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				grid[pos.RowIndex - counter, pos.ColIndex + counter] = word[counter];
			}
		}

		/// <summary>
		/// Place word diagonal left -> down right
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private static void PlaceWordDownRight(char[,] grid, string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				grid[pos.RowIndex + counter, pos.ColIndex + counter] = word[counter];
			}
		}

		/// <summary>
		/// Place word diagonal left -> up left
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private static void PlaceWordUpLeft(char[,] grid, string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				grid[pos.RowIndex - counter, pos.ColIndex - counter] = word[counter];
			}
		}

		/// <summary>
		/// Place word diagonal left -> down left
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private static void PlaceWordDownLeft(char[,] grid, string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				grid[pos.RowIndex + counter, pos.ColIndex - counter] = word[counter];
			}
		}
		#endregion
	}
}
