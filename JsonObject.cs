using System;
using System.IO;
using System.Collections.Generic;
public class JsonObject
{
	private Dictionary<string,string> _da = new Dictionary<string,string>(); 
	public int Size{get=>_da.Count;}
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
	private string _extract(string s,char cbegin,char cend)
	{
		int i_beg=-1;
		int i_end=-1;
		for(int i=0;i<s.Length;i++)
		{
			if(i_beg==-1 && s[i]==cbegin) i_beg = i+1;
			else if(i_beg>-1 && s[i]==cend)
			{
				i_end = i;
				break;
			}
		}
		if(i_beg+i_end>0 && i_end-i_beg <= s.Length) 
			return s.Substring(i_beg,i_end-i_beg);
		return default(string);
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
		return default(string);
	}
	public JsonObject(string sfile)
	{
		int i_begin=-1;
		int i_end=-1;
		int i_tps=-1;
		string data=""; 
		StreamReader file = File.OpenText(sfile);
        	string s = file.ReadToEnd();
		for(int i=0;i<s.Length;i++)
		{
			if(s[i]!=' ' || s[i]!='\n' || s[i]!='\b') 
			{
				data+=s[i];
			}
		}
		for(int i=0;i<data.Length;i++)
		{
			if(data[i]=='{') 
			{
				i_begin=i;
				i_tps = i+1;
			}
			else if(data[i]==',' || data[i]=='}')
			{
				string data2="";
				data2 = data.Substring(i_tps,i-i_tps);
				string name = _extract(data2,'"','"');
				string value = _extract(data2,':');
				_da[name]=value;
				i_tps = i+1;
			}
		}
	}
	public string getString(string name)
	{
		if(_da.ContainsKey(name)) return _da[name];
		return default(string);
	}
	public int getInt(string name)
	{
		int o = -1;
		string v = getString(name);
		if(v!=null)
			o = Int32.Parse(v);
		return o;
	}
	public float getFloat(string name)
	{
		float o = -1;
		string v = getString(name);
		if(v!=null)
			o = float.Parse(v);
		return o;
	}
}
