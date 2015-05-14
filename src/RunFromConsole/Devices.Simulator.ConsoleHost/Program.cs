﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.IoTJourney.Devices.Simulator.Instrumentation;
using Microsoft.Practices.IoTJourney.Logging;

namespace Microsoft.Practices.IoTJourney.Devices.Simulator.ConsoleHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var observableEventListener = new ObservableEventListener();

            observableEventListener.EnableEvents(
              ScenarioSimulatorEventSource.Log, EventLevel.Informational);

            observableEventListener.LogToConsole();

            var configuration = SimulatorConfiguration.GetCurrentConfiguration();

            var instrumentationPublisher =
                new SenderInstrumentationManager(instrumentationEnabled: true, installInstrumentation: true)
                    .CreatePublisher("Console");

            var deviceSimulator = new SimulationProfile("Console", 1, instrumentationPublisher, configuration);

            var options = SimulationScenarios
                .AllScenarios
                .ToDictionary(
                    scenario => "Run " + scenario,
                    scenario => (Func<CancellationToken, Task>)(token => deviceSimulator.RunSimulationAsync(scenario, token)));

            Tests.Common.ConsoleHost.WithOptions(options, configuration.ScenarioDuration);
        }
    }
}
