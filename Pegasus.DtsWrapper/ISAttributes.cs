using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.DtsWrapper
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public sealed class ConstructorForAutomationAttribute : Attribute
    {
        //  intended for use when more than one constructors are available and one is for project and one is package.
        private bool _isForProject;

        public ConstructorForAutomationAttribute(bool isForProject = true)
        {
            _isForProject = isForProject;
        }
        public bool IsForProject { get { return _isForProject; } }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class Experimental : Attribute
    {
        //  intended for use when more than one constructors are available and one is for project and one is package.
        private string _message;

        public Experimental(string message)
        {
            Message = message;
        }
        public string Message { get { return _message; } set { _message = value; } }
    }
}
