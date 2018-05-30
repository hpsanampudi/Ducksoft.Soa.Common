using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Ducksoft.SOA.Common.Utilities
{
    /// <summary>
    /// Base class for all the data which we will read/write to XML using xpath queries
    /// </summary>
    public class XmlParameters
    {
        /// <summary>
        ///  Creating variable for Xml document
        /// </summary>
        private XmlDocument document;

        /// <summary>
        /// Constant string variable which represents "ENTRY" element in Xml file.
        /// </summary>
        private const string m_ENTRY = "Entry";

        /// <summary>
        /// Gets the XML file path from where it is loaded.
        /// </summary>
        public string XmlFilePath { get; private set; }

        /// <summary>
        /// Returns the complete xml string of the document
        /// </summary>
        public string XmlData
        {
            get
            {
                return (document.InnerXml);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlParameters"/> class.
        /// </summary>
        public XmlParameters()
        {
        }

        /// <summary>
        /// Removes all the parameters/sub parameters which fall under the given xPath query.
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        public void RemoveParameters(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);
            XmlNode CurrentParent = document.DocumentElement;
            XmlNodeList SearchNode = CurrentParent.SelectNodes(xPath);
            if (null != SearchNode)
            {
                foreach (XmlNode paramNode in SearchNode)
                {
                    paramNode.ParentNode.RemoveChild(paramNode);
                }
            }
        }

        /// <summary>
        /// Creates entire XML node structure according to xPath.
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        private XmlNode CreateNodesUsingXPath(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);
            xPath = xPath.Trim('/');
            string CurrentPath = string.Empty;
            XmlNode CurrentParent = document.DocumentElement;
            while (xPath.Length > 0)
            {
                string nodeName = string.Empty;
                if (xPath.IndexOf('/') > 0)
                {
                    nodeName = xPath.Substring(0, xPath.IndexOf('/'));
                }
                else
                {
                    nodeName = xPath;
                    xPath = string.Empty;
                }

                xPath = xPath.Substring(xPath.IndexOf('/') + 1);
                XmlNode SearchNode = CurrentParent.SelectSingleNode(CurrentPath + "/" + nodeName);
                CurrentPath += "/" + nodeName;
                if (null == SearchNode)
                {
                    XmlNode CurrentNode = null;
                    //For nodes with same name and multiple accuraance/ different data set
                    if (nodeName.IndexOf('[') > 0)
                    {
                        CurrentNode = document.CreateElement
                            (nodeName.Substring(0, nodeName.IndexOf('[')));
                    }
                    else
                    {
                        CurrentNode = document.CreateElement(nodeName);
                    }
                    ErrorBase.Require(null != CurrentNode);
                    CurrentParent.AppendChild(CurrentNode);
                    SearchNode = CurrentNode;

                }

                CurrentParent = SearchNode;
            }
            return CurrentParent;
        }

        /// <summary>
        /// Gets the value of the single parameter
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual object Query(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            ErrorBase.Require(null != Search);
            if (null != Search)
            {
                return (Search.InnerXml.ToString());
            }
            return (null);
        }

        /// <summary>
        /// Checks for the presence of the node belonging to given XPath
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual bool QueryExists(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            return (null != Search);
        }

        /// <summary>
        /// Gets all the bytes stored in a tag
        /// </summary>
        public virtual byte[] QueryBytes(string xPathQuerry)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPathQuerry));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPathQuerry);
            if (null != Search)
            {
                return (Convert.FromBase64String(Search.InnerXml.ToString().Trim()));
            }
            else
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPathQuerry);
            }
        }

        /// <summary>
        /// Gets the value of the single parameter
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual string QueryText(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null != Search)
            {
                return (Search.InnerText.ToString().Trim());
            }
            else
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
        }

        /// <summary>
        /// Gets the Node Values for the given xPath
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual XmlNode QueryNode(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null != Search)
            {
                return (Search);
            }
            else
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
        }

        /// <summary>
        /// Gets the First child nodes Values for the given xPath
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual XmlNodeList QueryNodeList(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNodeList Search = document.DocumentElement.SelectNodes(xPath);
            if (null != Search)
            {
                return (Search);
            }
            else
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
        }

        /// <summary>
        /// Gets the First child nodes count for the given path
        /// </summary>
        /// <param name="xPath">The x path.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public virtual int QueryChildCount(string xPath, int defaultValue)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            int childCount = defaultValue;
            XmlNodeList Search = document.DocumentElement.SelectNodes(xPath);
            if (null != Search)
            {
                childCount = Search.Count;
            }
            else
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }

            return (childCount);
        }

        /// <summary>
        /// Gets the name of the node belongs to given xPath
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual string QueryNodeName(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null != Search)
            {
                return (Search.Name.Trim());
            }
            else
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
        }

        /// <summary>
        /// Gets the value of the single parameter
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual string QueryString(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null != Search)
            {
                return (Search.InnerXml.ToString().Trim());
            }
            else
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
        }

        /// <summary>
        /// Gets the value of the single parameter. If not found assigns the default value.
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public virtual string QueryString(string xPath, string defaultValue)
        {
            string data = defaultValue;
            try
            {
                data = QueryString(xPath);
            }
            catch (XmlParamNotFoundException)
            {
                data = defaultValue;
            }
            return data;
        }

        /// <summary>
        /// Gets the value of the single parameter
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual int QueryInt32(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            //ErrorBase.Require(null != Search);
            if (null != Search)
            {
                return ((int)Convert.ToDouble(Search.InnerXml.ToString(),
                    CultureInfo.CurrentUICulture));
            }
            else if (null == Search)
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
            return 0;
        }

        /// <summary>
        /// Gets the value of the single parameter. If not found assigns the default value
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public virtual Int32 QueryInt32(string xPath, Int32 defaultValue)
        {
            int data = defaultValue;
            try
            {
                data = QueryInt32(xPath);
            }
            catch (XmlParamNotFoundException)
            {
                data = defaultValue;
            }
            return data;
        }

        /// <summary>
        /// Gets the value of the single parameter
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual bool Querybool(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);
            bool data = false;

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            //ErrorBase.Require(null != Search);
            if (null != Search)
            {
                string booleanStatus = Search.InnerXml.ToString().Trim();
                if (booleanStatus == "1")
                {
                    data = true;
                }
                else if (booleanStatus == "0")
                {
                    data = false;
                }

                else
                {
                    data = Convert.ToBoolean(Search.InnerXml.ToString().Trim(),
                        CultureInfo.CurrentUICulture);
                }
                return (data);
            }
            else if (null == Search)
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
            return data;
        }


        /// <summary>
        /// Gets the value of the single parameter. If not found assigns the default value
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        public virtual bool Querybool(string xPath, bool defaultValue)
        {
            bool data = defaultValue;
            try
            {
                data = Querybool(xPath);
            }
            catch (XmlParamNotFoundException)
            {
                data = defaultValue;
            }
            return data;
        }

        /// <summary>
        /// Gets the value of the single parameter
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual double QueryDouble(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            //ErrorBase.Require(null != Search);
            if (null != Search)
            {
                return (Convert.ToDouble(Search.InnerXml.ToString(), CultureInfo.CurrentUICulture));
            }
            else if (null == Search)
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }

            return (0.0D);
        }

        /// <summary>
        /// Gets the value of the single parameter. If not found assigns the default value
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public virtual double QueryDouble(string xPath, double defaultValue)
        {
            double data = defaultValue;
            try
            {
                data = QueryDouble(xPath);
            }
            catch (XmlParamNotFoundException)
            {
                data = defaultValue;
            }

            return data;
        }
        /// <summary>
        /// Gets the value of the single parameter
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual float Queryfloat(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            //ErrorBase.Require(null != Search);
            if (null != Search)
            {
                return ((float)Convert.ToDouble(Search.InnerXml.ToString(),
                    CultureInfo.CurrentUICulture));
            }
            else if (null == Search)
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
            return 0.00F;
        }
        /// <summary>
        /// Gets the value of the single parameter
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <returns></returns>
        public virtual decimal QueryDecimal(string xPath)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            ErrorBase.Require(null != Search);
            if (null != Search)
            {
                return (Convert.ToDecimal(Search.InnerXml.ToString(), CultureInfo.CurrentUICulture));
            }
            return 0.0M;
        }

        /// <summary>
        /// Loads the XML data into memory by initializing XMLDocument object
        /// </summary>
        /// <param name="fileName">XML filename with complete path</param>
        public virtual void LoadXml(string fileName)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(fileName));
            document = new XmlDocument();
            document.Load(fileName);
            XmlFilePath = fileName;
            ErrorBase.Require(null != document);
        }

        /// <summary>
        /// Loads the XML data into memory by initializing XMLDocument object
        /// </summary>
        /// <param name="xmlStr">The XML string.</param>
        public virtual void LoadXmlString(string xmlStr)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xmlStr));
            //ErrorBase.Require(null == document);//We are using a fresh object
            XmlFilePath = null;
            document = new XmlDocument();

            if (0 == xmlStr.Trim().IndexOf("<?xml"))
            {
                document.LoadXml(xmlStr);
            }
            else
            {
                document.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + xmlStr);
            }

            ErrorBase.Require(null != document);
        }

        /// <summary>
        /// Gets the values by refering to an array variable
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="values">The values.</param>
        public virtual void Query(string xPath, IList<int> values)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != values);
            ErrorBase.Require(null != document);

            values.Clear();
            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
            else
            {
                for (int i = 0; i < Search.ChildNodes.Count; i++)
                {
                    values.Add(Convert.ToInt32(Search.ChildNodes[i].InnerText.ToString(),
                        CultureInfo.CurrentUICulture));
                }
            }
        }

        /// <summary>
        /// Gets the values by refering to an array variable
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="values">The values.</param>
        public virtual void Query(string xPath, IList<double> values)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != values);
            ErrorBase.Require(null != document);

            values.Clear();
            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
            else
            {
                for (int i = 0; i < Search.ChildNodes.Count; i++)
                {
                    values.Add(Convert.ToDouble(Search.ChildNodes[i].InnerText.ToString(),
                        CultureInfo.CurrentUICulture));
                }
            }
        }

        /// <summary>
        /// Gets the values by refering to an array variable
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="values">The values.</param>
        public virtual void Query(string xPath, IList<string> values)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != values);
            ErrorBase.Require(null != document);

            values.Clear();
            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
            else
            {
                for (int i = 0; i < Search.ChildNodes.Count; i++)
                {
                    if (XmlNodeType.Element == Search.ChildNodes[i].NodeType)
                    {
                        //Adds only xml nodes having data. I.e., it eliminates commented nodes
                        values.Add(Search.ChildNodes[i].InnerText.Trim());
                    }
                }
            }
        }

        /// <summary>
        /// Gets the values by refering to an array variable along the commented nodes
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="values">The values.</param>
        public virtual void QueryTextWithComments(string xPath, IList<string> values)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != values);
            ErrorBase.Require(null != document);

            values.Clear();
            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                throw new XmlParamNotFoundException("Parameter not found for " + xPath);
            }
            else
            {
                for (int i = 0; i < Search.ChildNodes.Count; i++)
                {
                    if (XmlNodeType.Element == Search.ChildNodes[i].NodeType)
                    {
                        //Adds xml nodes having inner text
                        values.Add(Search.ChildNodes[i].InnerText.Trim());
                    }
                    else if (XmlNodeType.Comment == Search.ChildNodes[i].NodeType)
                    {
                        //Adds xml nodes having commented text.                        
                        string comment = Search.ChildNodes[i].InnerText.Trim();
                        //Get back custom tag(&dh;) actual value('-')
                        comment = comment.Replace("&dh;", "-");
                        //During serialization, If user enters our custom tag(&dh;) inside the textbox, it converts as &dh;dh;
                        //So, after the execution of the above line this data(&dh;dh;) gets changes to "-dh;"
                        ////Therefore "-dh;" is &dh;(actual value)
                        comment = comment.Replace("-dh;", "&dh;");

                        //To differentiate the commented data add "%" symbol before it
                        values.Add("% " + comment);
                    }
                }
            }
        }

        /// <summary>
        /// writes given bytes in the XML refered by XPath Querry
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="value">The value.</param>
        public virtual void Write(string xPath, byte[] value)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != value);
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                //If this node is not present create it
                Search = CreateNodesUsingXPath(xPath);
            }
            ErrorBase.Require(null != Search);
            Search.InnerText = Convert.ToBase64String(value);
        }

        /// <summary>
        /// Updates a value in the XML referred by XPath Query
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public virtual void Write(string xPath, bool value)
        {
            Write(xPath, Convert.ToInt32(value));
        }

        /// <summary>
        /// Updates a value in the XML referred by XPath Query
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="value">The object value.</param>
        public virtual void Write(string xPath, object value)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != value);
            ErrorBase.Require(null != document);
            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                //Create all the nodes in the XPath if not present
                Search = CreateNodesUsingXPath(xPath);
            }
            ErrorBase.Require(null != Search);

            //If any special characters present it converts like below
            // &(And) as &amp;
            // <(LessThan) as &lt;
            // >(GreaterThan) as &gt;
            Search.InnerText = value.ToString();
        }

        /// <summary>
        /// Updates a values of an array variable in the XML refered by XPath Query
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="values">The list if string values.</param>
        public virtual void Write(string xPath, IList<string> values)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != values);
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                Search = CreateNodesUsingXPath(xPath);
            }

            ErrorBase.Require(null != Search);
            XmlNodeList SelectedNode = Search.SelectNodes(m_ENTRY);
            foreach (XmlNode node in SelectedNode)
            {
                node.ParentNode.RemoveChild(node);
            }
            for (int i = 0; i < values.Count; i++)
            {
                XmlElement newElement = document.CreateElement(m_ENTRY);
                newElement.InnerText = values[i];
                Search.AppendChild(newElement);
            }
        }

        /// <summary>
        /// Updates a values of an array variable along comments in the XML refered by XPath Querry
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="values">The values.</param>
        public virtual void WriteTextWithComments(string xPath, IList<string> values)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != values);
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                Search = CreateNodesUsingXPath(xPath);
            }

            ErrorBase.Require(null != Search);
            XmlNodeList SelectedNode = Search.SelectNodes(m_ENTRY);
            foreach (XmlNode node in SelectedNode)
            {
                node.ParentNode.RemoveChild(node);
            }
            for (int i = 0; i < values.Count; i++)
            {
                //Differentiating the commented text based on "%" symbol
                if (0 == values[i].IndexOf('%'))
                {
                    //Write text as comment
                    string commentStr = values[i].TrimStart('%').Trim();

                    //An XML comment cannot contain '--', and '-' cannot be the last character.
                    //To handle this convert '-' as &dh;
                    //If user enters our custom tag(&dh;) inside the textbox then convert it as &dh;dh;
                    commentStr = commentStr.Replace("&dh;", "&dh;dh;");
                    commentStr = commentStr.Replace("-", "&dh;");
                    XmlComment comment = document.CreateComment(commentStr);
                    Search.AppendChild(comment);
                }
                else
                {
                    //Write text as inner text to xml node "<ENTRY />"
                    XmlElement newElement = document.CreateElement(m_ENTRY);
                    newElement.InnerText = values[i];
                    Search.AppendChild(newElement);
                }
            }
        }

        /// <summary>
        /// Updates a values of an array variable in the XML refered by XPath Querry
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="values">The values.</param>
        public virtual void Write(string xPath, IList<double> values)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != values && 0 != values.Count);
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                Search = CreateNodesUsingXPath(xPath);
            }

            ErrorBase.Require(null != Search);
            XmlNodeList SelectedNode = Search.SelectNodes(m_ENTRY);
            foreach (XmlNode node in SelectedNode)
            {
                node.ParentNode.RemoveChild(node);
            }
            for (int i = 0; i < values.Count; i++)
            {
                XmlElement newElement = document.CreateElement(m_ENTRY);
                newElement.InnerText = values[i].ToString(CultureInfo.CurrentUICulture);
                Search.AppendChild(newElement);
            }
        }

        /// <summary>
        /// Updates a values of an array variable in the XML refered by XPath Querry
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="values">The values.</param>
        public virtual void Write(string xPath, IList<int> values)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(null != values && 0 != values.Count);
            ErrorBase.Require(null != document);
            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                Search = CreateNodesUsingXPath(xPath);
            }

            ErrorBase.Require(null != Search);
            XmlNodeList SelectedNode = Search.SelectNodes(m_ENTRY);
            foreach (XmlNode node in SelectedNode)
            {
                node.ParentNode.RemoveChild(node);
            }
            for (int i = 0; i < values.Count; i++)
            {
                XmlElement newElement = document.CreateElement(m_ENTRY);
                newElement.InnerText = values[i].ToString(CultureInfo.CurrentUICulture);
                Search.AppendChild(newElement);
            }
        }

        /// <summary>
        /// Updates the Attribute in the XML with reference to the path, and attribute node
        /// </summary>
        /// <param name="xPath">The xpath query.</param>
        /// <param name="attrName">The attributenode.</param>
        /// <param name="attrValue">The attributedata.</param>
        /// <param name="value">The value.</param>
        public virtual void WriteAttribute(string xPath, string attrName, object attrValue,
            object value)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(xPath));
            ErrorBase.Require(!string.IsNullOrEmpty(attrName));
            ErrorBase.Require(null != attrValue);
            ErrorBase.Require(null != value);
            ErrorBase.Require(null != document);

            XmlNode Search = document.DocumentElement.SelectSingleNode(xPath);
            if (null == Search)
            {
                Search = CreateNodesUsingXPath(xPath);
            }
            XmlAttribute attribute = document.CreateAttribute(attrName);
            Search.Attributes.Append(attribute);
            attribute.Value = attrValue.ToString();

            ErrorBase.Require(null != Search);
            Search.InnerText = value.ToString();
        }

        /// <summary>
        /// Updates the Attribute in the XML with reference to the path
        /// </summary>
        /// <param name="attrXPath">The attribute Xpath.</param>
        /// <param name="attrValue">The attribute value.</param>
        public virtual void WriteAttribute(string attrXPath, object attrValue)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(attrXPath));
            ErrorBase.Require(null != attrValue);
            ErrorBase.Require(null != document);

            string attrIndentifier = "//@";
            int attrXPathIndex = attrXPath.LastIndexOf(attrIndentifier);
            string nodeXPath = attrXPath.Substring(0, attrXPathIndex);
            string attrName = attrXPath.Substring(attrXPathIndex + attrIndentifier.Length);

            XmlNode Search = document.DocumentElement.SelectSingleNode(nodeXPath);
            if (null == Search)
            {
                Search = CreateNodesUsingXPath(nodeXPath);
            }
            XmlAttribute attribute = document.CreateAttribute(attrName);
            Search.Attributes.Append(attribute);
            attribute.Value = attrValue.ToString();
        }

        /// <summary>
        /// Saves the XML data with the given Filename
        /// </summary>
        /// <param name="fileName">XML File name with complete path</param>
        public void SaveXml(string fileName)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(fileName));
            ErrorBase.Require(null != document);
            //If  a '.' is provided in the file name then the rest of the string after '.' is read as File Extension. 
            //and doesn`t allow the file to be saved as an XML File. So Concatenate ".xml" to the File Name.
            if (0 != string.Compare(Path.GetExtension(fileName), ".xml", true,
                CultureInfo.CurrentUICulture))
            {
                fileName += ".xml";
            }

            document.Save(fileName);
        }

    }
}
