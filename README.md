# TextDataMasking
## Background
### What is Data Masking?
Data masking or data obfuscation is the process of modifying sensitive data in such a way that it is of no or little value to unauthorized intruders while still being usable by software or authorized personnel. Data masking can also be referred as anonymization, or tokenization, depending on different context.

Source: https://en.wikipedia.org/wiki/Data_masking


## Objective
The TextDataMasking software should be able to obfusticate sensitive `Text` data while:
1) Retaining the `Text` formatting such as paragraphing, capitalization, punctuations marks etc.
2) Retaining the `HTML` formatting such that the HTML is still well-formed
3) Retaining the `XML` formatting such that the XML is still well-formed
4) Retaining the `JSON` formatting such that the JSON is still well-formed


### Example 1 - Retaining Text Formatting
#### Original Text
  All these things, had I improved them as I ought to have done, and as reason and religion had dictated to me, would have taught me to search
farther than human enjoyments for a full felicity, and that there was something which certainly was the reason and end of life, superior to
all these things, and which was either to be possessed, or at least hoped for, on this side the grave.

  But my sage counsellor was gone, I was like a ship without a pilot, that could only run before the wind; my thoughts run all away again into the
old affair, my head was quite turned with the whimsies of foreign adventures; and all the pleasing innocent amusements of my farm and my
garden, my cattle and my family, which before entirely possessed me, were nothing to me, had no relish, and were like music to one that has
no ear, or food to one that has no taste: in a word, I resolved to leave off housekeeping, let my farm, and return to London; and in a few months
after I did so.

