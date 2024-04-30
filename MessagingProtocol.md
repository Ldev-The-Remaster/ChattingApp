# Technical specifications for the League Sockets Messaging Protocol (LSMP)
This document lists the technical specifications and details for implementing LSMP in your websocket server.  
LSMP is a custom protocol intended for use on top of the websockets protocol for the purpose of a live chat messaging application.
# Client -> Server
Websocket messages coming from the client should conform to the following pattern:

```
DO <verb>
FROM <subject>
TO <object>
IN <location>
WITH
<message>
```

- Verb: The requested action
- Subject: The entity that requested the action
- Object: The entity on which the action was requested
- Location: The channel the request was performed in, and/or for alerts to be sent in
- Message: The text content of request or alert

## Details
- Request parameters must be seperated by new lines (CRLF: `\r\n`), with the parameter keyword (`DO`, `FROM`...etc) starting the line, followed by a space, then followed by the argument (the value associated with the parameter). And finally, the new line characters `\r\n`.
- Each request must contain and start with the `DO` parameter.
- Each parameter (`DO`, `FROM`...etc) should be in their own line with their respective argument (verb, subject...etc).
- The `WITH` parameter must be in a line of its own, it must also be the last parameter in the request, everything after this line until the end of the request (EOF) will be considered as the argument (the message body/content), including any subsequent new lines.
- Parameter keywords (`DO`, `FROM`...etc) and verbs are case**IN**sensitive (can be UPPERCASE or lowercase). However, it is advised to use UPPERCASE in order to standardize the format.
- Depending on the verb, certain parameters may be optional or completely omitted. For example, the `FROM` parameter does not need to provided in subsequent requests after `AUTH` as the server is expected to store the username for the current connection. The parameter should be ignored in such cases even if provided by the request. Optional parameters however can alter the functionality of certain actions.
- The first request from the client must be an `AUTH` verb with the username passed in the `FROM` parameter. If the first message is anything but `AUTH` the connection will be terminated immediatly. Additionally, the connection will also be terminated if no message was received from the client for 10 seconds after the initial connection. The server will not send any messages to a client while it is not authenticated.

## Verb list (WIP)
\* `DO` is required for all verbs.
- ### AUTH: Authenticate a user
   > Required params: `FROM <username>`
- ### CONNECT: Connect to a channel
   > Required params: `TO <channel-id>`
- ### SEND: Send a message in a channel
   > Required params: `IN <channel-id>`, `WITH <message>`
- ### ALERT: Send a server alert in a channel
   > Required params: `WITH <message>`  
   > Optional params: `IN <channel-id>`

   \*Alerts should probably be limited to admins  
   \*If the `IN` parameter is omitted, the alert is global
- ### MUTE: Mute a user in a channel
   > Required params: `TO <username>`, `IN <channel-id>`  
   > Optional params: `WITH <message>`
- ### KICK: Kick a user
   > Required params: `TO <user-id>`  
   > Optional params: `IN <channel-id>`, `WITH <message>`
- ### BAN: Ban a user
   > Required params: `TO <user-id>`  
   > Optional params: `IN <channel-id>`, `WITH <message>`
- ### IPBAN: Ban an IP
   > Required params: `TO <ip>`  
   > Optional params: `IN <channel-id>`, `WITH <message>`
- ### CREATE: Create a channel
   > Required params: `WITH <channel-name>`
- ### DELETE: Delete a channel
   > Required params: `TO <channel-id>`

\* The `IN` parameter for infractions (mutes, kicks, bans...etc) specifies which channel to send the alert in. Thus, the `IN` and `WITH` parameters must either be both present, or both omitted. EXCEPT for mutes; mutes require the `IN` parameter to specify which channel the user is to be muted in.

## Example messages

```
DO AUTH
FROM Akram
```
> Requests authentication from the server using the username `Akram`

---
```
DO SEND
IN 47532
WITH
yoo good morning
how you been?
```
> Request to send a message in channel with id `47532`, with the message content being:  
> `yoo good morning`  
> `how you been?`

---
```
DO ALERT
IN 133
WITH
Okkio connected!
```
> Request to send a server alert in channel with id `133` with the alert content being `Okkio connected!`

---
```
DO BAN
TO 94321
IN 23
WITH
Advertising is prohibited...
```
> Requests a ban on user with id `94321`, displaying the message `Advertising is prohibited...` in channel with id `23`

---
```
DO CREATE
WITH
general-chat
```
> Requests to create a channel with name `general-chat`

# Server -> Client
The server sends messages to the client in a similar way, with a few minor differences.

## Details
- The server does not use the `IN` parameter; this is because the server should only send the message to users connected to the concerned channel. E.g., the server should send an `ALERT` about channel `328` ONLY to users that are connected to channel `328`. So if the client receives a message, it is intended for the channel the user is currently connected to, making the `IN` parameter redundant.
- The same reasoning above applies to the `TO` parameter. If a user has an action performed on them like a ban or a mute, the server is only obligated to notify other users using an `ALERT` accompanied by a message explaining the details using the `WITH` parameter.
- The server should store the username given in the `AUTH` request for the current websocket connection. The username can then be used by the server to check for authority level, show message sender names, or to target infractions.

## Verbs
Keeping the details mentioned above in mind, besides performing backend operations, the server is obligated to notify the users using `SEND` for messages and `ALERT` for server alerts.

- ### SEND: Tells the client that a user has sent a message
   > Required params: `FROM <username>`, `WITH <message>`
- ### ALERT: Displays a server message 
   > Required params: `WITH <message>`

Besides displaying messages and alerts, the server also needs to respond to `AUTH` and `CONNECT` requests, this is done using the following verbs:
- ### ACCEPT: Accepts the request from the client
- ### REFUSE: Refuses the request from the client
   > Optional params: `WITH <reason>`

## Examples
User connects and attempts authentication using the username `Midfield`
```
DO AUTH
FROM Midfield
```

The server accepts the request
```
DO ACCEPT
```

---
`Midfield` tries to connect to a channel he does not have permissions for
```
DO CONNECT
TO 328
```

The server refuses the request
```
DO REFUSE
WITH
You are not allowed to access this channel
Please contact the administrator
```