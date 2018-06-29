using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Repositories
{
    public class GridFsCleanupLoop
    {
        public GridFsCleanupLoop(FileRepository fileRepository)
        {
            FileRepository = fileRepository;

            System.Timers.Timer asyncUpdater = new System.Timers.Timer();
            asyncUpdater.Elapsed += AsyncUpdater_Elapsed;
            asyncUpdater.Interval = 5000;
            asyncUpdater.Enabled = true;

        }

        private FileRepository FileRepository { get; }

        private void AsyncUpdater_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FileRepository.CleanBucket();
        }
    }
}
