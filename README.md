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
```
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
    <p><span style="padding-left: 10px">All</span> these things, had I improved them as I ought to have done, and as reason and religion had dictated to me, would have taught me to search farther than human enjoyments for a <strong>full felicity</strong>, and that there was something which <font style="font-weight: bold;">certainly</font> was the reason and end of life, superior to all these things, and which was either to be possessed, or at least hoped for, on this side the grave.</p>
    <p><span style="padding-left: 10px">But</span> my <font style="font-weight: bold;">sage counsellor</font> was gone, I was like a ship without a pilot, that could only run before the wind; my thoughts run all away again into the old affair, my head was quite turned with the whimsies of foreign adventures; and all the pleasing innocent amusements of my farm and my garden, my cattle and my family, which before entirely possessed me, were nothing to me, had no relish, and were like music to one that has no ear, or food to one that has no taste: in a word, I resolved to leave off housekeeping, let my farm, and return to London; and in a few months after I did so.</p>
</body>
</html>
```

#### Obfusticated HTML
```
<!DOCTYPE html>
<html>
<head>
    <title>Caa Feby yeb Xaijzapvic we Yinvivta Jexpiv (1808) vu Jezfus Aktae</title>
</head>
<body>
    <h1 style="font-size: large;">
        <strong>Suvzask wand</strong>: Haw Awux wuz Heldavimga ye Sojlagre Zuuwpo (1808) pu Tonjil Baers
    </h1>
    <div>
        <a href="https://www.gutenberg.org/ebooks/12623" style="color: blue; text-decoration: none;">O Vugkaizur xefezpi uBfeu</a>
    </div>
    <p><span style="padding-left: 10px">Cav</span> joymu wipvii, eni U jainwafy hita da A xemgi qe ricb yefr, kob le yiobwo hep laovjayh nak orrosnaj ka yo, seekp keck qesvag si ro zulavh zabmisu fufz yadco dejmokquem lab e <strong>seaf ojqibwux</strong>, pab seaf xegji lab xuemgeeyo kulqi <font style="font-weight: bold;">wufooergu</font> oyn saz dogmuk cul eni gi bise, siujdawj xe nap xinki zulavh, noh jeief bob imxesj ak pa kolafwoeg, ub et fepxu wioha caa, xa macu retw pot fepxu.</p>
    <p><span style="padding-left: 10px">Bis</span> ma <font style="font-weight: bold;">keck xoqziqsesj</font> haw lozr, E ofw retw u wogu razdecd e vobac, sosr fenze newg sip gurjaf yep geng; ex micmouhi eni dox kieo matji emne ujr sip qosaxu, ub luyx uwt zodfe jezfus qayr ibc houtcocq ye hehpeub etxayqoijz; ofw aky jaf gaswefju catgozuc quugfiagxa mu pe zukf yeb ye yismes, aq zarfer wob ro okpaoq, nedcu urhefz mifpuhzo qowqauduu yo, tabu wakrill yo ko, uwq yo rokqop, neu luyx woaq wucbe ze pit laln kua ya lor, qo jujw ma uwt woaq aho qi qepve: le o weuh, E tolabmuz ak jeief bov wurjukyiabco, yax qi juvg, saz rabfil ru Zogmoy; bis ir u jaf tekpew qaoxn O siu de.</p>
</body>
</html>
```

### Example 3 - Retaining XML Formatting
#### Original XML
```
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
```
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
```
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
```
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




