# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.6] - 2025-06-21
### Added
- Async `WaitForStartAsync`, `WaitForResumeAsync`, `WaitForPauseAsync`, `WaitForStopAsync`, and `WaitForCloseAsync` methods in `UIScreen` using `UniTaskCompletionSource` to await lifecycle events without modifying existing signatures.

### Changed
- `UIScreenManager.OpenScreen` and `CloseScreen` no longer await transition animations; transitions are now fired asynchronously (`.Forget()`) to avoid blocking the screen management API.

## [1.0.5] - 2025-06-16
### Added
- `AddScreenResultListener`, `RemoveScreenResultListener`, `ClearScreenResultListeners`, and `SendResultToListeners` methods to `UIScreen`.

## [1.0.4] - 2025-06-14
### Added
- `ScreenResult` base class for screen results.
- `IScreenResultListener` interface for screens to send results to their callers.
- Functionality in `UIScreen` to manage result listeners and send results.

## [1.0.3] - 2025-06-14
### Changed
- Made `ViewModelProvider.CreateViewModelInstance<V>()` method virtual to allow for easier extension.

## [1.0.2] - 2025-06-14
### Fixed
- Package.json version.

## [1.0.1] - 2025-06-14
### Fixed
- Git commit history.

## [1.0.0] - 2025-06-13
### Added
- Initial project setup for Yans.Panels.
