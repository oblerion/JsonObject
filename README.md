# JsonObject.cs
c# read json without dependencies
- [[ in work ]] can use it for simple json, array 2d not supported now
# Features
- read int
- read float
- read string
- read bool
- read int array 1D
- read object
- get object empty or reading file error

# Install
just copy JsonObject.cs in your project

# How use it
your json file
```json
{
"x":5, "size": 0.5, "name":"jake", "point":{"x":12, "y":34}, 
"visible":true, "array":[0,2,4]
}
```
your code
```csharp
JsonObject jo = new JsonObject("file.json");
if(jo.IsEmpty()==false)
{
  int x = jo.GetInt("x");            //-> 5
  float size = jo.GetFloat("size");  //-> 0,5
  string name = jo.GetString("name");//-> jake
  int point_x = jo.GetObject("point").GetInt("x"); // -> 12
  int point_y = jo.GetObject("point").GetInt("y"); // -> 34
  bool visible = jo.GetBool("visible"); //-> true
  List<int> arr = jo.GetArray("array");
}
```
[full api](https://github.com/oblerion/JsonObject/wiki)
