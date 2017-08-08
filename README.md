# Pegasus
A tool to create SSIS packages programmatically.

This tool uses the [SSIS Object Model and API](https://msdn.microsoft.com/en-us/library/mt574046.aspx).

Wrappers are created around a DtsObject to simplify usage and hide operations where possible.


## Available DtsObjects

Currently, the following are available.

### Control Flow Tasks:
1. Package
2. Sequence Container
3. Data Flow Task
4. Execute Package Task
5. Execute Sql Task
6. Script Task

### Pipeline Components:
1. Conditional Split Component
2. Lookup Component
3. Derived Column Component
4. OleDb Destination Component
5. OleDb Source Component
6. Odbc Source Component 
7. Flat File Source Component
8. Multicast Component
9. Script Component
10.Rowcount Component

### Others:
1. Variables
2. Parameters
3. Connection Managers
4. Event Handlers
