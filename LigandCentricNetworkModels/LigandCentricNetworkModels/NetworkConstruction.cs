using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using chemaxon.descriptors;
using chemaxon.formats;
using chemaxon.standardizer;
using System.IO;


namespace LigandCentricNetworkModels
{
    class NetworkConstruction
    {


        private static float tanimoto_sim_threshold = (float)0.7;

        List<ligand> completeligands = PDBInterface.completeLigands;
        List<Node> completenodes = PDBInterface.completeNodes;

        List<Edge> unweightedSimEdges = new List<Edge>();
        List<Edge> weightedSimEdges = new List<Edge>();
        List<Edge> normWeiSimEdges = new List<Edge>();

        List<Edge> unweightedIdeEdges = new List<Edge>();
        List<Edge> weightedIdeEdges = new List<Edge>();
        List<Edge> normWeiIdeEdges = new List<Edge>();


        CFParameters cfpConfig = new CFParameters();



        public NetworkConstruction()
        {
            this.operate();
        }


        public void operate()
        {


            // 0: unweighted 1:weighted 2: normalized weighted


            constructIdentityNetwork(0, unweightedIdeEdges); //Unweighted 
            writeEdgeLists(unweightedIdeEdges, "ide_unwei");
            constructIdentityNetwork(1, weightedIdeEdges); //Weighted 
            writeEdgeLists(weightedIdeEdges, "ide_wei");
            constructIdentityNetwork(2, normWeiIdeEdges); //Normalized weighted 
            writeEdgeLists(normWeiIdeEdges, "ide_norm_wei");


            constructSimilarityNetwork(0, unweightedSimEdges); //Unweighted 
            writeEdgeLists(unweightedSimEdges, "sim_unwei");
            constructSimilarityNetwork(1, weightedSimEdges); //Weighted 
            writeEdgeLists(weightedSimEdges, "sim_wei");
            constructSimilarityNetwork(2, normWeiSimEdges); //Normalized weighted 
            writeEdgeLists(normWeiSimEdges, "sim_norm_wei");



        }




        public void constructIdentityNetwork(int networkTyp, List<Edge> edgeList)
        {
            Console.Write("-------Constructing Unweighted Identity Network----------\n");

            foreach (ligand activeLigand in completeligands) // Complete Ligands
            {
                for (int i = 0; i < activeLigand.nodes.Count; i++)
                {
                    Node activeNode = activeLigand.nodes[i];

                    for (int j = i + 1; j < activeLigand.nodes.Count; j++)
                    {

                        Node nextNode = activeLigand.nodes[j];

                        string edge_id = activeNode.uniProtID + "_" + nextNode.uniProtID;


                        if (doesEdgeExist(activeNode, nextNode, edgeList) == false)
                        {
                            Edge newEdge = new Edge(edge_id, activeLigand.lID, activeNode, nextNode, (float)1);
                            edgeList.Add(newEdge);


                        }
                        else
                        {
                            int index = returnExistingEdgeIndex(activeNode, nextNode, edgeList);
                            edgeList[index].ligandID = edgeList[index].ligandID + "_" + activeLigand.lID;

                            if (networkTyp == 1 || networkTyp == 2)
                            {

                                float edgeWeight = edgeList[index].weight + (float)1;
                                edgeList[index].weight = edgeWeight;
                            }


                        }



                    }

                }


            }

            if (networkTyp == 2)
            {
                List<Edge> identitytempEdges = new List<Edge>(edgeList);

                edgeList.Clear();

                foreach (ligand activeLigand in completeligands) // Complete Ligands
                {
                    for (int i = 0; i < activeLigand.nodes.Count; i++)
                    {
                        Node activeNode = activeLigand.nodes[i];

                        for (int j = i + 1; j < activeLigand.nodes.Count; j++)
                        {

                            Node nextNode = activeLigand.nodes[j];

                            if (doesEdgeExist(activeNode, nextNode, identitytempEdges) == true)
                            {
                                int index = returnExistingEdgeIndex(activeNode, nextNode, identitytempEdges);

                                int union = (int)identitytempEdges[index].weight;
                                int num_of_total_ligs = activeNode.ligands.Count() + nextNode.ligands.Count() - union;

                                float edgeWeight = (float)Math.Round((identitytempEdges[index].weight / (float)num_of_total_ligs), 2);

                                string edge_id = activeNode.uniProtID + "_" + nextNode.uniProtID;
                                Edge weighted = new Edge(edge_id, identitytempEdges[index].ligandID, activeNode, nextNode, edgeWeight);
                                edgeList.Add(weighted);



                            }


                        }
                    }
                }


            }


        }




        public string similarityEdgeLigands = String.Empty;

        public void constructSimilarityNetwork(int networkType, List<Edge> edgeList)  // 0: unweighted 1:weighted 2: normalized weighted
        {
            Console.Write("-------Constructing Similarity Network----------\n");


            for (int i = 0; i < completenodes.Count; i++)
            {
                Console.WriteLine("Please Wait..");

                Node node1 = completenodes[i];

                for (int j = i + 1; j < completenodes.Count; j++)
                {

                    Node node2 = completenodes[j];

                    string edge_id = node1.uniProtID + "_" + node2.uniProtID;
                    float weight = calculateEdgeWeight(node1, node2, networkType);

                    if (weight != 0)
                    {

                        Edge weighted = new Edge(edge_id, similarityEdgeLigands, node1, node2, weight);
                        edgeList.Add(weighted);
                        similarityEdgeLigands = String.Empty;
                    }

                }
            }


        }




