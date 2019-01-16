using System;
using System.Windows.Input;

namespace CompanyEmployees.ViewModel
{
    internal class ViewModelCommand : ICommand
    {
        private readonly Action<object> Action;

        private readonly Func<object, bool> IsActionExecutable;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ViewModelCommand(Action<object> action, Func<object, bool> isActionExecutable = null)
        {
            Action = action;
            IsActionExecutable = isActionExecutable;
        }

        public bool CanExecute(object parameter)
        {
            return IsActionExecutable == null || IsActionExecutable(parameter);
        }

        public void Execute(object parameter)
        {
            Action(parameter);
        }
    }
}
