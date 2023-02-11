# JsonObject
read json without dependencies
- [[ in work ]] can use it for simple json, table and list not supported now

# Install
just copy JsonObject.cs in your project

# How use it
your json file
```json
{"x":5, "size": 0.5, "name":"jake", "point":{"x":0, "y":0}}
```
your code
```csharp
JsonObject jo = new JsonObject("file.json");
int x = jo.getInt("x");            //-> 5
float size = jo.getFloat("size");  //-> 0,5
string name = jo.getString("name");//-> jake
int point_x = jo.getObject("point").getInt("x");
int point_y = jo.getObject("point").getInt("y");
```