        public float calculateEdgeWeight(Node n1, Node n2, int networkTy)  // 0: unweighted 1:weighted 2: normalized weighted
        {

            float sum_score = (float)0;
            int num_of_shared = 0;
            int active_pairs = 0;

            for (int i = 0; i < n1.ligands.Count; i++)
            {
                ligand lgd1 = n1.ligands[i];



                for (int j = 0; j < n2.ligands.Count; j++)
                {
                    ligand lgd2 = n2.ligands[j];


                    float sim_score = calculateTanimotoScore(lgd1, lgd2);

                    if (sim_score > tanimoto_sim_threshold)
                    {
                        sum_score = sum_score + sim_score;
                        similarityEdgeLigands = similarityEdgeLigands + "_" + lgd1.lID + "_" + lgd2.lID;
                        active_pairs++;
                    }

                    if (sim_score == 1)
                        num_of_shared++;

                }
            }

            int num_of_ligands = (n2.ligands.Count + n1.ligands.Count) - num_of_shared;
            float weight = (float)0;

            if (sum_score != 0)
            {
                if (networkTy == 0)
                    weight = (float)1;
                else if (networkTy == 1)
                    weight = (float)Math.Round(sum_score, 2);
                if (networkTy == 2)
                    weight = (float)Math.Round(sum_score / (float)num_of_ligands, 2);
            }


            return weight;
        }




        public float calculateTanimotoScore(ligand lg1, ligand lg2)
        {

            string smiles1 = lg1.lSMILES;
            string smiles2 = lg2.lSMILES;



            if (smiles1 != string.Empty && smiles2 != string.Empty)
            {

                ChemicalFingerprint cf1 = new ChemicalFingerprint(cfpConfig);
                ChemicalFingerprint cf2 = new ChemicalFingerprint(cfpConfig);


                cf1.generate(MolImporter.importMol(smiles1));
                cf2.generate(MolImporter.importMol(smiles2));


                return (1 - cf1.getTanimoto(cf2));
            }
            else return (float)0;

        }


        public bool doesEdgeExist(Node nd1, Node nd2, List<Edge> edgeList)
        {
            string edgeID = nd1.uniProtID + "_" + nd2.uniProtID;

            if (edgeList.Any(l => l.ID == edgeID))
            {
                return true;
            }

            else
            {
                edgeID = nd2.uniProtID + "_" + nd1.uniProtID;

                if (edgeList.Any(l => l.ID == edgeID))
                {
                    return true;
                }

                return false;
            }
        }

        public int returnExistingEdgeIndex(Node nd1, Node nd2, List<Edge> edgeList)
        {
            string edgeID = nd1.uniProtID + "_" + nd2.uniProtID;

            int index = edgeList.FindIndex(
                               delegate(Edge e)
                               {
                                   return e.ID == edgeID;
                               });


            if (index == -1)
            {
                edgeID = nd2.uniProtID + "_" + nd1.uniProtID;

                index = edgeList.FindIndex(
                               delegate(Edge e)
                               {
                                   return e.ID == edgeID;
                               });

            }

            return index;
        }







        public void writeEdgeLists(List<Edge> eList, string filename)
        {
            StreamWriter sw = new StreamWriter(filename + ".txt");

            foreach (Edge e in eList)
            {
                string line = e.leftNode.uniProtID + "\t" + e.ligandID +
                                               "\t" + e.rightNode.uniProtID + "\t" + e.weight.ToString();

                sw.WriteLine(line);
            }

            sw.Close();
        }

        public void writeSimScores(List<float> scores)
        {
            StreamWriter sw = new StreamWriter("tanimoto_scores.txt");

            foreach (float el in scores)
                sw.WriteLine(el);

            sw.Close();
        }

        public void writeNodeInfo(List<Node> nodeList)
        {
            StreamWriter sw = new StreamWriter("node_ligand_info.txt");
            int countOnes = 0;

            foreach (Node nd in nodeList)
            {
                sw.Write(nd.uniProtID + "\t" + nd.ligands.Count.ToString());

                for (int i = 0; i < nd.ligands.Count; i++)
                {
                    sw.Write("\t" + nd.ligands[i].lID);
                }

                sw.WriteLine();

                if (nd.ligands.Count == 1)
                    countOnes++;
            }

            sw.WriteLine("Ones:" + countOnes.ToString());
            sw.WriteLine("Number of Nodes:" + nodeList.Count());
            sw.Close();
        }

        public void writeLigandInfo(List<ligand> ligandList)
        {
            StreamWriter sw = new StreamWriter("ligand_node_info.txt");
            int countOnes = 0;

            foreach (ligand lg in ligandList)
            {
                sw.Write(lg.lID + "\t" + lg.nodes.Count.ToString() + "\t" + lg.lID + "_");

                for (int i = 0; i < lg.nodes.Count; i++)
                {
                    sw.Write(lg.nodes[i].uniProtID + "_");
                }

                sw.WriteLine();
                if (lg.nodes.Count == 1)
                    countOnes++;
            }

            sw.WriteLine("Ones:" + countOnes.ToString());
            sw.WriteLine("Number of Ligands:" + ligandList.Count());
            sw.Close();
        }

        public void writeLigands(List<ligand> ligandList, string name)
        {
            StreamWriter sw = new StreamWriter(name);


            foreach (ligand lg in ligandList)
            {
                sw.WriteLine(lg.lID);

            }
            sw.Close();
        }





    }
}
