using System.Data;
using System.Linq;

namespace Challenger.Models
{
    public class ChallengerArray
    {
        private const int MAX_SQUARE_VALUE = 9;
        private const int MIN_SQUARE_VALUE = 1;

        private readonly ChallengerSquare[][] _array;
        private readonly int _rowCount;
        private readonly int _columnCount;

        public ChallengerArray(DataTable challengerTable)
        {
            _array = GetArrayFromDataTable(challengerTable);
            _rowCount = _array.Count();
            _columnCount = _array[0].Count();
        }

        public DataTable GetSolution()
        {
            var rowCount = _array.GetLength(0);

            while (!AnswerIsCorrect())
            {
                for (int i = 1; i < rowCount - 1; i++)
                {
                    ChallengerSquare[] currRow = _array[i];

                    var currRowNextSolution = GetNextValidRow(currRow);

                    if (currRowNextSolution == null)
                    {
                        if (i < rowCount - 1)
                        {
                            var initializedRow = GetInitializedRow(_array[i]);
                            var initialValidRow = GetNextValidRow(_array[i]);
                            _array[i] = initialValidRow;
                            continue;
                        }
                        else
                        {
                            return null; // No solution
                        }
                    }

                    if (currRowNextSolution != null)
                    {
                        _array[i] = currRowNextSolution;
                        break;
                    }

                    break;
                }
            }

            return GetDataTableFromArray(_array);
        }

        private ChallengerSquare[][] GetArrayFromDataTable(DataTable dataTable)
        {
            var rowCount = dataTable.Rows.Count;
            var columnCount = dataTable.Columns.Count;

            var challengerArray = new ChallengerSquare[rowCount][];

            for (int row = 0; row < rowCount; row++)
            {
                challengerArray[row] = new ChallengerSquare[columnCount];

                for (int col = 0; col < columnCount; col++)
                {
                    var currSquare = new ChallengerSquare
                    {
                        Number = int.Parse(dataTable.Rows[row][col].ToString())
                    };

                    if (row == rowCount - 1 || col == columnCount - 1)
                    {
                        currSquare.IsSum = true;
                    }

                    if (currSquare.Number != 0 ||
                        (row == 0 && col < columnCount - 1))
                    {
                        currSquare.IsFixed = true;
                    }
                    else
                    {
                        currSquare.Number = MAX_SQUARE_VALUE;
                    }

                    challengerArray[row][col] = currSquare;
                }
            }

            return challengerArray;
        }

        private DataTable GetDataTableFromArray(ChallengerSquare[][] array)
        {
            var rowCount = array.Count();
            var columnCount = array[0].Count();

            var dataTable = new DataTable();

            for (int i = 0; i < columnCount; i++)
            {
                dataTable.Columns.Add();
            }

            for (int i = 0; i < rowCount; i++)
            {
                var row = dataTable.NewRow();

                for (int j = 0; j < columnCount; j++)
                {
                    row[j] = array[i][j].Number;
                }

                dataTable.Rows.Add(row);
            }
    
            return dataTable;
        }

        private ChallengerSquare[] GetInitializedRow(ChallengerSquare[] squares)
        {
            foreach (var square in squares)
            {
                if (square.IsFixed || square.IsSum)
                {
                    continue;
                }

                square.Number = MAX_SQUARE_VALUE;
            }

            return squares;
        } 

        private ChallengerSquare[] GetNextValidRow(ChallengerSquare[] squares)
        {
            var additives = squares.Take(squares.Count() - 1).ToArray();
            var sum = squares[squares.Count() - 1];
            var additivesLastIndex = additives.Count() - 1;

            do
            {
                if (additives.Where(x => !x.IsFixed).Sum(x => (x.Number - MIN_SQUARE_VALUE)) == 0)
                {
                    return null; // Next solution does not exist
                }

                for (int i = 0; i < additives.Count(); i++)
                {
                    if (additives[i].IsFixed)
                    {
                        continue;
                    }

                    if (additives[i].Number <= MIN_SQUARE_VALUE && i < additivesLastIndex)
                    {
                        additives[i].Number = MAX_SQUARE_VALUE;
                        continue;
                    }

                    if (additives[i].Number > MIN_SQUARE_VALUE)
                    {
                        additives[i].Number -= 1;
                        break;
                    }

                    break;
                }
            }
            while (sum.Number != additives.Sum(x => x.Number));

            return additives.Concat(new ChallengerSquare[] { sum }).ToArray();
        }

        private bool AnswerIsCorrect()
        {
            if (!RowSumsAreValid())
            {
                return false;
            }

            if (!ColSumsAreValid())
            {
                return false;
            }

            if (!DiagonalSumsAreValid())
            {
                return false;
            }

            return true;
        }

        #region Summation Helpers

        private bool RowSumsAreValid()
        {
            for (int row = 1; row < _rowCount - 1; row++)
            {
                var expectedRowSum = _array[row][_columnCount - 1].Number;
                var actualRowSum = 0;

                for (int col = 0; col < _columnCount - 1; col++)
                {
                    var currentValue = _array[row][col].Number;
                    actualRowSum += currentValue;
                }

                if (expectedRowSum != actualRowSum)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ColSumsAreValid()
        {
            for (int col = 0; col < _columnCount - 1; col++)
            {
                var expectedColSum = _array[_rowCount - 1][col].Number;
                var actualColSum = 0;

                for (int row = 1; row < _rowCount - 1; row++)
                {
                    var currentValue = _array[row][col].Number;
                    actualColSum += currentValue;
                }

                if (expectedColSum != actualColSum)
                {
                    return false;
                }
            }

            return true;
        }

        private bool DiagonalSumsAreValid()
        {
            var expectedBottomRightDiagonalSum = _array[_rowCount - 1][_columnCount - 1].Number;
            var actualBottomRightDiagonalSum = 0;

            for (int row = 1; row < _rowCount - 1; row++)
            {
                for (int col = 0; col < _columnCount - 1; col++)
                {
                    if ((row - 1) == col)
                    {
                        var currentValue = _array[row][col].Number;
                        actualBottomRightDiagonalSum += currentValue;
                    }
                }
            }

            if (expectedBottomRightDiagonalSum != actualBottomRightDiagonalSum)
            {
                return false;
            }

            var expectedTopRightDiagonalSum = _array[0][_columnCount - 1].Number;
            var actualTopRightDiagonalSum = 0;

            for (int row = _rowCount - 2; row > 0; row--)
            {
                for (int col = 0; col < _columnCount - 1; col++)
                {
                    if ((_rowCount - 2) - row == col)
                    {
                        var currentValue = _array[row][col].Number;
                        actualTopRightDiagonalSum += currentValue;
                    }
                }
            }

            if (expectedTopRightDiagonalSum != actualTopRightDiagonalSum)
            {
                return false;
            }

            return true;
        }

        #endregion

    }
}
