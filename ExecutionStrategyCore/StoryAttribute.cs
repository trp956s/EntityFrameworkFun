using System;

namespace ExecutionStrategyCore
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StoryAttribute : Attribute
    {
        public StoryAttribute(string storyFlag)
        {
            StoryFlag = storyFlag;
        }

        public string StoryFlag { get; }
    }
}
