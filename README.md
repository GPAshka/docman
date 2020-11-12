# docman

Handling commands
    - Validate the command
    - Turn the command into an event
    - Persist the event and publish it to interested parties

1. When a command is received, the current state of the affected entity is computed from its history.
2. The command is turned into an event and applied to the entity’s state, obtaining its new state.
3. The event is persisted and published to interested parties for further processing.

System can:
    - create new document
    - approve document