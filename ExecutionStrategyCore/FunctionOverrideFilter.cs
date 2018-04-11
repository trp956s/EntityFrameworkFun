﻿using System;
using System.Linq;
using System.Reflection;

namespace ExecutionStrategyCore
{
    internal class FunctionOverrideFilter<T> : IRunner<Func<T>>
    {
        private Func<T>[] overrides;
        private readonly IRunner<ActiveStories> storiesGetter;

        public FunctionOverrideFilter(Func<T>[] overrides, IRunner<ActiveStories> storiesGetter)
        {
            this.overrides = overrides;
            this.storiesGetter = storiesGetter;
        }

        public Func<T> Run()
        {
            var stories = storiesGetter.Run();
            if(!stories.Any())
            {
                return null;
            }

            return overrides.Reverse().FirstOrDefault(func => 
                IsFuncAnActiveStoryOverride<T>(func.Target, stories)
                || IsFuncAnActiveStoryOverride(func.Method, stories)
            );
        }

        private bool IsFuncAnActiveStoryOverride<T>(object target, ActiveStories stories)
        {
            if (target is StoryFunctionRunner<T>)
            {
                var storyFunctionRunner = (StoryFunctionRunner<T>)target;
                return stories.AnyMatching(storyFunctionRunner.StoryNumbers);
            }

            return false;
        }

            private bool IsFuncAnActiveStoryOverride(MethodInfo methodInfo, ActiveStories stories)
        {
            var storyAttributes = methodInfo.GetCustomAttributes(typeof(StoryAttribute), true);
            var storyAttributeValues = storyAttributes.OfType<StoryAttribute>().Select(
                    x => x.StoryFlag
            );

            return stories.AnyMatching(storyAttributeValues);
        }
    }
}