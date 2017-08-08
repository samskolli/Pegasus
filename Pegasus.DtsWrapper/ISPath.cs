using System.Linq;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISPath
    {
        #region Object Model Properties

        #region IDTSPath100

        internal IDTSPath100 Path { get; set; }

        #endregion

        #region Description

        public string Description
        {
            get { return Path.Description; }
            set { Path.Description = value; }
        }

        #endregion

        #region ID

        public int ID
        {
            get { return Path.ID; }
            set { Path.ID = value; }
        }

        #endregion

        #region IdentificationString

        public string IdentificationString
        {
            get { return Path.IdentificationString; }
        }

        #endregion

        #region Name

        public string Name
        {
            get { return Path.Name; }
            set { Path.Name = value; }
        }

        #endregion

        #region Visualized

        public bool Visualized
        {
            set { Path.Visualized = value; }
        }

        #endregion

        #region StartPoint

        internal IDTSOutput100 StartPoint_m
        {
            get { return Path.StartPoint; }
            set { Path.StartPoint = value; }
        }

        public string StartPoint
        {
            get { return StartPoint_m.Name; }
            set { StartPoint_m.Name = value; }
        }

        #endregion

        #endregion

        #region Get IDTSOutput100

        //internal static IDTSOutput100 GetInputFromLineage(string lineage, ISProject project, ISPackage package)
        //{
        //    // lineage = ..|DataFlowTask|ComponentName|InputName
        //    string[] lineageArray = lineage.Split('|');
        //    string dftLineage = lineageArray.Take(lineageArray.Length - 3).Aggregate((a, b) => a + "|" + b);
        //    string componentName = lineageArray[lineageArray.Length - 2];
        //    string inputName = lineageArray[lineageArray.Length - 1];
        //    ISOutput o = new ISOutput(inputName, componentName, dftLineage, project, package);
        //    return o.Output;
        //}

        #endregion
    }
}
