using System.Dynamic;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ApiCompositor.Contracts;

/// <summary>
/// Class that provides extensible properties and methods. This
/// dynamic object stores 'extra' properties in a dictionary or
/// checks the actual properties of the instance.
/// 
/// This means you can subclass this expando and retrieve either
/// native properties or properties from values in the dictionary.
/// 
/// This type allows you three ways to access its properties:
/// 
/// Directly: any explicitly declared properties are accessible
/// Dynamic: dynamic cast allows access to dictionary and native properties/methods
/// Dictionary: Any of the extended properties are accessible via IDictionary interface
/// </summary>
[Serializable]
public class ComposedResult : DynamicObject
{
    /// <summary>
    /// Instance of object passed in
    /// </summary>
    internal object _instance;

    /// <summary>
    /// Cached type of the instance
    /// </summary>
    internal Type _instanceType;

    internal PropertyInfo[] InstancePropertyInfo
    {
        get
        {
            if (_instancePropertyInfo == null && _instance != null)
                _instancePropertyInfo = _instance.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            return _instancePropertyInfo;
        }
    }

    private PropertyInfo[] _instancePropertyInfo;


    /// <summary>
    /// String Dictionary that contains the extra dynamic values
    /// stored on this object/instance
    /// </summary>        
    /// <remarks>Using PropertyBag to support XML Serialization of the dictionary</remarks>
    
    [JsonIgnore]
    public Dictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();
    
    [JsonPropertyName("_errors")]
    public List<Error>? Errors { get; private set; } = null;

    public void AddError(string name, string message, Exception? exception = null)
    {
        Errors ??= new List<Error>();
        Errors.Add(new Error(name, message, exception));
    }

    [JsonIgnore]
    public bool HasErrors => Errors != null && Errors.Any();
    
    [JsonExtensionData]
    public IDictionary<string, object> Result => AsDictionary(true);

    /// <summary>
    /// This constructor just works off the internal dictionary and any 
    /// public properties of this object.
    /// 
    /// Note you can subclass Expando.
    /// </summary>
    public ComposedResult()
    {
        Initialize(this);
    }

    /// <summary>
    /// Allows passing in an existing instance variable to 'extend'.        
    /// </summary>
    /// <remarks>
    /// You can pass in null here if you don't want to 
    /// check native properties and only check the Dictionary!
    /// </remarks>
    /// <param name="instance"></param>
    public ComposedResult(object instance)
    {
        Initialize(instance);
    }


    protected virtual void Initialize(object instance)
    {
        _instance = instance;
        if (instance != null)
            _instanceType = instance.GetType();
    }


    /// <summary>
    /// Try to retrieve a member by name first from instance properties
    /// followed by the collection entries.
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        result = null;

        // first check the Properties collection for member
        if (Properties.Keys.Contains(binder.Name))
        {
            result = Properties[binder.Name];
            return true;
        }


        // Next check for Public properties via Reflection
        if (_instance != null)
        {
            try
            {
                return GetProperty(_instance, binder.Name, out result);
            }
            catch
            {
            }
        }

