# Technical specifications for the **L**eague **S**ockets **M**essaging **P**rotocol (LSMP)
This document lists the technical specifications and details for implementing LSMP in your websocket server.  
LSMP is a custom protocol intended for use on top of the websockets protocol for the purpose of a live chat messaging application.  
*Version: 0.5.0*

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
- Location: The channel the message was sent in
- Date: The date-time stamp associated with the message (in Unix time)
- Payload: The text content of the message

## Details
- Request parameters must be seperated by new lines (CRLF: `\r\n`), with the parameter keyword (`DO`, `FROM`...etc) starting the line, followed by a space, then followed by the argument (the value associated with the parameter). And finally, the new line characters `\r\n`.
- Each request must contain and start with the `DO` parameter.
- Each parameter (`DO`, `FROM`...etc) should be in their own line with their respective argument (verb, subject...etc).
- The `WITH` parameter must be in a line of its own, it must also be the last parameter in the request, everything after this line until the end of the request (EOF) will be considered as the argument (the payload, A.K.A message body/content), including any subsequent new lines.
- Parameter keywords (`DO`, `FROM`...etc) and verbs are case**IN**sensitive (can be UPPERCASE or lowercase). However, it is advised to use UPPERCASE in order to standardize the format.
- Depending on the verb, certain parameters may be optional or completely omitted. For example, the `FROM` parameter does not need to provided in subsequent requests after `AUTH` as the server is expected to store the username for the current connection. The parameter should be ignored in such cases even if provided by the request. Optional parameters however can alter the functionality of certain actions.
- If the `AT` parameter is omitted, the timestamp is set to the exact time the request was received.
- The first request from the client must be an `AUTH` verb with the username passed in the `FROM` parameter. The server will not send any messages to a client while it is not authenticated. If a message is sent from an unregistered client, the server will reply with a `REFUSE`.

## Verb list
\* `DO` is required for all verbs.
- ### AUTH: Authenticate a user
   > Required params: `FROM <username>`
- ### IDENTIFY: Request user list
   > Required params: `IN <channel-name>`
- ### INQUIRE: Request room list
- ### REMEMBER: Request message history
   > Required params: `FROM <message-order>`, `TO <message-order>`, `IN <channel-name>`
- ### SEND: Send a message in a channel
   > Required params: `IN <channel-name>`, `WITH <message>`  
---
### Channel management (Admin only):
- ### CREATE: Create a channel
   > Required params: `WITH <channel-name>`  
- ### DELETE: Delete a channel
   > Required params: `WITH <channel-name>`
---
### Infractions (Admin only):
- ### MUTE: Mute a user in a channel
   > Required params: `TO <username>`  
   > With reason: `WITH <reason>`
- ### KICK: Kick a user
   > Required params: `TO <username>`  
   > With reason: `WITH <reason>`
- ### BAN: Ban a user
   > Required params: `TO <username>`  
   > With reason: `WITH <reason>`
- ### BANIP: Ban an IP
   > Required params: `TO <ip>`  
   > With reason: `WITH <reason>`
- ### UNMUTE: Unmute a user
   > Required params: `TO <username>`  
   > With reason: `WITH <reason>`
- ### UNBAN: Unban a user
   > Required params: `TO <username>`  
   > With reason: `WITH <reason>`
- ### UNBANIP: Unban an IP
   > Required params: `TO <ip>`  
   > With reason: `WITH <reason>`


## Example messages

```
DO AUTH
FROM Akram
```
> Requests authentication from the server using the username `Akram`

---
```
DO SEND
WITH
yoo good morning
how you been?
```
> Request to send a message (defaults to `general-chat`) with the message content being:  
> `yoo good morning`  
> `how you been?`

---
```
DO BAN
TO Forki
WITH
Advertising is prohibited...
```
> Requests a ban on user with username `Forki`, with the reason being `Advertising is prohibited...`

# Server -> Client
The server sends messages to the client in a similar way, with a few minor differences.

