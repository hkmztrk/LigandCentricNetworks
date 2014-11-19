using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LigandCentricNetworkModels
{
    class Edge
    {
        public string ID { get; set; }
        public string ligandID { get; set; }
        public Node rightNode { get; set; }
        public Node leftNode { get; set; }
        public float weight { get; set; }


        public Edge(string id, string ligandID_, Node left, Node right, float weight_)
        {
            this.ID = id;
            this.ligandID = ligandID_;
            this.leftNode = left;
            this.rightNode = right;
            this.weight = weight_;
        }


    }
}
