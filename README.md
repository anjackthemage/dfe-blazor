# DopeFish Engine (Blazor)

dfe-blazor is a game engine written in C# and JavaScript, using Blazor to compile to WebAssembly.

[](https://raw.githubusercontent.com/anjackthemage/dfe-blazor/main/Assets/ss_1.png) [](https://raw.githubusercontent.com/anjackthemage/dfe-blazor/main/Assets/ss_2.png)

The virtual 3D game view is created using a custom raycasting engine that supports loading textures and sprites from PNG files.The raycaster is written in C# and individual video frames are sent to the HTML canvas using JavaScript Interop functionality provided by Blazor.

The client-side render loop is handled by the JavaScript [requestAnimationFrame](https://developer.mozilla.org/en-US/docs/Web/API/window/requestAnimationFrame) method. On the server side, we use C# async Tasks and the [Task.delay](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay?view=net-5.0) method to run a non-blocking game loop.

Multiplayer support and the client-server networking model use the SignalR protocol.

This project is no longer under active development, however the only major component missing from the current version is collision detection. At present, it could work as a sort of virtual chat room.