# Market comparer

Skin price, volume from [buff163.com](https://buff163.com) and [market.csgo.com](https://market.csgo.com) download in xlsx format.

## Usages

Download the archive from the release and add the cookies.json file with the contents to the comparer folder:
```
{
  "Cookies": "Device-Id=XXXXXX; csrf_token=XXXXX; Locale-Supported=ru; game=csgo; session=XXX",
  "BaseUrlBuyers" : "https://buff.163.com/api/market/goods",
  "BaseUrlSellers" : "https://market.csgo.com/api/v2/prices/RUB.json",
  "TimeSleep":  500
}
```
Your cookies are located on buff.163.com, you can get them through the developer tool. 
Cmd + Option + I - for MacOS
<img width="885" alt="Снимок экрана 2025-01-14 в 14 48 03" src="https://github.com/user-attachments/assets/29d8080f-2a44-4bb4-866b-1377c4581e1d" />
