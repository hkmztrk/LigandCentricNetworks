using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using chemaxon.descriptors;
using chemaxon.formats;
using System.Net;
using System.Xml.Linq;

namespace LigandCentricNetworkModels
{
    class PDBInterface
    {


        #region DEF_exclude

        /* contains ions, modified residues, and
         * ligands which are listed to associate with
         * more than 50 entries in PDB
         */
        #endregion

        public static List<Node> completeNodes = new List<Node>();
        public static List<ligand> completeLigands = new List<ligand>();

        public static List<Node> nodesFromPDB = new List<Node>();
        public static List<ligand> ligandsFromPDB = new List<ligand>();
        public static List<Node> nonExistingnodes = new List<Node>();

        // "NC1","PG1",
        private static string[] exclude = new string[]{ "ZN", "PO4", "SO4","CO3","ACT","CO","K","NA","CIT","BA",
                                                     "IOD","MN", "NO3", "EMC", "SCN","CA","CD","CL","BCT","MG", 
                                                     "CAC", "NI","CU","LU","HG", "OH","AZI","DOD", "IOD",  "EDO",
                                                      "MES","BHD","EPE","SUC","KCX","DMS","PEG","ACY","GOL", "CO2", 
                                                      "PG4","EOH","PGE","FMT","OCS","BME","MRD","CSW","PCA", "ACN",
                                                      "CSD","CSO","FLC","IPA","IMD","P6G","NHE","DAL","SEP",
                                                      "SIN","POP","LDA","TLA","DPN","DTT", "MSE", "MPD" };



        public PDBInterface(string filename)
        {
            Console.WriteLine("Preparing data..");
            this.extractPDBInfo(filename);

            this.addPDBDatabase();

            Console.WriteLine("Completed!");
        }



        public void extractPDBInfo(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            string headlines = sr.ReadLine();
            //int reviewed = -1;

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] arr = line.Split('\t');

                string element = arr[0];
                string lgCode = arr[1];
                string lgSmiles = arr[2];

