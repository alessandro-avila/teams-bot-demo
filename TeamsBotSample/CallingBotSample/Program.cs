// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;
using Orleans;

namespace CallingBotSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder.UseLocalhostClustering()
                               .AddMemoryStreams("StreamProvider")
                               .AddMemoryGrainStorage("PubSubStore")
                               .AddMemoryGrainStorage("messages");
                    siloBuilder.UseInMemoryReminderService();
                    siloBuilder.UseDashboard(x => x.HostSelf = true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
