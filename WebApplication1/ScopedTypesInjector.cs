﻿using ExecutionStrategyCore;
using Microsoft.Extensions.Configuration;
using WebApplication1.Data;
using System.Linq;
using WebApplication1.Data.Queries;

namespace WebApplication1
{
    //TODO: consider appsettings.json picking between an injector that checks for active-stories section and one that does not.  
    public class ScopedTypesInjector : IServicesConfig
    {
        private readonly IServiceCollectionWrapper serviceCollectionWrapper;
        private readonly IConfiguration configuration;

        public ScopedTypesInjector(IServiceCollectionWrapper scopedServicesWrapper, IConfiguration configuration)
        {
            this.serviceCollectionWrapper = scopedServicesWrapper;
            this.configuration = configuration;
        }

        public void ConfigureServices()
        {
            serviceCollectionWrapper.AddConfig<ServicesConfig>();
            serviceCollectionWrapper.AddScoped<BlogDbSetRunner>();
            serviceCollectionWrapper.AddScoped<IRunner<BloggingContext>, BloggingContextRunner>();

            var activeStoriesSection = configuration.GetSection("active-stories");
            if (activeStoriesSection.Exists())
            {
                var storyFlags = activeStoriesSection.GetChildren().Select(x => x.Value);
                var activeStories = new ActiveStories(storyFlags);
                var activeStoryFactory = new ActiveStoryFactory(activeStories);
                var storyRunner = new StoryOverrideRunner (new ExecutionStrategyRunner(), activeStoryFactory);
                serviceCollectionWrapper.AddSingleton<ITaskRunner>(storyRunner);
            }
            else
            {
                serviceCollectionWrapper.AddScoped<ITaskRunner, ExecutionStrategyRunner>();
            }
        }
    }
}
