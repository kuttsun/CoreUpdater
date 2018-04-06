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
        CoreUpdaterInfo CoreUpdaterInfo { get; set; }

        Task<CoreUpdaterInfo> CheckForUpdatesAsync();// note: no async here
        Task<CoreUpdaterInfo> PrepareForUpdatesAsync(string outputPath);
        Task<CoreUpdaterInfo> PrepareForUpdatesAsync(string outputPath, CoreUpdaterInfo coreUpdaterInfo);
        Task<CoreUpdaterInfo> PrepareForUpdatesAsync(string outputPath, string target);
        void Update(string[] args);
        void RestartApplication(string[] args, ExecutionType executionType, bool result = true);
        void StartUpdater(ExecutionType executionType);
        void StartUpdater(CoreUpdaterInfo coreUpdaterInfo, ExecutionType executionType);
        bool CanUpdate(string[] args);
        UpdateCompletedType Completed(string[] args);
    }
}
