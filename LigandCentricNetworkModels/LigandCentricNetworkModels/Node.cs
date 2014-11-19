using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LigandCentricNetworkModels
{
    class Node
    {

        public string uniProtID { get; set; }
        //public string sequence { get; set; }

        public List<ligand> ligands = new List<ligand>();


        public Node(string id)
        {
            this.uniProtID = id;
            //this.sequence = sequence_;
        }

        public void addLigand(ligand lg)
        {
            this.ligands.Add(lg);
        }


    }
}
