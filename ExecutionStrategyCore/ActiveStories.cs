using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ExecutionStrategyCore
{
    public class ActiveStories
    {
        private IEnumerable<string> activeStories;

        public ActiveStories(IEnumerable<string> activeStories)
        {
            this.activeStories = activeStories;
        }

        public bool Any()
        {
            return activeStories.Any(); 
        }

        public bool AnyMatching(IEnumerable<string> stories)
        {
            var joinedStrings = activeStories.Intersect(stories);
            return joinedStrings.Any();
        }
    }
}
