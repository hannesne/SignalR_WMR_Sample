# SignalR Sample for Windows Mixed Reality
This is a sample SignalR server and client. 
The client is a Unity project that can be run on Hololens and immersive Windows Mixed Reality headsets. The client is built with Unity 2017.2.2f1.

The server and client currently uses the older ASP.Net 4.6 implementation for SignalR. The newer ASP.Net core SignalR server is incompatible with the older .Net Framework client libraries. It only supports the newer .Net Standard 2.0 client library. Unfortunately at this point Unity does not yet support .Net Standard 2.0 plugins. This support is planned for Unity 2018.1. When that becomes available I will provide a .Net Core sample, though it's very similar to what's already here.

To run the sample, open the web site in Visual Studio, and run it. This will start a local instance of your server. Now open the index.html page in your browser. You should be prompted for a nickname, type anything in the box. And send a message. Your message will be sent to the server, which will broadcast it out. The browser will receive this message and display it. If you can see the message, you server is working.

Next open the client app in Unity Editor. The sample works in the edit, so you don't need to deploy to a UWP app to test it. Look for the ChatClientBehaviour component on the text component in the editor. It has a field called SignalRServer where you can provide the url of your server. A default fallback is provided in the code, but this server is not guarenteed to be available. When you run the app, the text should change to indicate connection changes. When it is connected, you will see a message in the hololens stating that Hololens is connected. You will see the same message in the page you earlier opened in your browser.

Your next step should be to deploy the web server to Azure Web Apps. Do this by right clicking your project, and clicking on Publish. Follow the instructions to create a new web app. You can run the server on the free tier of an app service hosting plan. After deploying, open the index.html page at your deployed server location and test it. By default, Azure web apps do not have websockets enabled. To enable it, open the web app in the azure portal, and look for the websocket setting in the settings tab of the web app. 

This project uses 
* SignalR client and server
* NewtonSoft Json.Net
* JQuery
* Mixed Reality Toolkit

Sorry if I've missed your project, happy to take pull requests to add it.






