using System;
using System.Windows.Input;

namespace AntiKeyloggerUI.Auxiliary
{
    public sealed class ActionCommand : ICommand
    {
        private Action<object> executeGeneric;
        private Func<object, bool> canExecuteGeneric;

        private Action execute;
        private Func<bool> canExecute;

        public String Name {   get; private set; }

        public ActionCommand( Action<object> execute, Func<object, bool> canExecute = null, String name = null )
        {
            this.executeGeneric = execute;
            this.canExecuteGeneric = canExecute;
            Name = name ?? String.Empty;
        }

        public ActionCommand( Action execute, Func<bool> canExecute = null, String name = null )
        {
            this.execute = execute;
            this.canExecute = canExecute;
            Name = name ?? String.Empty;
        }

        public bool CanExecute( object parameter )
        {
            if (canExecute != null)
                return canExecute( );
            else
            {
                if (canExecuteGeneric != null)
                    return canExecuteGeneric(parameter);
            }

            return true;
        }

        public static bool CastParameterToType<TType>( object parameter, out TType value )
        {
            try
            { value = (TType)parameter; }
            catch
            {
                value = default(TType);
                return false;
            }

            return true;
        }

        public void Execute( object parameter )
        {
            if (execute != null)
                execute( );
            else
                executeGeneric(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}