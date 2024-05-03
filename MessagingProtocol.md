# Technical specifications for the League Sockets Messaging Protocol (LSMP)
This document lists the technical specifications and details for implementing LSMP in your websocket server.  
LSMP is a custom protocol intended for use on top of the websockets protocol for the purpose of a live chat messaging application.  
*Version: 0.2.0*

# Client -> Server
Websocket messages coming from the client should conform to the following pattern:

```
DO <verb>
FROM <subject>
TO <object>
IN <location>
AT <date>
WITH
<payload>
```

- Verb: The requested action
- Subject: The entity that requested the action
- Object: The entity on which the action was requested
- Location: The channel the request was performed in, and/or for alerts to be sent in
- Date: The date-time stamp associated with the message (in Unix time)
- Payload: The text content of request or alert

## Details
- Request parameters must be seperated by new lines (CRLF: `\r\n`), with the parameter keyword (`DO`, `FROM`...etc) starting the line, followed by a space, then followed by the argument (the value associated with the parameter). And finally, the new line characters `\r\n`.
- Each request must contain and start with the `DO` parameter.
- Each parameter (`DO`, `FROM`...etc) should be in their own line with their respective argument (verb, subject...etc).
- The `WITH` parameter must be in a line of its own, it must also be the last parameter in the request, everything after this line until the end of the request (EOF) will be considered as the argument (the payload, A.K.A message body/content), including any subsequent new lines.
- Parameter keywords (`DO`, `FROM`...etc) and verbs are case**IN**sensitive (can be UPPERCASE or lowercase). However, it is advised to use UPPERCASE in order to standardize the format.
- Depending on the verb, certain parameters may be optional or completely omitted. For example, the `FROM` parameter does not need to provided in subsequent requests after `AUTH` as the server is expected to store the username for the current connection. The parameter should be ignored in such cases even if provided by the request. Optional parameters however can alter the functionality of certain actions.
- If the `AT` parameter is omitted, the timestamp is set to the exact time the request was received.
- The first request from the client must be an `AUTH` verb with the username passed in the `FROM` parameter. If the first message is anything but `AUTH` the connection will be terminated immediatly. Additionally, the connection will also be terminated if no message was received from the client for 10 seconds after the initial connection. The server will not send any messages to a client while it is not authenticated.

## Verb list
\* `DO` is required for all verbs.
- ### AUTH: Authenticate a user
   > Required params: `FROM <username>`
- ### CONNECT: Connect to a channel
   > Required params: `TO <channel-name>`
- ### SEND: Send a message in a channel
   > Required params: `IN <channel-name>`, `WITH <message>`  
   > Optional params: `AT <timestamp>`
- ### ALERT: Send a server alert in a channel
   > Required params: `WITH <message>`  
   > Optional params: `IN <channel-name>`, `AT <timestamp>`

   \*Alerts should probably be limited to admins  
   \*If the `IN` parameter is omitted, the alert is global
- ### REMEMBER: Request message history
   > Required params: `FROM <timestamp>`, `TO <timestamp>`, `IN <channel-name>`
- ### MUTE: Mute a user in a channel
   > Required params: `TO <username>`, `IN <channel-name>`  
   > Optional params: `WITH <message>`
- ### KICK: Kick a user
   > Required params: `TO <username>`  
   > Optional params: `IN <channel-name>`, `WITH <message>`
- ### BAN: Ban a user
   > Required params: `TO <username>`  
   > Optional params: `IN <channel-name>`, `WITH <message>`
- ### IPBAN: Ban an IP
   > Required params: `TO <ip>`  
   > Optional params: `IN <channel-name>`, `WITH <message>`
- ### CREATE: Create a channel
   > Required params: `WITH <channel-name>`
- ### DELETE: Delete a channel
   > Required params: `TO <channel-name>`

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
IN general-chat
WITH
yoo good morning
how you been?
```
> Request to send a message in channel with name `general-chat`, with the message content being:  
> `yoo good morning`  
> `how you been?`

---
```
DO ALERT
IN welcome
WITH
Okkio connected!
```
> Request to send a server alert in channel with name `welcome` with the alert content being `Okkio connected!`

---
```
DO BAN
TO Forki
IN league
WITH
Advertising is prohibited...
```
> Requests a ban on user with username `Forki`, displaying the message `Advertising is prohibited...` in channel with name `league`

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
- The server does not use the `IN` parameter; this is because the server should only send the message to users connected to the concerned channel. E.g., the server should send an `ALERT` about channel `game-chat` ONLY to users that are connected to channel `game-chat`. So if the client receives a message, it is intended for the channel the user is currently connected to, making the `IN` parameter redundant.
- The same reasoning above applies to the `TO` parameter. If a user has an action performed on them like a ban or a mute, the server is only obligated to notify other users using an `ALERT` accompanied by a message explaining the details using the `WITH` parameter.
- The server should store the username given in the `AUTH` request for the current websocket connection. The username can then be used by the server to check for authority level, show message sender names, or to target infractions.

## Verb list
Keeping the details mentioned above in mind, besides performing backend operations, the server is obligated to notify the users using `SEND` for messages and `ALERT` for server alerts.

- ### SEND: Tells the client that a user has sent a message
   > Required params: `FROM <username>`, `WITH <message>`  
   > Optional params: `AT <timestamp>`
- ### ALERT: Displays a server message 
   > Required params: `WITH <message>`  
   > Optional params: `AT <timestamp>`

Besides displaying messages and alerts, the server also needs to respond to `AUTH` and `CONNECT` requests, this is done using the following verbs:
- ### ACCEPT: Accepts the request from the client
   > No params
- ### REFUSE: Refuses the request from the client
   > Optional params: `WITH <reason>`

### Multisend
When the server receives a `REMEMBER` request from a client, the server is expected to forward all the messages that users have sent in the specified period, this is done through the `POPULATE` server verb. When sending a `POPULATE` from the server, the `WITH` parameter will house multiple message requests from the server so the client can populate its UI with the message history.

- ### POPULATE: Send multiple messages at once
   > Required params; `WITH <array-of-messages>`

In the `WITH` argument, seperate message requests should start, end, and be seperated using a sequence of 3 dollar signs `/*$*/` like so:
```
DO POPULATE
WITH
/*$*/
DO SEND
FROM Okkio
AT 1714754642
WITH
yoo good morning
how you been?
/*$*/
DO SEND
FROM Psycho
AT 1714754703
WITH
i'm good, just working on the protocol
/*$*/
DO SEND
FROM Forki
AT 1714754754
WITH
league? 💀
/*$*/
DO ALERT
AT 1714715436
WITH
User Forki has been banned
/*$*/
```
It's then up to the client to re-interpret the nested message requests to populate the UI sequentially.

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
TO eid-drip
```

The server refuses the request
```
DO REFUSE
WITH
You are not allowed to access this channel
Please contact the administrator
```