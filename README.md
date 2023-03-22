
# Networking Practical Midterm

For Ontario Tech's Distributed Systems and Networking (INFR3830)

Abdalla Mohamed (100795120)  
Nathan Woo (100787454)

## How to Run

- Run "Client1.exe" in client. You may run multiple instances of the client application.
- Run "Server.exe"
You need to run Server.exe before typing in the IP.   
The server can hold multiple clients, but the clients are only coded to contain two clients at the moment. (Hardcoded at two)


## Connecting to the Server
The server will display the server IP you can use to connect to it on it's console.
Input the Server IP and press "connect" the client in order to connect to the server.

The server is currently hardcoded to **127.0.0.1** but can be changed in the source code.


## Interaction
- You will be able to move with "WASD" and the arrow keys 
- The server will receive the positions fro and send them to the client
- The client will only send positional information to the server when the position has changed.
- In order to send chat messages, make sure you type in the IP first and then type your message in the bottom left input box UI. Press Enter in order to send the message.
- Disconnect from the server by typing "/quit" in the chat box or quitting the application.
