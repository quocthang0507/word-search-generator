using System;
using System.Linq;

namespace WordSearchLib.Common
{
	public enum LangCharsets
	{
		Vi,
		En
	}

	public static class Helper
	{
		private const string ENG = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string VI = "ABCDEGHIKLMNOPQRSTUVXY";
		private static readonly Random random = new Random();
		private static readonly string[] diacritics = new string[]
		{
			"aAeEoOuUiIdDyY",
			"áàạảãâấầậẩẫăắằặẳẵ",
			"ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
			"éèẹẻẽêếềệểễ",
			"ÉÈẸẺẼÊẾỀỆỂỄ",
			"óòọỏõôốồộổỗơớờợởỡ",
			"ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
			"úùụủũưứừựửữ",
			"ÚÙỤỦŨƯỨỪỰỬỮ",
			"íìịỉĩ",
			"ÍÌỊỈĨ",
			"đ",
			"Đ",
			"ýỳỵỷỹ",
			"ÝỲỴỶỸ"
		};

		/// <summary>
		/// Return capitalised Words array1D
		/// </summary>
		/// <param name="words"></param>
		/// <returns></returns>
		public static string[] CapitalizeAll(string[] words) => words.Select(word => word.ToUpper()).ToArray();

		public static int CountDigits(int number) => number.ToString().Length;

		public static int CountAllCharacters(string[] words)
		{
			int count = 0;
			foreach (string word in words)
			{
				count += word.Length;
			}
			return count;
		}

		public static string GetLongestWord(string[] words) => words.OrderByDescending(s => s.Length).First();

		public static int Random(int start, int end) => random.Next(start, end + 1);

		public static char Random(LangCharsets lang, char start, char end)
		{
			int start_idx, end_idx;
			switch (lang)
			{
				case LangCharsets.Vi:
					start_idx = VI.IndexOf(char.ToUpper(start));
					end_idx = VI.IndexOf(end == '\0' ? 'Y' : end);
					break;
				default:
				case LangCharsets.En:
					start_idx = ENG.IndexOf(char.ToUpper(start));
					end_idx = ENG.IndexOf(end == '\0' ? 'Z' : end);
					break;
			}

			int length = (end_idx - start_idx) + 1;

			// Return default null value('\0') if min or max not letter
			if (start_idx == -1 | end_idx == -1)
			{
				return '\0';
			}

			string charSet = lang switch
			{
				LangCharsets.Vi => VI.Substring(start_idx, length),
				_ => ENG.Substring(start_idx, length),
			};
			char randomChar = charSet[Random(0, charSet.Length - 1)];

			return randomChar;
		}

		public static string RemoveDiacriticsInVietnameseString(string text)
		{
			for (int i = 1; i < diacritics.Length; i++)
			{
				for (int j = 0; j < diacritics[i].Length; j++)
					text = text.Replace(diacritics[i][j], diacritics[0][i - 1]);
			}
			return text;
		}
	}
}