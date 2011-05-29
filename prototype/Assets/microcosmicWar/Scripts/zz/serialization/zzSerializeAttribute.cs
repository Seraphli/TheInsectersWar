using System;

[System.AttributeUsage(System.AttributeTargets.Property)]
public class zzSerializeAttribute : Attribute
{
    public bool serializeIn = true;
    public bool serializeOut = true;
}

[System.AttributeUsage(System.AttributeTargets.Property)]
public class zzSerializeInAttribute : zzSerializeAttribute
{
    public zzSerializeInAttribute()
    {
        serializeOut = false;
    }
}

[System.AttributeUsage(System.AttributeTargets.Property)]
public class zzSerializeOutAttribute : zzSerializeAttribute
{
    public zzSerializeOutAttribute()
    {
        serializeIn = false;
    }
}