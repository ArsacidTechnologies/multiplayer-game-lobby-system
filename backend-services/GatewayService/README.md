Base URL: `/api/`

Base URL: `https://localhost:7000/api/`

| HTTP Method | Endpoint                                  | Description                                    |
| ----------- | ----------------------------------------- | ---------------------------------------------- |
| POST        | /api/player                               | Creates a new player.                          |
| GET         | /api/player                               | Retrieves the list of all players.             |
| POST        | /api/lobby                                | Creates a new lobby.                           |
| POST        | /api/lobby/{lobbyId}/join?playerId={id}   | Adds a player to the specified lobby.          |
| POST        | /api/lobby/{lobbyId}/remove?playerId={id} | Removes a player from the specified lobby.     |
| GET         | /api/lobby/{lobbyId}                      | Retrieves the details of the specified lobby.  |
| GET         | /api/lobby                                | Retrieves the list of all lobbies.             |
| POST        | /api/notification/send?message={message}  | Sends a notification to all connected clients. |
