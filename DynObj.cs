using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace YFPos.Utils
{
    public delegate object DeleDynamic(dynamic sender, params object[] pramas);
    public class DynObj : DynamicObject
    {
        public Dictionary<string, object> _values;

        public DynObj()
        {
            _values = new Dictionary<string, object>();
        }

        public object GetPropertyValue(string propertyName)
        {
            if (_values.ContainsKey(propertyName) == true)
            {
                return _values[propertyName];
            }
            return null;
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            if (_values.ContainsKey(propertyName) == true)
            {
                _values[propertyName] = value;
            }
            else
            {
                _values.Add(propertyName, value);
            }

        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetPropertyValue(binder.Name);
            return result == null ? false : true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetPropertyValue(binder.Name, value);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var deleObj = GetPropertyValue(binder.Name) as DelegateObj;
            if (deleObj == null || deleObj.CallMethod == null)
            {
                result = null;
                return false;
            }
            result = deleObj.CallMethod(this, args);
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }

    }


    public class DelegateObj
    {
        private DeleDynamic _deleDynamic;

        public DeleDynamic CallMethod
        {
            get { return _deleDynamic; }
        }

        public DelegateObj(DeleDynamic deleDynamic)
        {
            this._deleDynamic = deleDynamic;
        }

        public static DelegateObj Function(DeleDynamic deleDynamic)
        {
            return new DelegateObj(deleDynamic);
        }
    }
}
