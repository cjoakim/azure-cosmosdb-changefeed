# azure-cosmosdb-changefeed

An implementation of consuming the CosmosDB Change Feed stream, using another container
in the same database as the sink for the stream.

The example repo contains the following:
- Azure Function, implemented in .Net Core/C#.
- CosmosDB test client, implemented in Python 3.

## Azure Provisioning

Create the following in Azure Portal

1) A CosmosDB/SQL account
2) A database named **dev** within the CosmosDB account
3) Collections named **events** and **changes** in the **dev** database
4) Set the partition key for both collections to **/pk**, RU value 1000, and default indexing
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
  Postal Code documents in the **events** container.  The document id and pk are the zip code.
- Azure Function consumes the stream of the CosmosDB/SQL change feed documents from the
  **events** container and persists them in the **changes** container in the same database.
- Python client CLI program can query both collections to identify restore candidates.
- Python client CLI program can selectively restore documents from the **changes** container
  into the **events** container.

---

## Processing Examples

### Python Client Setup

```
$ git clone https://github.com/cjoakim/azure-cosmosdb-changefeed.git
$ cd azure-cosmosdb-changefeed    
$ cd py                          # <-- the python client code is in this directory
$ ./venv.sh create               # <-- create the python virtual environment, install PyPI libs
$ source bin/activate            # <-- activate the python virtual environment
$ pip list                       # <-- list the python libraries, your output should be similar
Package         Version
--------------- ---------
arrow           0.15.7
azure-core      1.6.0
azure-cosmos    4.0.0
certifi         2020.6.20
chardet         3.0.4
click           7.1.2
docopt          0.6.2
idna            2.9
pip             20.1.1
pip-tools       5.2.1
python-dateutil 2.8.1
requests        2.24.0
setuptools      41.2.0
six             1.15.0
urllib3         1.25.9
```

The primary file is **main.py**, and CosmosDB logic is in file **pysrc/cosmos.py**.
The latest version 4.0.0 of the CosmosDB SDK is used in this project.

### Truncate the Containers - remove all documents from each

The following deletes up to 1000 documents in each container.
```
$ python main.py truncate_container dev events 1000
$ python main.py truncate_container dev changes 1000
```

### Upsert 5000 documents in the events container

This logic executes a loop 5000 times and randomly selects and augments
a North Carolina zip code in file **data/nc_zipcodes.json** in each iteration
of the loop.  The zip code value (i.e. - 20836) is used as both the **id**
and the **pk** of the upserted documents.  Since there are only 1075 zip codes 
in the file, some of the iterations are inserts while others are updates.

```
$ python main.py populate_cosmos_zipcodes dev events 5000 true
```

Example output:
```

```

Corresponding example document with CosmosDB generated attributes.
```

```

### Query Both Collections

```
SELECT VALUE COUNT(1) FROM c
select * from c where c._ts > 1592923320
```

```

```