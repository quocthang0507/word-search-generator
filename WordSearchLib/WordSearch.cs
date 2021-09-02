using System;
using WordSearch.Common;
using WordSearchLib.Common;

namespace WordSearchLib
{
	public class WordSearch
	{
		/// <summary>
		/// The word list is used for current game
		/// </summary>
		public string[] Words { get; private set; }
		public char[,] Grid { get; private set; }
		public Lang LangCharset { get; private set; }

		public WordSearch(string filename, int size, Lang lang)
		{
			Words = DataHandler.GetWordsFromFile(filename, size);
			if (Words == null)
				throw new Exception();
			LangCharset = lang;
		}

		/// <summary>
		/// Handle setup Grid size, populate
		/// </summary>
		public void HandleSetupGrid()
		{
			InitializeEmptyGrid();
			PlaceWordsIntoGrid(Words);
			PopulateEmptyElements(LangCharset);
		}

		/// <summary>
		/// Initialize an empty Grid
		/// </summary>
		private void InitializeEmptyGrid()
		{
			int numChars = Helper.CountElements(Words);
			int longestWordLen = Helper.GetLongestWord(Words).Length;
			int numRows = GetGridSize(numChars, longestWordLen);
			Grid = new char[numRows, numRows];
		}

		private int GetGridSize(int numChars, int longestWordLen)
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
		/// <param name="Grid"></param>
		private void PlaceWordsIntoGrid(string[] words)
		{
			int numberWordsToPlace = Helper.CountElements(words);

			// iterate Words to place
			for (int wordCurrent = 0; wordCurrent < numberWordsToPlace; wordCurrent++)
			{
				bool wordPlaced = false;
				while (!wordPlaced)
				{
					// Get random starting point for word
					GridCell coord = new(Helper.Random(0, Grid.GetLength(0) - 1), Helper.Random(0, Grid.GetLength(1) - 1));
					if (PlaceWordIntoGrid(coord, words[wordCurrent]))
					{
						wordPlaced = true;
					}
				}
			}
		}

		private bool PlaceWordIntoGrid(GridCell position, string word)
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
				if (Grid[x, y] == '\0' | Grid[x, y] == word[0])
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
								PlaceWordRight(word, position);
								break;
							case 2:
								PlaceWordLeft(word, position);
								break;
							case 3:
								PlaceWordDown(word, position);
								break;
							case 4:
								PlaceWordUp(word, position);
								break;
							case 5:
								PlaceWordUpRight(word, position);
								break;
							case 6:
								PlaceWordDownRight(word, position);
								break;
							case 7:
								PlaceWordUpLeft(word, position);
								break;
							case 8:
								PlaceWordDownLeft(word, position);
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
		/// <param name="Grid"></param>
		private void PopulateEmptyElements(Lang lang)
		{
			for (int i = 0; i < Grid.GetLength(0); i++)
			{
				for (int j = 0; j < Grid.GetLength(1); j++)
				{
					if (Grid[i, j] == '\0')
					{
						Grid[i, j] = Helper.Random(lang, 'a', '\0');
					}
				}
			}
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
					if (Grid[pos.RowIndex, pos.ColIndex + counter] != '\0' && Grid[pos.RowIndex, pos.ColIndex + counter] != word[counter])
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
					if (Grid[pos.RowIndex, pos.ColIndex - counter] != '\0' && Grid[pos.RowIndex, pos.ColIndex - counter] != word[counter])
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
					if (Grid[pos.RowIndex + counter, pos.ColIndex] != '\0' && Grid[pos.RowIndex + counter, pos.ColIndex] != word[counter])
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
					if (Grid[pos.RowIndex - counter, pos.ColIndex] != '\0' && Grid[pos.RowIndex - counter, pos.ColIndex] != word[counter])
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
					if (Grid[pos.RowIndex - counter, pos.ColIndex + counter] != '\0' && Grid[pos.RowIndex - counter, pos.ColIndex + counter] != word[counter])
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
					if (Grid[pos.RowIndex + counter, pos.ColIndex + counter] != '\0' && Grid[pos.RowIndex + counter, pos.ColIndex + counter] != word[counter])
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
					if (Grid[pos.RowIndex - counter, pos.ColIndex - counter] != '\0' && Grid[pos.RowIndex - counter, pos.ColIndex - counter] != word[counter])
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
					if (Grid[pos.RowIndex + counter, pos.ColIndex - counter] != '\0' && Grid[pos.RowIndex + counter, pos.ColIndex - counter] != word[counter])
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
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private void PlaceWordRight(string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				Grid[pos.RowIndex, pos.ColIndex + counter] = word[counter];
			}
		}

		/// <summary>
		/// Place word right -> left
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private void PlaceWordLeft(string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				Grid[pos.RowIndex, pos.ColIndex - counter] = word[counter];
			}
		}

		/// <summary>
		/// Place word up -> down
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private void PlaceWordDown(string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				Grid[pos.RowIndex + counter, pos.ColIndex] = word[counter];
			}
		}

		/// <summary>
		/// Place word down -> up
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private void PlaceWordUp(string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				Grid[pos.RowIndex - counter, pos.ColIndex] = word[counter];
			}
		}

		/// <summary>
		/// Place word diagonal left -> up right
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private void PlaceWordUpRight(string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				Grid[pos.RowIndex - counter, pos.ColIndex + counter] = word[counter];
			}
		}

		/// <summary>
		/// Place word diagonal left -> down right
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private void PlaceWordDownRight(string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				Grid[pos.RowIndex + counter, pos.ColIndex + counter] = word[counter];
			}
		}

		/// <summary>
		/// Place word diagonal left -> up left
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private void PlaceWordUpLeft(string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				Grid[pos.RowIndex - counter, pos.ColIndex - counter] = word[counter];
			}
		}

		/// <summary>
		/// Place word diagonal left -> down left
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="Grid"></param>
		private void PlaceWordDownLeft(string word, GridCell pos)
		{
			for (int counter = 0; counter < word.Length; counter++)
			{
				Grid[pos.RowIndex + counter, pos.ColIndex - counter] = word[counter];
			}
		}
		#endregion
	}
}
