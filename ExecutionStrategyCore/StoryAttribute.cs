using System;
using System.Collections.Generic;
using System.Text;

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
