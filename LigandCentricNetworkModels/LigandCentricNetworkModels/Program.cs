using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using chemaxon.descriptors;
using chemaxon.formats;

namespace LigandCentricNetworkModels
{
    class Program
    {
        static void Main(string[] args)
        {


            //compileTexts();

            string filenamePDB = "Text/PDB_complete.txt";  // PDB_3526 PDB_34164 PF00905  PF00768  PF13354  PF00144 PDB_complete.


            PDBInterface pdbInterface = new PDBInterface(filenamePDB);

            NetworkConstruction net = new NetworkConstruction();



        }

        public static void compileTexts()
        {
            StreamWriter sw = new StreamWriter("Text/PDB_complete.txt");

            StreamReader sr = new StreamReader("Text/PDB_3526.txt");
            string complete = sr.ReadToEnd();
            sw.WriteLine(complete);

            sr = new StreamReader("Text/PDB_34164.txt");
            complete = sr.ReadToEnd();
            sw.WriteLine(complete);

            sr = new StreamReader("Text/PF00905.txt");
            complete = sr.ReadToEnd();
            sw.WriteLine(complete);

            sr = new StreamReader("Text/PF00768.txt");
            complete = sr.ReadToEnd();
            sw.WriteLine(complete);

            sr = new StreamReader("Text/PF13354.txt");
            complete = sr.ReadToEnd();
            sw.WriteLine(complete);

            sr = new StreamReader("Text/PF00144.txt");
            complete = sr.ReadToEnd();
            sw.WriteLine(complete);
        }




    }
}