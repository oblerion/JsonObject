# JsonObject
read json without dependencies

# How use it
your json file
```json
{"x":5, "size": 0.5, "name":levis}
```
```csharp
JsonObject jo = new JsonObject("file.json");
int x = jo.getInt("x");            //-> 5
float size = jo.getFloat("size");  //-> 0.5
string name = jo.getString("name");//-> levis
```