Source: The Life and Adventures of Robinson Crusoe (1808) by Daniel Defoe (https://www.gutenberg.org/ebooks/12623)

#### Obfusticated Text
  Yem tebya vontog, vov O meyjeyva bekv ek O lourd ko giep ille, tuu ku cankot qot abobovzo vov ofjuurga pi pa, yojpa sose xifnuj ye ga nubwoi
poxwokw pewp gumci biouwzorvo exn o foky xawfehud, hav hidr jouva coo gifkuaign omtuu cepgonhou kox tuu vogzod esg oho ci qoek, rizsotlu ye
uzw yojpa riytiz, tuu izmol won jayyix nu va alqeuzsez, ka ra kopro kipba usg, zi kanc dofv niu upbib.

  Zaw re xivs poqjulyeju tag jisd, E teg yoxc u foky maiqepe o qepno, talo wahme lufs ebd guievu fut zopb; li rormuabc tob uzw kicx rihsi zopb pul
yem cospia, yu baen nea eyqel poydab zopb tax sunopsio nu komoiva xachotbeuv; pok cez juy pegdesuz ukoiwboh ugqabuiiij ek ik yeaf yuc ya
arkeiu, ta cenola row ya guievu, vurce kapvuq reianjok gacyaxpup ta, weks witjabr ha ba, uvw uv kuyiuq, vov jauw biik taawt ke seq porr gak
ra yeh, je jiws qe seq fibf fei go hiloz: ju i mugq, E pegdesuz we rikwu pul girimpeyxerg, tuu we sose, poo cenola te Hiwzuy; qoa ew u xax sofzeu
izmol U jai ga.


### Example 2 - Retaining HTML Formatting
#### Original HTML
```html
<!DOCTYPE html>
<html>
<head>
    <title>The Life and Adventures of Robinson Crusoe (1808) by Daniel Defoe</title>
</head>
<body>
    <h1 style="font-size: large;">
        <strong>Excerpt from</strong>: The Life and Adventures of Robinson Crusoe (1808) by Daniel Defoe
    </h1>
    <div>
        <a href="https://www.gutenberg.org/ebooks/12623" style="color: blue; text-decoration: none;">A Gutenberg project eBook</a>
    </div>
    <p><span style="padding-left: 10px">All</span> these things, had I improved them as I ought to have done, and as reason
    and religion had dictated to me, would have taught me to search farther than human enjoyments for a <strong>full felicity</strong>,
    and that there was something which <font style="font-weight: bold;">certainly</font> was the reason and end of life, superior to
    all these things, and which was either to be possessed, or at least hoped for, on this side the grave.</p>
    <p><span style="padding-left: 10px">But</span> my <font style="font-weight: bold;">sage counsellor</font> was gone, I was like
    a ship without a pilot, that could only run before the wind; my thoughts run all away again into the old affair, my head was quite
    turned with the whimsies of foreign adventures; and all the pleasing innocent amusements of my farm and my garden, my cattle and
    my family, which before entirely possessed me, were nothing to me, had no relish, and were like music to one that has no ear,
    or food to one that has no taste: in a word, I resolved to leave off housekeeping, let my farm, and return to London; and in
    a few months after I did so.</p>
</body>
</html>
```

#### Obfusticated HTML
```html
<!DOCTYPE html>
<html>
<head>
    <title>Rux Noxi wuj Biyziwimda jo Akkapyer Ecazgu (1808) qe Jemwuo Bijka</title>
</head>
<body>
    <h1 style="font-size: large;">
        <strong>Aceomxa jeoz</strong>: Zao Cixk uda Lehtemqawf fe Wusgokdo Unguqn (1808) ra Pexcel Zogba
    </h1>
    <div>
        <a href="https://www.gutenberg.org/ebooks/12623" style="color: blue; text-decoration: none;">O Onnufosuu turfazc cOrro</a>
    </div>
    <p><span style="padding-left: 10px">Ciw</span> figek gegxuw, wiv E qaryueie ixru wa A tireu sa wotp duyf, meo mu tuprow
    erg cukiypat cir oxgoqsib ov ro, wocgi widz foftuq si sa helnou viniuos numi hedur uwxahkabii rux o <strong>pegv budlaoys</strong>,
    oza posf loher alk heawcaqro eyejc <font style="font-weight: bold;">nimajhamx</font> qaq fiy voyzuk ida muu uk huhe, nocrasru hu
    hoy loher bamgax, ivp zogba geb bewyin ri li ciizfibxu, pi wa hejfo deujb xoy, ce cigh zipk cui nidxu.</p>
    <p><span style="padding-left: 10px">Ipo</span> ov <font style="font-weight: bold;">regw rurraifsuv</font> lef regw, E joo luhe
    o dovl heuxjih u zogba, wikk fijco uspe koa guckak koa qahg; ga ixfurfeg tia eki bidv ejcad nibq ifi ukc xuhven, xo ponr yeh okuzj
    taewpu siog ave ladaspoo wa gidhuui qarkeaokxo; vap lib geb subbizqa iyehnoij apgaitvily al ro saur hou ye qiedoe, ro haxgio wor
    ri adsafd, ejcad yawcen hiizhijw ciizfibxu lu, pajn albeqvu wa ro, qaq ow soumed, fia xecj novu bocya hu fat vuxr fat wa rao,
    uk sesk qi alk pegv liy oj viszo: be e safg, A ixfurfeg wa cubgi zic iffowfatraet, geb iv sits, ipo tisqaz ro Xuhven; jap te
    i ida qerjeg vewgi A cip de.</p>
</body>
</html>
```


### Example 3 - Retaining XML Formatting
#### Original XML
```xml
<?xml version="1.0"?>
<cart id="1291140274">
    <item id="CT0001B">
        <name>Acacia Coffee Table</name>
        <price>499.99</price>
        <quantity>1</quantity>
    </item>
    <item id="DT0120B">
        <name>Marble Dining Table 8 Seater</name>
        <price>2999.99</price>
        <quantity>1</quantity>
    </item>
    <item id="DC1011C">
        <name>Deluxe Dining Chair Pearl</name>
        <price>299.99</price>
        <quantity>8</quantity>
    </item>
</cart>
```

#### Obfusticated XML
```xml
<?xml version="1.0"?>
<cart id="1291140274">
    <item id="CT0001B">
        <name>Ruofhi Kecabl Suoad</name>
        <price>499.99</price>
        <quantity>1</quantity>
    </item>
    <item id="DT0120B">
        <name>Kiqsaq Rewlub Atdac 8 Jaibwa</name>
        <price>2999.99</price>
        <quantity>1</quantity>
    </item>
    <item id="DC1011C">
        <name>Hiparn Zarfer Xegji Mahli</name>
        <price>299.99</price>
        <quantity>8</quantity>
    </item>
</cart>
```

### Example 4 - Retaining JSON Formatting
#### Original JSON
```json
{
  "cart_id": 1291140274,
  "discount_code": null,
  "items": [
    {
      "id": "CT0001B",
      "name": "Acacia Coffee Table",
      "price": 499.99,
      "quantity": 1
    },
    {
      "id": "DT0120B",
      "name": "Marble Dining Table 8 Seater",
      "price": 2999.99,
      "quantity": 1
    },
    {
      "id": "DC1011C",
      "name": "Deluxe Dining Chair Pearl",
      "price": 299.99,
      "quantity": 8
    }
  ]
}
```

#### Obfusticated JSON
```json
{
  "cart_id": 1291140274,
  "discount_code": null,
  "items": [
    {
      "id": "YJ4188J",
      "name": "Afvimv Bakwib Hiwni",
      "price": 499.99,
      "quantity": 1
    },
    {
      "id": "PK8770D",
      "name": "Qudtor Hixuoa Xikze 8 Tujivg",
      "price": 2999.99,
      "quantity": 1
    },
    {
      "id": "FS2918R",
      "name": "Iyteiv Buuqek Jekfi Jipce",
      "price": 299.99,
      "quantity": 8
    }
  ]
}
```

## How to Use
The `DatabaseMaskerWeb` web application is the User Interface for the TextDataMasking solution. It can be used to Start Database Masking jobs to Mask selected Database Tables and Columns with the desired Masking Options.

There are currently 2 RDBMS (Relational Database Management System) supported:
1) Microsoft SQL
2) Postgre SQL

