using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Challenger.Models;
using Challenger.Models.Extensions;

namespace Challenger.ViewModels
{
    public class ChallengerVm : INotifyPropertyChanged
    {
        #region Properties

        private DataTable _challengerTable;
        public DataTable ChallengerTable
        {
            get { return _challengerTable; }
            set
            {
                _challengerTable = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _findAnswer;
        public ICommand FindAnswer
        {
            get { return _findAnswer; }
            set
            {
                _findAnswer = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public ChallengerVm()
        {
            FindAnswer = new DelegateCommand(FindAnswerToChallenger);

            ChallengerTable = new DataTable();
            
            for (int i = 0; i < 5; i++)
            {
                ChallengerTable.Columns.Add();
            }

            for (int i = 0; i < 6; i++)
            {
                var row = ChallengerTable.NewRow();

                for (int j = 0; j < 5; j++)
                {
                    row[j] = 0;
                }

                ChallengerTable.Rows.Add(row);
            }
        }

        #endregion

        public void FindAnswerToChallenger()
        {
            var challengerArray = new ChallengerArray(ChallengerTable);

            var solution = challengerArray.GetSolution();

            if (solution == null)
            {
                MessageBox.Show("No solution.");
            }
            else
            {
                ChallengerTable = solution;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
