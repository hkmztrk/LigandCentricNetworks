using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LigandCentricNetworkModels
{
    class ligand
    {
        //TODO: add SMILES 

        public string lID { get; set; }
        public string lSMILES { get; set; }
        public string lAffinity_type { get; set; }
        public string lAffinity_score { get; set; }

        public List<Node> nodes = new List<Node>();


        public ligand(string id, string Smiles, string type, string score)
        {
            this.lID = id;
            this.lSMILES = Smiles;
            this.lAffinity_type = type;
            this.lAffinity_score = score;
        }

        public void addNode(Node newNode)
        {
            this.nodes.Add(newNode);
        }

    }
}