                string[] elements = element.Split(',');


                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i] != String.Empty && elements[i] != "-" && lgCode != String.Empty &&
                                                                               elements[i].Length > 4 && lgSmiles != String.Empty)
                    {

                        elements[i] = elements[i].TrimStart();

                        ligand currentLigand = new ligand(lgCode, lgSmiles, string.Empty, string.Empty);

                        Node nNode = new Node(elements[i]);

                        if (!exclude.Contains(currentLigand.lID))// If Ligand is NOT an ION 
                        {

                            if (!(ligandsFromPDB.Any(l => l.lID == currentLigand.lID)))
                            {
                                ligandsFromPDB.Add(currentLigand);
                            }


                            if (!(nodesFromPDB.Any(l => l.uniProtID == nNode.uniProtID)))
                            {
                                nNode.ligands.Add(currentLigand);
                                nodesFromPDB.Add(nNode);
                            }
                            else
                            {

                                int index = returnExistingNodeIndexfromNodeList(nodesFromPDB, nNode); //Node's index

                                if (doesMonomerIDExistinNode(nodesFromPDB[index], currentLigand) == 0)
                                {
                                    nodesFromPDB[index].ligands.Add(currentLigand);
                                }


                            }
                        }


                    }
                }


            }
        }





        public void addPDBDatabase()
        {


            foreach (Node pNode in nodesFromPDB)
            {
                //Console.WriteLine("Please Wait..");

                if (!(completeNodes.Any(l => l.uniProtID == pNode.uniProtID)))
                {

                    foreach (ligand currentLigand in pNode.ligands)
                    {


                        int indexofExistingLigand = returnExistingMonomerIndexfromLigandList(completeLigands, currentLigand);
                        // -1 or NUMBER


                        if (indexofExistingLigand == -1) // If Ligand is NOT in Ligandlist
                        {

                            currentLigand.nodes.Add(pNode);
                            completeLigands.Add(currentLigand);

                        }

                        else //If Ligand is IN Ligandlist
                        {

                            int isReallySameLigand = checkSimilarityTanimoto(completeLigands[indexofExistingLigand], currentLigand);

                            if (isReallySameLigand == 0) //Not Same
                            {
                                currentLigand.nodes.Add(pNode);
                                completeLigands.Add(currentLigand);
                            }
                            else
                            {
                                completeLigands[indexofExistingLigand].nodes.Add(pNode);
                            }


                        }

                    }

                    completeNodes.Add(pNode);


                }
                else
                {

                    int index = returnIndexofNode(completeNodes, pNode);


                    foreach (ligand currentLigand in pNode.ligands)
                    {

                        //if (!ions.Contains(currentLigand.lID))// If Ligand is NOT an ION 
                        //{

                        int indexofExistingLigand = returnExistingMonomerIndexfromLigandList(completeLigands, currentLigand);
                        // -1 or NUMBER


                        if (indexofExistingLigand == -1) // If Ligand is NOT in Ligandlist
                        {

                            currentLigand.nodes.Add(completeNodes[index]);
                            completeLigands.Add(currentLigand);

                        }

                        else //If Ligand is  In Ligandlist
                        {

                            int isReallySameLigand = checkSimilarityTanimoto(completeLigands[indexofExistingLigand], currentLigand);

                            if (isReallySameLigand == 0) //Not Same
                            {
                                currentLigand.nodes.Add(completeNodes[index]);
                                completeLigands.Add(currentLigand);
                            }
                            else if (isReallySameLigand == 1) //Same
                            {
                                int doesNodeHasLigand = doesMonomerExistinNode(completeNodes[index], currentLigand);


                                if (doesNodeHasLigand == 0) // Ligand Node listesinde yoksa
                                {
                                    completeLigands[indexofExistingLigand].nodes.Add(pNode);
                                    completeNodes[index].addLigand(currentLigand);
                                }
                            }


                        }
                        //} // end of ion if


                    }

                }

            }
        }






        private int doesMonomerExistinNode(Node n, ligand lg1) // is monomerid unique??
        {

            if ((n.ligands.Any(l => l.lSMILES == lg1.lSMILES)))
            {
                return 1;

            }
            else
                return 0;


        }

        private int doesMonomerIDExistinNode(Node n, ligand lg1) // is monomerid unique???
        {

            if ((n.ligands.Any(l => l.lID == lg1.lID)))
            {
                return 1;

            }
            else
                return 0;


        }



        public int returnExistingMonomerIndexfromLigandList(List<ligand> lg, ligand lg2)
        {
            int index = -1;


            if ((lg.Any(l => l.lSMILES == lg2.lSMILES)))
            {
                index = lg.FindIndex(
                             delegate(ligand l)
                             {
                                 return l.lSMILES == lg2.lSMILES;
                             });
            }

            return index;
        }



        public int returnExistingNodeIndexfromNodeList(List<Node> lg, Node nd)
        {
            int index = lg.FindIndex(
                               delegate(Node l)
                               {
                                   return l.uniProtID == nd.uniProtID;
                               });

            return index;
        }

        public int checkSmilarityString(ligand lg1_, ligand lg2_)
        {
            int flag = 0;


            if (lg1_.lSMILES == lg2_.lSMILES)
                flag = 1;

            return flag;
        }



        public int checkSimilarityTanimoto(ligand lg1_, ligand lg2_)
        {
            int flag = 0;

            float score = calculateTanimotoScore(lg1_, lg2_);

            if (score == 1)
                flag = 1;

            return flag;
        }


        public float calculateTanimotoScore(ligand lg1, ligand lg2)
        {

            string smiles1 = lg1.lSMILES;
            string smiles2 = lg2.lSMILES;

            CFParameters cfpConfig = new CFParameters();
            ChemicalFingerprint cf1 = new ChemicalFingerprint(cfpConfig);
            ChemicalFingerprint cf2 = new ChemicalFingerprint(cfpConfig);


            cf1.generate(MolImporter.importMol(smiles1));
            cf2.generate(MolImporter.importMol(smiles2));


            return (1 - cf1.getTanimoto(cf2));

        }


        public int returnIndexofNode(List<Node> nd, Node n)
        {
            int index = nd.FindIndex(
                             delegate(Node l)
                             {
                                 return l.uniProtID == n.uniProtID;
                             });

            return index;
        }












    }
}
