LigandCentricNetworks
=====================
-------------------------------------------------------------------------------------------------------------
Content
-----------
Source code is written in C#, and contains a Visual Studio 2010 solution named 'LigandCentricNetworkModels '. 

It includes:

Edge.cs /*defines edge property*/

ligand.cs /*defines ligand*/

NetworkConstruction.cs /*creates similarity and identity networks*/

Node.cs /*defines node*/

PDBInterface.cs /*reads input file provided as txt file and prepares for use*/

Program.cs /*main*/

--------------------------------------------------------------------------------------------------------------
Input
--------
To use your own data, you should prepare it as the following input form in a text file,

	ProteinID  LigandID   SMILES
	
	ProteinID  LigandID   SMILES

	....

i.e.,


	P25910	MES	C1COCC[NH+]1CCS(=O)(=O)[O-]
	
	P0C5C1	SFR	C[C@H]([C@H]([C@@H]1NC(=C(S1)[C@H]2CCCO2)C(=O)O)C(=O)O)O
	
	P0AD64	17O	c1c2n(nc1C3C(=CNC(=CS3)C(=O)O)C=O)CSC2

	....

After preaparing your file, "input.txt", you should place it under the folder "...\LigandCentricNetworkModels \bin\Debug\Text\".


Finally, go to LigandCentricNetworkModels.sln, and change the following input parameter in "Program.cs" with the name of your input file:

	string filenamePDB = "Text/PDB_complete.txt"; 


--------------------------------------------------------------------------------------------------------------
Output
-------

Outputs of the code are six different text files each containing a network, which are:

1 ide_unw.txt unweighted identity
2 ide_wei.txt  weighted identity
3 ide_norm_wei.txt normalized weighted identity
4 sim_unw.txt  unweighted similarity
5 sim_wei.txt weighted similarity
6 sim_norm_wei.txt normalized weighted similarity