        // failed to retrieve a property
        result = null;
        return false;
    }


    /// <summary>
    /// Property setter implementation tries to retrieve value from instance 
    /// first then into this object
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        // first check to see if there's a native property to set
        if (_instance != null)
        {
            try
            {
                bool result = SetProperty(_instance, binder.Name, value);
                if (result)
                    return true;
            }
            catch
            {
            }
        }

        // no match - set or add to dictionary
        Properties[binder.Name] = value;
        return true;
    }

    /// <summary>
    /// Dynamic invocation method. Currently allows only for Reflection based
    /// operation (no ability to add methods dynamically).
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="args"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
        if (_instance != null)
        {
            try
            {
                // check instance passed in for methods to invoke
                if (InvokeMethod(_instance, binder.Name, args, out result))
                    return true;
            }
            catch
            {
            }
        }

        result = null;
        return false;
    }


    /// <summary>
    /// Reflection Helper method to retrieve a property
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    protected bool GetProperty(object instance, string name, out object result)
    {
        if (instance == null)
            instance = this;

        var miArray =
            _instanceType.GetMember(name, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
        if (miArray != null && miArray.Length > 0)
        {
            var mi = miArray[0];
            if (mi.MemberType == MemberTypes.Property)
            {
                result = ((PropertyInfo) mi).GetValue(instance, null);
                return true;
            }
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Reflection helper method to set a property value
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected bool SetProperty(object instance, string name, object value)
    {
        if (instance == null)
            instance = this;

        var miArray =
            _instanceType.GetMember(name, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
        if (miArray != null && miArray.Length > 0)
        {
            var mi = miArray[0];
            if (mi.MemberType == MemberTypes.Property)
            {
                ((PropertyInfo) mi).SetValue(_instance, value, null);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Reflection helper method to invoke a method
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    protected bool InvokeMethod(object instance, string name, object[] args, out object result)
    {
        if (instance == null)
            instance = this;

        // Look at the instanceType
        var miArray = _instanceType.GetMember(name,
            BindingFlags.InvokeMethod |
            BindingFlags.Public | BindingFlags.Instance);

        if (miArray != null && miArray.Length > 0)
        {
            var mi = miArray[0] as MethodInfo;
            result = mi.Invoke(_instance, args);
            return true;
        }

        result = null;
        return false;
    }


    /// <summary>
    /// Convenience method that provides a string Indexer 
    /// to the Properties collection AND the strongly typed
    /// properties of the object by name.
    /// 
    /// // dynamic
    /// exp["Address"] = "112 nowhere lane"; 
    /// // strong
    /// var name = exp["StronglyTypedProperty"] as string; 
    /// </summary>
    /// <remarks>
    /// The getter checks the Properties dictionary first
    /// then looks in PropertyInfo for properties.
    /// The setter checks the instance properties before
    /// checking the Properties dictionary.
    /// </remarks>
    /// <param name="key"></param>
    /// 
    /// <returns></returns>
    public object this[string key]
    {
        get
        {
            try
            {
                // try to get from properties collection first
                return Properties[key];
            }
            catch (KeyNotFoundException ex)
            {
                // try reflection on instanceType
                object result = null;
                if (GetProperty(_instance, key, out result))
                    return result;

                // nope doesn't exist
                throw;
            }
        }
        set
        {
            if (Properties.ContainsKey(key))
            {
                Properties[key] = value;
                return;
            }

            // check instance for existance of type first
            var miArray = _instanceType.GetMember(key,
                BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            if (miArray != null && miArray.Length > 0)
                SetProperty(_instance, key, value);
            else
                Properties[key] = value;
        }
    }

    public IDictionary<string, object> AsDictionary(bool serializeJson = false)
    {
        var dictionary = new Dictionary<string, object>();
        if (_instanceType != typeof(ComposedResult))
        {
            foreach (var prop in InstancePropertyInfo)
            {
                var value = prop.GetValue(_instance, null);
                if(!serializeJson)
                    dictionary.TryAdd(prop.Name, value);
                else if (value != null)
                    dictionary.TryAdd(System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(prop.Name), value);
            }
        }

        foreach (var key in Properties.Keys)
        {
            if(!serializeJson)
                dictionary.TryAdd(key, Properties[key]);
            else if(Properties[key] != null)
                dictionary.TryAdd(System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(key), Properties[key]);
        }

        return dictionary;
    }

    public void SetErrors(List<Error> errors)
    {
        Errors ??= new List<Error>();
        Errors.AddRange(errors);
    }
}

public class ComposedResult<T> : ComposedResult
{
    public ComposedResult()
    {
        Initialize(Activator.CreateInstance<T>());
    }

    [JsonIgnore]
    public T Instance => (T) _instance;
}

public record Error(string Source, string Message, Exception? Exception = null)
{
    public Exception? Exception { get; init; } = Exception;
}