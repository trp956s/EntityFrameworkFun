namespace ExecutionStrategyCore
{
    public class ActiveStoryFactory : IRunner<ActiveStories>
    {
        private readonly ActiveStories activeStories;

        public ActiveStoryFactory(ActiveStories stories) {
            this.activeStories = stories;
        }

        public ActiveStories Run()
        {
            return activeStories;
        }
    }
}
