/*
MIT License JsonObject v0.2-1
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
	private bool _isVoid = false;
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
			data = _extract(data,'{','}');
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
				string name = _extract(s2,'"','"');
				string value = _extract(s2,':');
				if(value!="")
				{
					if(value[0]=='"') _addString(name,value);
					else if(value[0]=='{')
					{
						_dj[name] = new JsonObject(value);
					}
					else if(value[0]=='[')
					{

					}
					else if(value=="true" || value=="false")
					{
						bool lbool=false;
						if(value=="true") lbool=true;
						_db[name]=lbool;
					}
					else
					{
						_addInt(name,value);
						_addFloat(name,value);
					}
				}
				//Console.WriteLine($"{name} {value}...");
			}
		}
		else
		{
			_isVoid=true;
		}
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
		if(value!=null && value.Contains('.'))
		{
			for(int i=0;i<value.Length;i++)
			{
				if(value.ElementAt(i)=='.')
				{
					double o = (float)Int32.Parse(value.Substring(0,i));
					string ls = value.Substring(i+1,value.Length-(i+1));
					o += Int32.Parse(ls)/Math.Pow(10,ls.Length);
					_df[name] = (float)o;
					break;
				}
			} 
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
		string s="{\n";
		if(_isVoid) return "this JsonObject is empty";
		foreach (var (name,value) in _ds)
		{
			s = String.Concat(s,$"\t[{name}] "+'"'+$"{value}"+'"'+"\n");
		}
		foreach (var (name,value) in _di)
		{
			s = String.Concat(s,$"\t[{name}] {value}\n");
		}
		foreach (var (name,value) in _df)
		{
			s = String.Concat(s,$"\t[{name}] {value}\n");
		}
		foreach (var (name,value) in _db)
		{
			s = String.Concat(s,$"\t[{name}] {value}\n");
		}
		foreach (var (name,value) in _dj)
		{
			s = String.Concat(s,$"\t[{name}] {value.ToString()} "+"\t}\n");
		}

		return s;
	}
	public bool IsEmpty()
	{
		if(_isVoid) return true;
		return false;
	}
	public void Print()
	{
		Console.WriteLine($"{ToString()}"+"}");
	}
	public bool GetBool(string name)
	{
		bool s = false;
		if(_db.ContainsKey(name)) s = _db[name];
		return s;
	}
	public string GetString(string name)
	{
		string s = "";
		if(_ds.ContainsKey(name)) s = _ds[name];
		return s;
	}
	public int GetInt(string name)
	{
		int o = -1;
		if(_di.ContainsKey(name)) o = _di[name];
		return o;
	}
	public float GetFloat(string name)
	{
		float o = -1;
		if(_df.ContainsKey(name)) o = _df[name];
		return o;
	}
	public JsonObject GetObject(string name)
	{
		if(_dj.ContainsKey(name)) return _dj[name];
		return new JsonObject("");
	}
	
}
