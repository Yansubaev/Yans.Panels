# Yans Panels

A Unity package for managing UI screens, including panels and popups, with support for transitions, orientation changes, and ViewModel integration.

## Overview

Yans Panels provides a robust system for handling the lifecycle and navigation of UI screens in Unity projects. It is built with flexibility and extensibility in mind, allowing developers to easily create and manage complex UI flows.

## Core Components

### Screens
- **`UIScreen`**: The abstract base class for all UI screens. It manages the lifecycle (Create, Start, Resume, Pause, Stop, Destroy), provides access to `Canvas`, `GraphicRaycaster`, and `RectTransform` for transitions, and integrates with `IViewModelProvider`.
- **`UIPanel`**: A subclass of `UIScreen` specifically for panel-type UIs. It includes functionality to adjust its layout based on the screen's safe area, configurable for different screen sides (`ScreenSide` enum).
- **`UIPopup`**: A subclass of `UIScreen` intended for popup dialogs or notifications.

### Management
- **`IScreenManager`**: Defines the contract for managing screens. Key methods include `OpenScreen<T>`, `CloseScreen`, `CloseTop`, `CloseAll`, and `GetScreen<T>`.
- **`UIScreenManager`**: The default implementation of `IScreenManager`. It handles the screen stack, screen instantiation via `IScreenInstantiator`, transitions using `ITransitionResolver`, and ViewModel provision through `IViewModelProvider`. It also listens to orientation changes via `IOrientationChangeListener`.
- **`UIRoot`**: A `MonoBehaviour` that acts as the root for all UI screens. It holds the main `Canvas`, `CanvasScaler`, `GraphicRaycaster`, and the container `RectTransform` for screens. It also manages `IOrientationChangeListener`s.

### Instantiation
- **`IScreenInstantiator`**: Interface for screen instantiation logic. Defines methods to instantiate screens by type or generic type, considering screen orientation, and a method to clean up screens.
- **`ScreenInstantiator`**: An abstract `MonoBehaviour` implementing `IScreenInstantiator`. Provides a fallback mechanism for screen orientations.
- **`PrefabScreenInstantiator`**: A concrete implementation of `ScreenInstantiator` that instantiates screens from a list of prefabs (`OrientedScreenReference`), allowing different prefabs for different screen orientations.

### Transitions
- **`ITransition`**: Interface for defining screen transitions. The `Play` method takes the `from` and `to` `UIPanel`s.
- **`ITransitionResolver`**: Interface to resolve which `ITransition` to use between two screen types.
- **`TransitionResolver`**: Default implementation of `ITransitionResolver`. It allows registering specific transitions for pairs of screen types and provides a default transition ( `EmptyTransition` if none is specified).
- **`EmptyTransition`**: An implementation of `ITransition` that performs no visual transition.

### ViewModels
- **`IViewModelOwner`**: Interface implemented by classes that can own ViewModels (e.g., `UIScreen`). Requires `GetInstanceId()`.
- **`IViewModelProvider`**: Interface for providing `ViewModel` instances to `IViewModelOwner`s. Methods include `Get<VM>` and `Clear`.
- **`ViewModelProvider`**: Default implementation of `IViewModelProvider`. It manages `ViewModel` instances based on the `IViewModelOwner`'s instance ID, creating new ViewModels as needed and caching them.
- **`ViewModel`**: Abstract base class for ViewModels. Includes `OnCreated` and `OnAborted` lifecycle methods.

### Orientation
- **`IOrientationChangeListener`**: Interface for objects that need to react to screen orientation changes. Defines `OnOrientationChanged`.

## Features

- **Screen Lifecycle Management**: Comprehensive lifecycle control for `UIScreen` objects.
- **Panel and Popup Specializations**: Dedicated base classes (`UIPanel`, `UIPopup`) for common UI paradigms.
- **Stack-Based Navigation**: `UIScreenManager` manages a stack of screens for intuitive navigation.
- **Asynchronous Operations**: Leverages `UniTask` for asynchronous screen opening and closing.
- **Prefab-Based Instantiation**: `PrefabScreenInstantiator` allows easy instantiation of screens from prefabs, with support for orientation-specific prefabs.
- **Customizable Transitions**: Define and resolve custom screen transitions using `ITransition` and `ITransitionResolver`.
- **ViewModel Integration**: Built-in support for associating `ViewModel`s with screens via `IViewModelProvider`.
- **Safe Area Handling**: `UIPanel` can automatically adjust its padding to respect the device's safe area.
- **Orientation Change Handling**: System for notifying components about screen orientation changes.

