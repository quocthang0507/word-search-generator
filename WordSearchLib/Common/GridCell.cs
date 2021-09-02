namespace WordSearchLib.Common
{
	public class GridCell
	{
		public int RowIndex { get; set; }

		public int ColIndex { get; set; }

		public GridCell(int row, int col)
		{
			RowIndex = row;
			ColIndex = col;
		}
	}
}
