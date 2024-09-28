Test Project: Multiplayer Game Lobby System

Objective: Your task is to build a multiplayer game lobby system using a REST API, which will allow players to join a lobby.

The system should handle the following requirements:

Requirements:

For simplicity lobby name/address could be static and singular (e.g. lobby_1)

Players must be able to join lobby via a REST API. lobby will have a maximum capacity of 64 players. Once the lobby reaches 64 players, user join requests should be rejected.

Player Notifications: When a player successfully joins a lobby, they must be notified with the message: "You have joined the lobby with N users", where N is the unique users in the lobby.

Clustered Environment: Your application will be running in a cluster environment with N containers. Each player may send a request to any container to join the lobby. The system must handle this and ensure all requests are properly managed across pods.

In-Memory System: Use Redis for managing the in-memory state of lobbies and players.

Technical Requirements: Language/Framework: We recommend using C# and .NET Core, but feel free to use any technology you are comfortable with that demonstrates your skills. Networking: Use REST APIs for player interaction. For bonus points, you may add WebSocket or TCP connections for real-time features.

Scalability: Demonstrate how the system can scale to handle multiple players and lobbies concurrently.

Bonus Points: Include Docker and Kubernetes configuration to show how you would deploy and scale the system.

Deliverables: The complete source code of the project (preferably on GitHub or similar). A brief README explaining the architecture, how to run the project, and any design decisions made. If applicable, any Docker and Kubernetes setup instructions.
