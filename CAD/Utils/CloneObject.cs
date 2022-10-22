using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace CAD.Utils
{
    public static class ReflectionExtensions
    {
        public static IList<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
        {
            if (type == typeof(Object)) return new List<FieldInfo>();

            var list = type.BaseType.GetAllFields(flags);
            // in order to avoid duplicates, force BindingFlags.DeclaredOnly
            FieldInfo[] fields = type.GetFields(flags | BindingFlags.DeclaredOnly);
            foreach (var fi in fields)
                list.Insert(list.Count, fi);
            return list;
        }
    }
    /// <summary>
    /// <b>BaseObject</b> class is an abstract class for you to derive from. <br>
    /// Every class that will be dirived from this class will support the <b>Clone</b> method automaticly.<br>
    /// The class implements the interface <i>ICloneable</i> and there for every object that will be derived <br>
    /// from this object will support the <i>ICloneable</i> interface as well.
    /// </summary>
    [Serializable]
    public abstract class BaseObject : ICloneable, INotifyPropertyChanged
    {
        #region ICloneable
        /// <summary>
        /// Clone the object, and returning a reference to a cloned object.
        /// </summary>
        /// <returns>Reference to the new cloned object.</returns>
        public object Clone()
        {
            //First we create an instance of this specific type.
            object newObject = Activator.CreateInstance(this.GetType());

            //We get the array of fields for the new type instance.
            //            FieldInfo[] fields = newObject.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var fields = ReflectionExtensions.GetAllFields(newObject.GetType(), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            int i = 0;

            foreach (FieldInfo fi in ReflectionExtensions.GetAllFields(this.GetType(), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))
            {
                //We query if the fiels support the ICloneable interface.
                Type ICloneType = fi.FieldType.GetInterface("ICloneable", true);

                if (ICloneType != null)
                {
                    //Getting the ICloneable interface from the object.
                    ICloneable IClone = (ICloneable)fi.GetValue(this);

                    //We use the clone method to set the new value to the field.
                    if (IClone != null)
                        fields[i].SetValue(newObject, IClone.Clone());
                    else
                    {
                        //If the field doesn't support the ICloneable interface then just set it.
                        fields[i].SetValue(newObject, fi.GetValue(this));
                    }
                }
                else
                {
                    //If the field doesn't support the ICloneable interface then just set it.
                    fields[i].SetValue(newObject, fi.GetValue(this));
                }

                //Now we check if the object support the IEnumerable interface, so if it does
                //we need to enumerate all its items and check if they support the ICloneable interface.
                Type IEnumerableType = fi.FieldType.GetInterface("IEnumerable", true);
                if (IEnumerableType != null)
                {
                    //Get the IEnumerable interface from the field.
                    IEnumerable IEnum = (IEnumerable)fi.GetValue(this);

                    //This version support the IList and the IDictionary interfaces to iterate
                    //on collections.
                    Type IListType = fields[i].FieldType.GetInterface("IList", true);
                    Type IDicType = fields[i].FieldType.GetInterface("IDictionary", true);

                    int j = 0;
                    if (IListType != null)
                    {
                        //Getting the IList interface.
                        IList list = (IList)fields[i].GetValue(newObject);

                        foreach (object obj in IEnum)
                        {
                            //Checking to see if the current item support the ICloneable interface.
                            ICloneType = obj.GetType().GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                //If it does support the ICloneable interface, we use it to set the clone of
                                //the object in the list.
                                ICloneable clone = (ICloneable)obj;

                                list[j] = clone.Clone();
                            }

                            //NOTE: If the item in the list is not support the ICloneable interface then
                            // in the cloned list this item will be the same item as in the original list
                            //(as long as this type is a reference type).

                            j++;
                        }
                    }
                    else if (IDicType != null)
                    {
                        //Getting the dictionary interface.
                        IDictionary dic = (IDictionary)fields[i].GetValue(newObject);
                        j = 0;
                        foreach (DictionaryEntry de in IEnum)
                        {
                            //Checking to see if the item support the ICloneable interface.
                            ICloneType = de.Value.GetType().GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                ICloneable clone = (ICloneable)de.Value;

                                dic[de.Key] = clone.Clone();
                            }
                            j++;
                        }
                    }
                }
                i++;
            }
            return newObject;
        }
        #endregion
        #region INotifyPropertyChanged
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