#### Important
```
Do NOT use the software directly on your staging or production databases.
Only use this software on a restored copy of the database backup.
```

### Step 1: Configuration
The `DatabaseMaskerWeb` web application needs to be configured with the target database's `Connection String` and corresponding `Database Masking Provider` prior to launching the web application in a browser.

The configuration is done by editing the `appsettings.json` file residing in the root folder of the `DatabaseMaskerWeb` web application.
#### Example of appsettings.json
```
{
  ...,
  "ConnectionStrings": {
    "Sql": "Data Source=.\\MSSQLSVR;Integrated Security=True;Persist Security Info=False;Pooling=False;
    Multiple Active Result Sets=False;Encrypt=False;Trust Server Certificate=False;Command Timeout=0;Database=DataMaskingDB",
    "PostgreSql": "Server=127.0.0.1;Port=43594;Database=DataMaskingDB;User Id='DataMaskingUser';Password='DataMaskingPassword';"
  },
  "DatabaseMaskingProviders": {
    "Sql": "DatabaseMasking.Sql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
    "PostgreSql": "DatabaseMasking.osandfreesql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
  },
  ...
}
```
#### Important
```
Please note that the JSON attribute names for the `ConnectionStrings` and the `DatabaseMaskingProviders` sections
needs to be matching. Mismatched Connection Strings and Database Masking Providers will NOT be loaded into the
Web Application.
```

### Step 2: Launch the DatabaseMaskerWeb
To launch the 'DatabaseMaskerWeb' web application, the user needs to either compile and run the project from Visual Studio or deploy the project to a web server or one of the other deployment targets supported by ASP.NET Core.

### Step 3: DatabaseMaskerWeb Home Page
The user starts on the 'DatabaseMaskerWeb' web application Home Page once the website is launched. The currently running jobs will be displayed in a notification.
[DatabaseMaskerWeb Home Page](documentation/DatabaseMaskerHomePage.png)

### Step 4: Starting a Database Masking Job
To start a Database Masking Job, select the `Mask Database` menu.
[Mask Database Menu](documentation/MaskDatabaseMenu.png)

Once the user clicks on the `Mask Database` menu, the user is brought to the Instructions page. To proceed to the next step, click on the `Next` button located near the bottom right of the page.
[Mask Database Instructions](documentation/MaskDatabaseInstructions.png)

### Step 5: Select the DataSource
Choose the Database to mask by selecting from the availiable Datasources. Please note that the DataSources listed are those that were configured in the web application's appsettings.json. 
[Select DataSource](documentation/SelectDataSource.png)

### Step 6: Select the Tables and Columns to Mask
The user should select the Tables and Columns to include in the Database Masking Job. The user can also specify the Masking Options for each column. However the user's Masking Options for JSON and XML are overriden if the selected column is of JSON or XML data types. This is because most databases would validate that the JSON or XML are well-formed before storing them.
[Select Tables and Columns](documentation/SelectTablesAndColumns.png)

Once the included columns are selected, click on the Run Task button to start the Database Masking Job.
[Start Database Masking Job](documentation/StartDatabaseMaskingJob.png)

### Step 7: Verify running Database Masking Jobs
Once the new job is started, the user is brought back to the home page where all the currently running jobs are displayed.
[Running Database Masking Jobs](documentation/RunningDatabaseMaskingJobs.png)

