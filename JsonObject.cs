/*
MIT License JsonObject v0.3
Copyright (c) 2023 oblerion


Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.IO;
using System.Collections.Generic;
public class JsonObject
{
	private Dictionary<string,string> _ds = new Dictionary<string,string>(); 
	private Dictionary<string,int> _di = new Dictionary<string, int>();
	private Dictionary<string,float> _df = new Dictionary<string, float>();
	private Dictionary<string,JsonObject> _dj = new Dictionary<string,JsonObject>();
	private Dictionary<string,bool> _db = new Dictionary<string, bool>();
	private Dictionary<string,List<int>> _dai1 = new Dictionary<string, List<int>>();
	// private Dictionary<string,List<List<int>>> _dai2 = new Dictionary<string, List<List<int>>>();
	private List<string> _list_obj_key = new List<string>();
	public JsonObject()
	{}
	public JsonObject(string sfile)
	{
		int in_obj = 0;
		int in_array = 0;
		List<string> ldata = new List<string>();
		int i_tps=0;
		string data;
		data = _readFile(sfile);
		if(data.Length==0) 
			data=sfile;
		if(data.Length>1)
		{
			data = _filter(data);
			if(data[0]=='{') data = _extract(data,'{','}');
			// else if(data[0]=='[') data = _extract(data,'[',']');
			//Console.WriteLine($"{data}");
			for(int i=0;i<data.Length;i++)
			{
				switch(data[i])
				{
					case '[': in_array++;
					break;
					case '{': in_obj++;
					break;
					case ']': in_array--;
					break;
					case '}': in_obj--;
					break;
					case ',':
						if(in_array<=0 && in_obj<=0)
						{
							ldata.Add(data.Substring(i_tps,i-i_tps));
							i_tps = i+1;
						}
					break;
				}
				if(i==data.Length-1)
				{
					ldata.Add(data.Substring(i_tps,i-i_tps+1));
				}
				
			}
			foreach(string s2 in ldata)
			{
				if(s2[0]=='"')
				{
					string name = _extract(s2,'"','"');
					string value = _extract(s2,':');
					if(value!="")
					{
						if(value[0]=='"') _addString(name,value);
						else if(value[0]=='{')
						{
							_list_obj_key.Add(name);
							_addObject(name,value);
						}
						else if(value[0]=='[')
						{
							_addArray(name,value);
						}
						else if(value=="true" || value=="false")
						{
							_addBool(name,value);
						}
						else
						{
							_addInt(name,value);
							_addFloat(name,value);
						}
					}
				}
				//Console.WriteLine($"{name} {value}...");
			}
		}
	}
	private void _addArray(string name,string value)
	{
		string ext = this._extract(value,'[',']');
		if(ext.Length==0) return;
		
		if(ext[0]=='[')
		{
			// this._dai2[name] = new List<List<int>>();

			// string ext2 = this._extract(ext,'[',']');
		}
		else if(ext[0]=='{')
		{
			int in_array=0;
			int in_obj=0;
			string tstr="";
			JsonObject jo = new JsonObject("");
			for(int i=0;i<ext.Length;i++)
			{
				switch(ext[i])
				{
					case '[': in_array++;
					break;
					case '{': in_obj++;
					break;
					case ']': in_array--;
					break;
					case '}': in_obj--;
					break;
					case ',':
						if(in_array<=0 && in_obj<=0)
						{
							jo._list_obj_key.Add($"{jo.GetNbObject()}");
							jo._addObject($"{jo.GetNbObject()}",tstr);
							tstr = "";
						}else tstr += ext[i];
					break;
				}
				if(ext[i]!=',')
					tstr += ext[i];
			}
			jo._list_obj_key.Add($"{jo.GetNbObject()}");
			jo._addObject($"{jo.GetNbObject()}",tstr);
			_list_obj_key.Add(name);
			_dj[name]=jo;
		}
		else
		{
			this._dai1[name] = new List<int>();
			string tstr="";

			for(int i=0;i<ext.Length;i++)
			{
				if(ext[i]==',')
				{
					this._dai1[name].Add(Int32.Parse(tstr));
					tstr = "";
				}
				else
				{
					tstr += ext[i];
				}
			}
			this._dai1[name].Add(Int32.Parse(tstr));
		}
	}
	private void _addObject(string name,string value)
	{
		_dj[name] = new JsonObject(value);
	}
	private void _addBool(string name,string value)
	{
		bool lbool=false;
		if(value=="true") lbool=true;
		_db[name]=lbool;
	}
	private void _addString(string name,string value)
	{
		_ds[name] =	_extract(value,'"','"');
	}
	private void _addInt(string name,string value)
	{
		if(!value.Contains('.')) _di[name] = Int32.Parse(value);
	}
	private void _addFloat(string name,string value)
	{
		if(value!=null)
		{
			if(value.Contains('.'))
			{
				for(int i=0;i<value.Length;i++)
				{
					if(value[i]=='.')
					{
						double o = (float)Int32.Parse(value.Substring(0,i));
						string ls = value.Substring(i+1,value.Length-(i+1));
						o += Int32.Parse(ls)/Math.Pow(10,ls.Length);
						_df[name] = (float)o;
						break;
					}
				} 
			}
			// else if(Int32.Parse(value)==0) 
			// 	_df[name]=0;
		}
	}
	private bool IsNumeric(string s)
    {
        foreach (char c in s)
        {
            if (!char.IsDigit(c) && c != '.')
            {
                return false;
            }
        }
        return true;
    }
	private string _readFile(string sfile)
	{
		
		if(sfile.Length>0 && File.Exists(sfile))
		{
			StreamReader file = File.OpenText(sfile);
			if(file!=null)
        		return file.ReadToEnd();
		}
		return "";
	}
	public  void WriteFile(string sfile)
	{
		if(File.Exists(sfile)) File.Delete(sfile);
		string sdata;
		if(IsEmpty()) sdata = "{\n}";
		else sdata = ToString();
		File.AppendAllText(sfile,sdata);
	}

	private string _filter(string s)
	{
		string ls="";
		for(int i=0;i<s.Length;i++)
		{
			
			if(s[i]!=' ' && 
				s[i]!='\n' && 
				s[i]!='\b' &&
				s[i]!='\r' &&
				s[i]!='\t') 
			{
				ls+=s[i];
			}
		}
		return ls;
	}
	private string _extract(string s,char cbegin,char cend)
	{
		int i_beg=-1;
		int i_end=-1;
		bool b_sameC= (cbegin==cend);
		int i_nbSameC=0;
		for(int i=0;i<s.Length;i++)
		{
			if(i_beg==-1 && s[i]==cbegin) i_beg = i+1;
			else if(i_beg>-1 && s[i]==cbegin && !b_sameC)
			{
				i_nbSameC++;
			}
			else if(i_beg>-1 && s[i]==cend)
			{
				if(!b_sameC && i_nbSameC>0)
					i_nbSameC--;
				else
				{
					i_end = i;
					break;
				}
			}
		}

		if(i_beg+i_end>0 && i_end-i_beg < s.Length) 
			return s.Substring(i_beg,i_end-i_beg);
		return "";
	}
	private string _extract(string s,char cbegin)
	{
		int i_beg=-1;
		int i_end=-1;
		for(int i=0;i<s.Length;i++)
		{
			if(i_beg==-1 && s[i]==cbegin) i_beg = i+1;
		}
		if(i_beg+i_end>0 && i_end-i_beg <= s.Length) 
			return s.Substring(i_beg,s.Length-i_beg);
		return "";
	}
	public override string ToString()
	{
		int i_nb=0;
		int nb = _ds.Count+_di.Count+_df.Count+_db.Count+_dai1.Count+_dj.Count;
		string[] as_key = new string[nb];
		string[] as_value = new string[nb];
		if(IsEmpty()) return " ";//"this JsonObject is empty";
		foreach (var (name,value) in _ds)
		{
			as_key[i_nb]='"'+$"{name}"+'"';
			as_value[i_nb]='"'+$"{value}"+'"';
			i_nb++;
		}
		foreach (var (name,value) in _di)
		{
			as_key[i_nb]='"'+$"{name}"+'"';
			as_value[i_nb]=$"{value}";
			i_nb++;
		}
		foreach (var (name,value) in _df)
		{
			int iuni = (int)Math.Floor(value);
			int idec = (int)Math.Floor(value*10000)-iuni*10000;
			as_key[i_nb]='"'+$"{name}"+'"';
			as_value[i_nb]=$"{iuni}.{idec}";
			i_nb++;
		}
		foreach (var (name,value) in _db)
		{
			string sdata;
			if(value) sdata="true";
			else sdata="false";
			as_key[i_nb]='"'+$"{name}"+'"';
			as_value[i_nb]=sdata;
			i_nb++;
		}
		foreach (var (name,value) in _dai1)
		{
			string sdata;
			sdata = "[";
			for(int i=0;i<value.Count;i++)
			{
				sdata += $" {value[i]}";
				if(i<value.Count-1) sdata += ", ";
			}
			sdata += "]";
			as_key[i_nb]='"'+$"{name}"+'"';
			as_value[i_nb]=sdata;
			i_nb++;
		}
		foreach (var (name,value) in _dj)
		{
			as_key[i_nb]='"'+$"{name}"+'"';
			as_value[i_nb]=$"{value.ToString()}";
			i_nb++;
		}
		string s="{";
		for(int i=0;i<nb;i++)
		{
			s+= "\n" + as_key[i] + " :" + as_value[i];
			if(i<nb-1) s+= ",";
		}
		s += "\n}";
		return s;
	}
	public bool IsEmpty()
	{
		if(_ds.Count+_di.Count+_df.Count+_dj.Count+_db.Count+_dai1.Count<=0) return true;
		return false;
	}
	public void Print()
	{
		Console.WriteLine($"{ToString()}");
	}
	public int[] GetArray(string key)
	{
		if(_dai1.ContainsKey(key)) 
		{	
			int[] la;
			int li=0;
			la = new int[_dai1[key].Count];
			foreach(int v in _dai1[key])
			{
				la[li] = v;
				li++;
			}
			return la;
		}
		return new int[1];//default(int[]);
	}
	public bool GetBool(string key)
	{
		bool s = false;
		if(_db.ContainsKey(key)) s = _db[key];
		return s;
	}
	public string GetString(string key)
	{
		string s = "";
		if(_ds.ContainsKey(key)) s = _ds[key];
		return s;
	}
	public int GetInt(string key)
	{
		int o = -1;
		if(_di.ContainsKey(key)) o = _di[key];
		return o;
	}
	public float GetFloat(string key)
	{
		float o = -1;
		if(_df.ContainsKey(key)) o = _df[key];
		return o;
	}
	public JsonObject GetObject(string key)
	{
		if(_dj.ContainsKey(key)) return _dj[key];
		return new JsonObject("");
	}
	public JsonObject GetObject(int id)
	{
		if(id>-1 &&  _list_obj_key.Count>id)
		{
			string skey = _list_obj_key[id];
			if(_dj.ContainsKey(skey)) return _dj[skey];
		}
		return new JsonObject("");
	}
	public int GetNbObject()
	{
		return _list_obj_key.Count;
	}
	public void SetArray(string key,int[] value)
	{
		_dai1[key] = new List<int>();
		for(int i=0;i<value.Length;i++)
		{
			_dai1[key].Add(value[i]);
		}
	}
	public void SetObject(string key,JsonObject value)
	{
		_dj[key]=value;
	}
	public void SetBool(string key,bool value)
	{
		_db[key]=value;
	}
	public void SetString(string key,string value)
	{
		_ds[key]=value;
	}
	public void SetInt(string key,int value)
	{
		_di[key]=value;
	}
	public void SetFloat(string key,float value)
	{
		_df[key]=value;
	}
	
}
