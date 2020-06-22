# azure-cosmosdb-changefeed

An implementation of consuming the CosmosDB Change Feed stream.

The example repo contains the following:
- Azure Function, implemented in .Net Core/C#.
- CosmosDB test client, implemented in Python 3.

## Azure Provisioning

Create the following in Azure Portal

1) A CosmosDB/SQL account
2) A database named **dev** within the CosmosDB account
3) Collections named **events** and **changes** in the **dev** database
4) The partition key for both collections is **/pk**, and the RU value 400
5) An Azure Function App, using **.Net Core** as the Runtime Stack

## Development Environments 

This app was created on Windows 10 with the Visual Studio IDE to author and publish/deploy 
the **Azure Function**.

The **CosmosDB test client** is implemented in Python 3, created and executed on macOS
with the Visual Studio Code editor.

This choice was intentional - to demonstrate interacting with Azure and CosmosDB
from multiple operating systems and programming languages.

Azure Functions can be implemented in many ways (IDE, CLI) and in one of several 
programming languages.  They can even be containerized.  Likewise, there are SDKs
for CosmosDB/SQL in several programming languages.  This repository demonstrates 
just one of these many possibilities.

## Data Flow in this Demonstration App

- Python client CLI program generates and upserts randomized North Carolina
  Postal Code documents in the **events** container.
- Azure Function consumes the stream of the CosmosDB/SQL change feed documents from the
  **events** container and persists them in the **changes** container in the same database.
- Python client CLI program can query both collections to identify restore candidates.
- Python client CLI program can selectively restore documents from the **changes** container
  into the **events** container.

