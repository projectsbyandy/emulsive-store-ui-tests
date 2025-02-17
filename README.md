# emulsive-store-ui-tests
UI e2e tests for Emulsive-ts-store website

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