# Game Action Handler Service



### **Actions that don't allow packet loss (reliable actions)** :

* These are **critical game actions** that **must** be delivered and processed by the server to maintain the integrity of the game state. For example, actions like:
  * Player movements
  * Attacks or firing a weapon
  * Game state updates like score changes or item pickups
  * Server-side events like level changes or game over triggers
* **RUDP** is suitable for these actions because it guarantees:
  * **Reliable delivery** : Packets are resent if lost.
  * **Ordered delivery** : Packets arrive in the same order they were sent.
  * **Acknowledgment** : The sender knows when a packet has been received successfully.


## Directory Structure

```
multiplayer-game-lobby-system/
│
└─── backend-services/
     ├─── PlayerService/
     │    ├─── PlayerService.csproj
     │    ├─── Controllers/
     │    ├─── Services/
     │    ├─── Repositories/
     │    └─── Models/
     │
     ├─── LobbyService/
     │    ├─── LobbyService.csproj
     │    ├─── Controllers/
     │    ├─── Services/
     │    ├─── Repositories/
     │    └─── Models/
     │
     ├─── NotificationService/
     │    ├─── NotificationService.csproj
     │    ├─── Controllers/
     │    ├─── SignalR/
     │    └─── Events/
     │
     ├─── ApiGateway/
     │    ├─── ocelot.json
     │    ├─── ApiGateway.csproj
     │
     ├─── GameActionHandler/
     │    ├─── GameActionHandler.csproj
     │    ├─── Controllers/
     │    ├─── Services/
     │    ├─── RUDP/
     │    ├─── ReliableActions/      # For actions requiring reliable delivery
     │    ├─── UnreliableActions/    # For actions where packet loss is acceptable
     │    └─── Models/
     │
     ├─── SharedUtils/
     │    ├─── Events/
     │    ├─── RedisKeyHelper.cs
     │
     └─── Redis (for in-memory state)


```



## EndPoints


| Command Type   | Description                                                                                                                                                                                                              |
| -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| action_execute | A command sent from the client to the server to execute a game action.<br />The payload contains the player's action data (e.g., movement, attack).<br />This is handled by RUDP, so no need for manual acknowledgment. |
| state_update   | Sent from the server to update the client about the<br />current state of the game (e.g., player positions, scores).                                                                                                   |
| reliable_send  | A command used to send critical game actions that<br />require guaranteed delivery. This ensures the packet will<br />be retransmitted until acknowledged.                                                              |
| connect        | A command sent by the client to establish a RUDP connection<br />with the server for real-time game action interactions.                                                                                               |
| disconnect     | A command sent by the client to inform the server<br />that the player is disconnecting from the game.                                                                                                                  |
