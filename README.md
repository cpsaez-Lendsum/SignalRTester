# SignalRTester

Very simple signalR Core tester console client.
How to launch

```
dotnet SignalRTester.dll --url http://yoursupersignalrhub/realtimehub
```

send methodname

To send a message to the method specified in your endpoint. You can write the parameters one by one or just press enter to send the message without parameters.

observe methodname

Observe the messages sent by the server. You have to specify the number of expected parameters due to signalR binding limitations

Example

```
  dotnet SignalRTester.dll --url http://localhost:44355/pitgamehub

  Connected
  'send method' to send a message to the method
  'observe method' to observe a message from the method
  'exit now' to finish the application
  Write your command:

  observe PlayerUpdate
  2
  send Join
  gameRoom1
  cpsaez
   (enter)

  Message Received: PlayerUpdate
  Parameter ----------
  gameRoom1
  --------- ----------
  Parameter ----------
  {
    "something": null,
    "nick": "cpsaez",
    "ready": false
  }
  --------- ----------
```



