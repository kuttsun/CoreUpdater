using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreUpdater
{
    public enum UpdateCompletedType
    {
        None,
        Success,
        Failure,
    }

    public interface IUpdateManager
    {
        Task<CoreUpdaterInfo> CheckForUpdatesAsync();// note: no async here
        Task<CoreUpdaterInfo> PrepareForUpdatesAsync(string outputPath);
        void Update(string[] args);
        void RestartApplication(string[] args, bool result = true);
        void StartUpdater(CoreUpdaterInfo coreUpdaterInfo);
        bool CanUpdate(string[] args);
        UpdateCompletedType Completed(string[] args);
    }
}
