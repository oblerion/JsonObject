# JsonObject.cs
read json without dependencies
- [[ in work ]] can use it for simple json, array not supported now

# Install
just copy JsonObject.cs in your project

# How use it
your json file
```json
{"x":5, "size": 0.5, "name":"jake", "point":{"x":12, "y":34}}
```
your code
```csharp
JsonObject jo = new JsonObject("file.json");
int x = jo.getInt("x");            //-> 5
float size = jo.getFloat("size");  //-> 0,5
string name = jo.getString("name");//-> jake
int point_x = jo.getObject("point").getInt("x"); // -> 12
int point_y = jo.getObject("point").getInt("y"); // -> 34
```
[full api](https://github.com/oblerion/JsonObject/wiki)