## Details
- The server should store the username given in the `AUTH` request for the current websocket connection. The username can then be used by the server to check for authority level, show message sender names, or to target infractions.
- When the server receives a `SEND` request from a client, the server should fill in the `FROM`, `IN` and `AT` parameters with the username of the sender (stored in the point above), the channel name (defaults to `general-chat` if not provided), and timestamp respectively, before forwarding the message to all clients including the sender as a way to confirm the receiving of the message.

## Verb list
Keeping the details mentioned above in mind, besides performing backend operations, the server is obligated to notify the users using `SEND` WITHOUT a `FROM` parameter to signal that the message is sent from the server.  
The `AT` parameter for the timestamps is mainly used when the server is sending the message history to a client. Otherwise it is omitted and is to be interpreted as `date.now` by the client.

- ### SEND: Tells the client that a user has sent a message
   > Required params: `WITH <message>`  
   > From a user (omit for alert): `FROM <username>`  
   > In channel (defaults to `general-chat`): `IN <channel-name>`  
   > With a timestamp from history (defaults to `date.now`): `AT <timestamp>`

Besides displaying messages and alerts, the server also needs to respond to `AUTH` requests, as well as refuse `SEND` requests from unauthorized clients. This is done using the following verbs:
- ### ACCEPT: Accepts the request from the client
   > No params
- ### REFUSE: Refuses the request from the client
   > With response message: `WITH <reason>`

## Multisend verbs
### User List
When a new client authenticates using `AUTH`, the server will send the list of users that are currently connected (and authenticated) to this new client. Similarly, the server will also send the updated list to all other clients when the new client joins, or when another client disconnects.  
The client may also ask for the user list using the `IDENTIFY` verb.  
This is done via the `INTRODUCE` server verb, in which the server will provide the list of connected users in the form of an array.
- ### INTRODUCE: Send list of online users
   > Required params: `IN <channel-name>`, `WITH <array-of-users>`

In the `WITH` argument, the list of usernames should start, end, and be seperated using the character sequence `/*$*/` (forward slash, asterisk, dollar sign, asterisk, forward slash) with the seperator and each username in a different line like so:
```
DO INTRODUCE
IN general-chat
WITH
/*$*/
Psycho
/*$*/
Okkio
/*$*/
Asim
/*$*/
```
It's then up to the client to re-interpret the user array to populate the UI.

### Channel list
Same as the `INTRODUCE` verb for user list but for the list of channels using the verb `INFORM`.
- ### INFORM: Send list of channels
   > Required params: `WITH <array-of-channel-names>`
Example:
```
DO INFORM
WITH
/*$*/
general-chat
/*$*/
games
/*$*/
world-cup
/*$*/
eid-drip
/*$*/
```

### Message History
When the server receives a `REMEMBER` request from a client, the server is expected to forward all the messages that users have sent in the specified period, this is done through the `REMIND` server verb. When sending a `REMIND` from the server, the `WITH` parameter will house multiple message requests from the server so the client can populate its UI with the message history.

- ### REMIND: Send multiple messages at once
   > Required params: `IN <channel-name>`, `WITH <array-of-messages>`

In the `WITH` argument, seperate message requests should start, end, and be seperated using the character sequence `/*$*/` (forward slash, asterisk, dollar sign, asterisk, forward slash) like so:
```
DO REMIND
IN general-chat
WITH
/*$*/
DO SEND
FROM Okkio
IN general-chat
AT 1714754642
WITH
yoo good morning
how you been?
/*$*/
DO SEND
FROM Psycho
IN general-chat
AT 1714754703
WITH
i'm good, just working on the protocol
/*$*/
DO SEND
FROM Forki
IN general-chat
AT 1714754754
WITH
league? 💀
/*$*/
DO SEND
IN general-chat
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
Another user tries to authenticate with the same username while `Midfield` is currently logged in
```
DO AUTH
FROM Midfield
```

The server refuses the request
```
DO REFUSE
WITH
Username is already taken
```
