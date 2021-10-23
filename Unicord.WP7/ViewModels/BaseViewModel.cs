using DiscordLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Unicord.WP7.Providers;

namespace Unicord.WP7.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IHasDiscordClient
    {
        protected SynchronizationContext syncContext;
        private DiscordClient discord;

        public BaseViewModel()
        {
            // capture the sync context used for this VM
            syncContext = SynchronizationContext.Current;
            discord = App.Current.Discord;
        }

        public DiscordClient Discord { get { return discord; } }

        public event PropertyChangedEventHandler PropertyChanged;

        // Holy hell is the C# Discord great.
        // Y'all should join https://aka.ms/csharp-discord
        protected void OnPropertySet<T>(ref T oldValue, T newValue, [CallerMemberName] string property = null)
        {
            if (oldValue == null || newValue == null || !newValue.Equals(oldValue))
            {
                oldValue = newValue;
                InvokePropertyChanged(property);
            }
        }

        public virtual void InvokePropertyChanged([CallerMemberName] string property = null)
        {
            if (PropertyChanged == null) return;

            var args = new PropertyChangedEventArgs(property);
            if (syncContext == SynchronizationContext.Current)
                PropertyChanged.Invoke(this, args);
            else
                syncContext.Post((o) => PropertyChanged.Invoke(this, (PropertyChangedEventArgs)o), args);
        }
    }
}
