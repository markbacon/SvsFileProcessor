using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SvsFileProcessor {
	class SearchPatternManifest {

		public SearchPatternManifest() {

            string xml = LoadXml();

            _xmlDoc = new XmlDocument();
            _xmlDoc.LoadXml(xml);
		}
        //---------------------------------------------------------------------------------------------------------
        public List<FileSearchPattern> GetSearchPatternList(FileType fileType, Concept concept) {

            List<FileSearchPattern> searchPatternList = new List<FileSearchPattern>();

            
            string xpath = "//SearchPatterns/PatternGroup[@FileType='" + fileType.ToString() + "']/SearchPattern[@Concept='" + concept.ToString() + "']";

            XmlNodeList nodes = _xmlDoc.SelectNodes(xpath);

            foreach (XmlElement element in nodes) {

                FileSearchPattern fileSearchPattern = new FileSearchPattern() {
                    //AddDateToFileName = Convert.ToBoolean
                    Concept = concept,
                    FileType = fileType,
                    SearchPattern = element.GetAttribute("Value")
                };

                string temp = element.GetAttribute("AddDateToFileName");

                if (!string.IsNullOrEmpty(temp)) {
                    fileSearchPattern.AddDateToFileName = Convert.ToBoolean(temp);
                }

                searchPatternList.Add(fileSearchPattern);

            }

            return searchPatternList;
        }
        //---------------------------------------------------------------------------------------------------------
        //-- Private
        //---------------------------------------------------------------------------------------------------------
        private XmlDocument _xmlDoc;

        private string LoadXml() {

            string xml = "";

            System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();

            Stream s = thisExe.GetManifestResourceStream("SvsFileProcessor.XML.SvsFileSearchPatterns.xml");

            using (StreamReader sr = new StreamReader(s)) {
                xml = sr.ReadToEnd();
            }

            return xml;
        }
    }
}
