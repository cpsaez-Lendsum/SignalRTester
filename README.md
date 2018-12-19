# SignalRTester

Very simple signalR Core tester console client.
How to launch

```
dotnet SignalRTester.dll --url http://yoursupersignalrhub/realtimehub
```

## send methodname

To send a message to the method specified in your endpoint. You can write the parameters one by one or just press enter to send the message without parameters.

## observe methodname

Observe the messages sent by the server. You have to specify the number of expected parameters due to signalR binding limitations

Example of session with a server endpoint who accept a Join method
```
 public class PitGameHub : Hub<IPitGameHub>
    { ...

        public async Task Join(string gameName, string nick)
        { ...
```

And a PlayerUpdate is sent to browsers 
```
   await context.Clients.Group(GameId).PlayerUpdate(GameId, players[nick]);
```

Start with

```
  dotnet SignalRTester.dll --url http://localhost:44355/pitgamehub

  Connected
  'send method' to send a message to the method
  'observe method' to observe a message from the method
  'exit now' to finish the application
  Write your command:

  >observe PlayerUpdate
  >2
  
  'send method' to send a message to the method
  'observe method' to observe a message from the method
  'exit now' to finish the application
  Write your command:
  
  >send Join
  >gameRoom1
  >cpsaez
  >(enter)

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



