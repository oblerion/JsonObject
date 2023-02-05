using System;
using System.IO;
using System.Collections.Generic;
public class JsonObject
{
	private Dictionary<string,string> _da = new Dictionary<string,string>(); 
	public int Size{get=>_da.Count;}
	public JsonObject(string sfile)
	{
		bool in_obj = false;
		bool in_array = false;
		List<string> ldata = new List<string>();
		int i_tps=0;
		StreamReader file = File.OpenText(sfile);
        string data = file.ReadToEnd();
		data = _filter(data);
		data = _extract(data,'{','}');
		Console.WriteLine($"{data}");
		for(int i=0;i<data.Length;i++)
		{
			if(i==data.Length-1)
			{
				ldata.Add(data.Substring(i_tps,i-i_tps+1));
			}
			else if(data[i]=='[')
			{
				in_array=true;
			}
			else if(data[i]=='{')
			{
				in_obj=true;
			}
			else if(data[i]==']')
			{
				in_array=false;
			}
			else if(data[i]=='}')
			{
				in_obj=false;
			}		
			else if(data[i]==',' && !in_array && !in_obj)
			{
				ldata.Add(data.Substring(i_tps,i-i_tps));
				i_tps = i+1;
			}
		}
		foreach(string s2 in ldata)
		{
		 	string name = _extract(s2,'"','"');
		 	string value = _extract(s2,':');
			if(value!="")
			{
				if(value[0]=='"') value = _extract(value,'"','"');
			}
		 	_da[name]=value;
			Console.WriteLine($"{name} {value}...");
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
	public void print()
	{
		for(int i=0;i<_da.Count;i++)
		{
			Console.WriteLine(_da.ElementAt(i));
		}
	}
	public string getString(string name)
	{
		string ls = "";
		if(_da.ContainsKey(name)) ls = _da[name];
		return ls;
	}
	public int getInt(string name)
	{
		int o = -1;
		Console.WriteLine("getInt "+name);
		string v = getString(name);
		if(v!=null)
			o = Int32.Parse(v);
		return o;
	}
	public float getFloat(string name)
	{
		double o = -1;
		string v = getString(name);
		if(v!=null && v.Contains('.'))
		{
			for(int i=0;i<v.Length;i++)
			{
				if(v.ElementAt(i)=='.')
				{
					o = (float)Int32.Parse(v.Substring(0,i));
					string ls = v.Substring(i+1,v.Length-(i+1));
					o += Int32.Parse(ls)/Math.Pow(10,ls.Length);
					break;
				}
			}
			
			//o = float.Parse(v);
		}
		return (float)o;
	}
	
}
