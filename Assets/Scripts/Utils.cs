using UnityEngine;

public class MissingChildException : System.Exception
{
    public MissingChildException(GameObject gameObject, string childName) : base($"The child {childName} was not found on gameobject {gameObject.name}.") { }
}
public static class Utils
{
    public static Transform GetChildByName(this Transform transform, string name, bool throwException = true)
    {
        foreach (Transform t in transform)
        {
            if (t.name == name)
                return t;
        }
        if (throwException) throw new MissingChildException(transform.gameObject, name);
        return null;
    }
}