## Installation

To use Yans Panels in your Unity project:

1.  **Add Dependencies**: This package depends on `com.cysharp.unitask`. Ensure it's included in your project's `Packages/manifest.json`:
    ```json
    {
      "dependencies": {
        "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
      }
    }
    ```
2.  **Add Yans Panels**: Add the Yans Panels package to your `manifest.json` (replace `[GIT_URL_TO_YANSPANELS_REPO_HERE]` with the actual Git URL when available, or use a local path if you have the package locally):
    ```json
    {
      "dependencies": {
        "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
        "com.yans.panels": "https://github.com/Yansubaev/Yans.Panels.git" 
      }
    }
    ```
    Or, use the Unity Package Manager: `Window > Package Manager > + > Add package from git URL...`

## Basic Usage

1.  **Create a `UIRoot`**: Add the `UIRoot` component to an object in your scene (e.g., a dedicated UI GameObject).
2.  **Create Screen Prefabs**: Design your UI screens (panels, popups) and save them as prefabs. Ensure they have a component inheriting from `UIPanel` or `UIPopup`.
3.  **Configure `PrefabScreenInstantiator`**: Create an instance of `PrefabScreenInstantiator` (or your custom instantiator) and link your screen prefabs to it, specifying orientations if needed.
4.  **Set up `TransitionResolver`**: Create a `TransitionResolver` and register any custom transitions.
5.  **Initialize `UIScreenManager`**: Instantiate `UIScreenManager` with the `UIRoot`, your screen instantiator, transition resolver, and optionally a `ViewModelProvider`.
    ```csharp
    UIRoot uiRoot = FindObjectOfType<UIRoot>(); // Or however you access your UIRoot
    IScreenInstantiator screenInstantiator = FindObjectOfType<PrefabScreenInstantiator>(); // Configure with your prefabs
    ITransitionResolver transitionResolver = new TransitionResolver();
    IViewModelProvider viewModelProvider = new ViewModelProvider();

    IScreenManager screenManager = new UIScreenManager(uiRoot, screenInstantiator, transitionResolver, viewModelProvider);
    ```
6.  **Open and Close Screens**:
    ```csharp
    // Open a screen
    MyPanelType panel = await screenManager.OpenScreen<MyPanelType>();

    // Close a screen
    await screenManager.CloseScreen(panel);

    // Close the top-most screen
    await screenManager.CloseTop();
    ```

## ViewModels

- Create custom ViewModel classes by inheriting from `ViewModel`.
- Access ViewModels within your `UIScreen` implementations:
  ```csharp
  public class MyScreenWithViewModel : UIScreen
  {
      private MyViewModel _myViewModel;

      protected override void OnStarted()
      {
          base.OnStarted();
          _myViewModel = ViewModelProvider.Get<MyViewModel>(this);
          // Use _myViewModel
      }
  }
  ```

## Custom Transitions

1.  Implement the `ITransition` interface:
    ```csharp
    public class MyCustomTransition : ITransition
    {
        public async UniTask Play(UIPanel from, UIPanel to)
        {
            // Your transition animation logic here
            if (from != null) from.gameObject.SetActive(false);
            if (to != null) to.gameObject.SetActive(true);
            await UniTask.DelayFrame(1); // Example placeholder
        }
    }
    ```
2.  Register your transition with the `TransitionResolver`:
    ```csharp
    transitionResolver.RegisterTransition(typeof(MyFirstPanel), typeof(MySecondPanel), new MyCustomTransition());
    ```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on the [GitHub repository](https://github.com/Yansubaev/YansPanels).

## License

This package is licensed under the MIT License. See the [LICENSE](Runtime/LICENSE) file for details.
