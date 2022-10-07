using System.Dynamic;
using System.Reflection;

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
    internal object Instance;

    /// <summary>
    /// Cached type of the instance
    /// </summary>
    Type InstanceType;

    internal PropertyInfo[] InstancePropertyInfo
    {
        get
        {
            if (_InstancePropertyInfo == null && Instance != null)
                _InstancePropertyInfo = Instance.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            return _InstancePropertyInfo;
        }
    }

    PropertyInfo[] _InstancePropertyInfo;


    /// <summary>
    /// String Dictionary that contains the extra dynamic values
    /// stored on this object/instance
    /// </summary>        
    /// <remarks>Using PropertyBag to support XML Serialization of the dictionary</remarks>
    private Dictionary<string, object> _properties = new Dictionary<string, object>();

    public Dictionary<string, object> Errors { get; } = new Dictionary<string, object>();

    public bool AddError(string name, object value)
    {
        return Errors.TryAdd(name, value);
    }

    public bool HasErrors => Errors.Any();
    public Dictionary<string, object> Fields => _properties;

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
        Instance = instance;
        if (instance != null)
            InstanceType = instance.GetType();
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
        if (_properties.Keys.Contains(binder.Name))
        {
            result = _properties[binder.Name];
            return true;
        }


        // Next check for Public properties via Reflection
        if (Instance != null)
        {
            try
            {
                return GetProperty(Instance, binder.Name, out result);
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
        if (Instance != null)
        {
            try
            {
                bool result = SetProperty(Instance, binder.Name, value);
                if (result)
                    return true;
            }
            catch
            {
            }
        }

        // no match - set or add to dictionary
        _properties[binder.Name] = value;
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
        if (Instance != null)
        {
            try
            {
                // check instance passed in for methods to invoke
                if (InvokeMethod(Instance, binder.Name, args, out result))
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
            InstanceType.GetMember(name, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
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
            InstanceType.GetMember(name, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
        if (miArray != null && miArray.Length > 0)
        {
            var mi = miArray[0];
            if (mi.MemberType == MemberTypes.Property)
            {
                ((PropertyInfo) mi).SetValue(Instance, value, null);
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
        var miArray = InstanceType.GetMember(name,
            BindingFlags.InvokeMethod |
            BindingFlags.Public | BindingFlags.Instance);

        if (miArray != null && miArray.Length > 0)
        {
            var mi = miArray[0] as MethodInfo;
            result = mi.Invoke(Instance, args);
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
                return _properties[key];
            }
            catch (KeyNotFoundException ex)
            {
                // try reflection on instanceType
                object result = null;
                if (GetProperty(Instance, key, out result))
                    return result;

                // nope doesn't exist
                throw;
            }
        }
        set
        {
            if (_properties.ContainsKey(key))
            {
                _properties[key] = value;
                return;
            }

            // check instance for existance of type first
            var miArray = InstanceType.GetMember(key,
                BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            if (miArray != null && miArray.Length > 0)
                SetProperty(Instance, key, value);
            else
                _properties[key] = value;
        }
    }

    public IDictionary<string, object> AsDictionary()
    {
        var dictionary = new Dictionary<string, object>();
        foreach (var prop in InstancePropertyInfo)
            dictionary.Add(prop.Name, prop.GetValue(Instance, null));

        foreach (var key in _properties.Keys)
            dictionary.Add(key, _properties[key]);

        return dictionary;
    }

    public void SetErrors(List<KeyValuePair<string, object>> errors)
    {
        foreach (var error in errors)
            Errors.Add(error.Key, error.Value);
    }
}