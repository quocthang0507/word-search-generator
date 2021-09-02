using System;
using System.IO;
using System.Text;
using WordSearchLib;
using WordSearchLib.Common;

namespace WordSearch_Console
{
	class Program
	{
		static int Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			string[] arguments = Environment.GetCommandLineArgs();
			// string[] arguments = { "-i", "50fruits.txt", "-o", "puzzle.txt", "-s", "10", "-a" };
			if (arguments.Length == 1 || arguments[1].Equals("-H", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine("Vui lòng thêm các đối số vào sau chương trình này.");
				Console.WriteLine("\nWordSearchMaker -i \"đường dẫn đến tập tin\" -o \"đường dẫn đến tập tin\" [-s số_từ_trong_game -c mã_bộ_ký_tự -a]\n");
				Console.WriteLine("Trong đó:");
				Console.WriteLine("-i tên hoặc đường dẫn đến tập tin danh sách từ bắt buộc.");
				Console.WriteLine("-o tên hoặc đường dẫn đến tập tin xuất bắt buộc.");
				Console.WriteLine("[-s số_từ_trong_game] là số từ được lấy ngẫu nhiên từ tập tin danh sách từ. Mặc định là dùng hết từ.");
				Console.WriteLine("[-c mã_bộ_ký_tự] là Vi hoặc En. Mặc định là En.");
				Console.WriteLine("[-a] có in đáp án không? Mặc định là không.");
				return 1;
			}
			Console.WriteLine("CHƯƠNG TRÌNH TẠO GAME WORD SEARCH PUZZLE");

			string inputArg = GetArgumentByName(arguments, "-i");
			string outputArg = GetArgumentByName(arguments, "-o");
			string sizeArg = GetArgumentByName(arguments, "-s");
			string charsetArg = GetArgumentByName(arguments, "-c");
			string answerArg = GetArgumentByName(arguments, "-a");

			if (inputArg == string.Empty || outputArg == string.Empty)
			{
				Console.WriteLine("Lỗi: Không được bỏ trống đối số -i và -o.");
			}
			else
			{
				if (File.Exists(inputArg))
				{
					bool answer = true;
					if (sizeArg == string.Empty || !int.TryParse(sizeArg, out int size))
					{
						size = 0;
					}
					if (charsetArg == string.Empty || !Enum.TryParse(charsetArg, out LangCharsets charset))
					{
						charset = LangCharsets.En;
					}
					if (answerArg == string.Empty)
					{
						answer = false;
					}
					WordSearch puzzle = new(inputArg, size, charset);
					puzzle.ExportToFile(outputArg, answer);
				}
				Console.WriteLine("Không tìm thấy tập tin -i này");
			}
			Console.WriteLine("Đã tạo xong, mời xem tập tin!");
			return 0;
		}

		static string GetArgumentByName(string[] args, string name)
		{
			int index = Array.FindIndex(args, a => a.Equals(name, StringComparison.OrdinalIgnoreCase));
			if (index != -1 && name.Equals("-a", StringComparison.OrdinalIgnoreCase))
				return "1";
			return index == -1 || index + 1 >= args.Length ? string.Empty : args[index + 1];
		}
	}
}
