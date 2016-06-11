﻿using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.AutoRest
{
    /// <summary>
    /// Wrapper around Azure AutoRest
    /// </summary>
    public class AutoRestRunner : Cake.Core.Tooling.Tool<AutoRestSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoRestRunner"/> class
        /// </summary>
        /// <param name="fileSystem">The file system</param>
        /// <param name="environment">The environment</param>
        /// <param name="processRunner">Process runner</param>
        /// <param name="tools">Registered tools</param>
        /// <param name="log">Log</param>
        public AutoRestRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools, ICakeLog log) : base(fileSystem, environment, processRunner, tools)
        {
            Environment = environment;
            Log = log;

        }

        private ICakeLog Log { get; set; }

        private ICakeEnvironment Environment { get; set; }

        /// <summary>
        /// Gets the tool name
        /// </summary>
        /// <returns>Tool name</returns>
        protected override string GetToolName() => "Azure AutoRest";

        /// <summary>
        /// Gets the possible executable names for the tool
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            yield return "AutoRest.exe";
            yield return "AutoRest";
        }

        /// <summary>
        /// Invokes AutoRest to create a client model for the provided API spec, optionally using the provided settings.
        /// </summary>
        /// <param name="inputFile">Local API specification file (e.g. swagger definition)</param>
        /// <param name="configure">Optional action to configure runner settings</param>
        /// <returns>The directory where the client was generated</returns>
        public DirectoryPath Generate(FilePath inputFile, Action<AutoRestSettings> configure = null)
        {
            var settings = new AutoRestSettings(inputFile);
            configure?.Invoke(settings);
            return Generate(inputFile, settings);
        }


        /// <summary>
        /// Invokes AutoRest to create a client model for the provided API spec, using the provided settings
        /// </summary>
        /// <param name="inputFile">Local API specification file (e.g. swagger definition)</param>
        /// <param name="settings">Settings to use when invoking AutoRest</param>
        /// <returns>The directory where the client was generated</returns>
        public DirectoryPath Generate(FilePath inputFile, AutoRestSettings settings)
        {
            settings.InputFile = settings.InputFile ?? inputFile;
            var args = GetToolArguments(settings);
            Log.Verbose(args.Render());
            Run(settings, args);
            return settings.OutputDirectory ?? "./Generated";
        }

        private ProcessArgumentBuilder GetToolArguments(AutoRestSettings settings)
        {
            var args = new ProcessArgumentBuilder();
            settings.Build(args);
            return args;
        }
    }
}
