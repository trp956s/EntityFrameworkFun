using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutionStrategyCore
{
    public class ActiveStories
    {
        private IEnumerable<string> activeStories;

        public ActiveStories(IEnumerable<string> activeStories)
        {
            this.activeStories = activeStories;
        }
    }
}
