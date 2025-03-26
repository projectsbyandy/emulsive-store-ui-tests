# emulsive-store-ui-tests
C# UI e2e tests for Emulsive-ts-store website. For Typescript playwright tests go to the emulsive-ts-store project.

## Setup and start emulsive store
1. clone `git@github.com:projectsbyandy/emulsive-ts-store.git`
2. navigate to `src`
3. run `npm install`
4. start api and front end by running `npm run all`
- Note that by default, all data is mocked.

## Features
- Sequential and Parallel playwright tests
  - Stopwatch logs during tests provide an indication og test times (note unit does not report correctly paralleled tests run times)
  - Update the `enableDelayForParallelTest` flag in `appsettings.json` to illustrate time saved on longer user journeys.
- Parallel scenario level ReqNRoll tests
  - Using Aufofac for DI
  - `[assembly: Parallelizable(ParallelScope.Children)]` attribute at root
- Config support
- Retry resilience
- Dependency separation between business logic in core and playwright implementation

## Test Setup
Before running the tests the local configuration file needs to be setup.

1. Create a new file `appsettings.local.json` in the location `emulsive-store-ui-tests/src/EmulsiveStoreE2E.Tests/`. 
   - There should be other configuration files in the same directory
2. Copy the following settings into the json and save
   ```json
    {
      "environment": "local",
      "emulsiveStoreUrl": "http://localhost:5500"
    }
   ```
3. Save and ensure that the Copy to Output directory is set to Copy if newer.
4. The Nunit tests should now successfully run. For the BDD tests copy this file to the location `emulsive-store-ui-tests/src/EmulsiveStoreE2E.Tests.Bdd/`

## Trouble Shooting
### Unable to install the playwright browser due to unknown pwsh command
1. In an powershell cmd run `dotnet tool update --global PowerShell`
   - if there is a problem related to not being able to find the tool, it may require you to install a newer version of the dotnet sdk
2. Enter `pwsh` and the command should run successfully.