using Slipka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Repositories
{
    public class GridFsCleanupLoop
    {
        public GridFsCleanupLoop(FileRepository fileRepository, ProxySettings settings)
        {
            FileRepository = fileRepository;

            System.Timers.Timer asyncUpdater = new System.Timers.Timer();
            asyncUpdater.Elapsed += AsyncUpdater_Elapsed;
            asyncUpdater.Interval = settings.GridFsCleanupLoop;
            asyncUpdater.Enabled = true;

        }

        private FileRepository FileRepository { get; }

        private void AsyncUpdater_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FileRepository.CleanBucket();
        }
    }
}
