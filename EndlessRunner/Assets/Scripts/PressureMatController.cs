using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Linq;  


public class PressureMatController : MonoBehaviour {

	public GameObject player;
	private PlayerController playerController;
	int threshold_data = 50000;
	string line;
	float time;
	float[] add_data_time = Enumerable.Repeat(0.0f,44).ToArray();
	float[] projection1_time = Enumerable.Repeat(0.0f,44).ToArray();
	float mean_time_data;
	float distance;
	float[] polynomial_squad = new float[4] {3469.0f,-689.5f,24.17f,-0.1827f};
	float[] polynomial_push_up = new float[4] {-177.2f,-6.723f,1.115f,-0.01836f};
	//for squad: -0.1827 x3 + 24.17 x2 - 689.5 x + 3469, average, length 44, order 4 for distance
	//for push-up: -0.01836 x3 + 1.115 x2 - 6.723 x - 177.2, length: 44
	float[] mat_data = new float[96];
	float[] mean_vector = new float[96];
	float[,] basis = new float[3,96];
	float[,] centroids = new float[6,3];
	float[] projections = new float[3];
	int state;
	float add_data;
	//float[] basis2 = new float[96];
	//float[] basis3 = new float[96];
	string path;
	int j;
	bool pushUpDetected = false;

	System.IO.StreamReader file1; 


	int lineCount;
	int lineNumber = 0;

	// Use this for initialization
	void Start () {

		playerController = player.GetComponent<PlayerController> ();
		LoadTrainingSet ();

		path = "/Users/j_kay/Documents/IDD/git_final/hw3_gamecontrol/matplot/matdraw/2016_11_26_test.txt";
		file1 = new System.IO.StreamReader(path);

		//Get number of lines

		while (file1.ReadLine() != null)
		{
			lineCount++;
		}


	}
	
	// Update is called once per frame
	void Update () {

		pushUpDetected = false;
		if (playerController.GetPushUp()) {
			//print ("MAT GO!");
			print ("Line Count" + lineCount);
			if (lineNumber < lineCount) {
				//print ("INSIDE MAT");
				PressureMatDetection (lineNumber);
				lineNumber++;
			} else {
				lineNumber = 0;
			}
		}


	}


	void LoadTrainingSet () {
		/////////////////////////////////////////////////enter mean vector information
		path = "/Users/j_kay/Documents/IDD/git_final/hw3_gamecontrol/ML/mean3steps_v3.txt";
		System.IO.StreamReader file0 = new System.IO.StreamReader (path);
		j = 0;
		while ((line = file0.ReadLine ()) != null) {
			mean_vector [j] = float.Parse (line);
			j = j + 1;
		}


		/////////////////////////////////////////////////enter basis vector information
		path = "/Users/j_kay/Documents/IDD/git_final/hw3_gamecontrol/ML/basis3steps_v3.txt";
		System.IO.StreamReader file2 = new System.IO.StreamReader (path);
		j = 0;
		while ((line = file2.ReadLine ()) != null) {
			//Console.WriteLine(line);
			string[] data = line.Split (',');
			//Console.WriteLine(data.Length);

			for (int i = 0; i < data.Length; i++) {
				basis [j, i] = float.Parse (data [i]);
				//Console.WriteLine(data[i]);
			}
			j = j + 1;
		}

		/////////////////////////////////////////////////enter clusters information
		path = "/Users/j_kay/Documents/IDD/git_final/hw3_gamecontrol/ML/cluster3steps_v3.txt";
		System.IO.StreamReader file3 = new System.IO.StreamReader (path);
		j = 0;
		while ((line = file3.ReadLine ()) != null) {
			//Console.WriteLine(line);
			string[] data = line.Split (',');
			//Console.WriteLine(data.Length);

			for (int i = 0; i < data.Length; i++) {
				centroids [j, i] = float.Parse (data [i]);
				//Console.WriteLine(data[i]);
			}
			j = j + 1;
		}
	}


	string GetLine(string fileName, int line)
	{
		using (var sr = new StreamReader (fileName)) {
			for (int i = 1; i < line; i++)
				sr.ReadLine ();
			return sr.ReadLine ();
		}
	}


	void PressureMatDetection(int lineNumber) {
			
			//line = file1.ReadLine (lineNumber);
			path = "/Users/j_kay/Documents/IDD/git_final/hw3_gamecontrol/matplot/matdraw/2016_11_26_test.txt";
			line = GetLine(path, lineNumber);
			//Console.WriteLine(line);
			string[] data = line.Split(',');
		//print ("Line" + line);
		//print ("Data" + data);
			//Console.WriteLine(data.Length);
			time = float.Parse(data[0])/1000;
			add_data = 0;
			for (int i = 1; i < data.Length-1; i++)
			{
				mat_data[i-1] = float.Parse(data[i]);
				add_data = add_data+mat_data[i-1];
				//Console.WriteLine(data[i]);
			}

			//Classification:
			//state= -1 (no one on board), 4: right feet, 1: left feet, 0: both foots, 2: right hand, 5: left hand, 3: both hands
			if (add_data>threshold_data) {//check threshold data
				projections = PCA_classify(mat_data,basis,mean_vector,projections);
				state = which_cluster(projections,centroids);
			} else {
				state=-1;
			}
			Console.WriteLine(time);
			Console.WriteLine(state);

			//detect squad, this is like a switch data, so a positive must be handle like such, i.e. many positives will be together
			add_data_time = FIFO_update(add_data_time,add_data);
			mean_time_data = mean_data(add_data_time);
			distance = distance_to_template(add_data_time,mean_time_data,polynomial_squad);
			Console.WriteLine(distance);
			if ((distance)<4.267e14){
				print("squad squad squad squad squad squad squad");
			}

			//detect push up, this is like a switch data, so a positive must be handle like such, i.e. many positives will be together
			projection1_time = FIFO_update(projection1_time,projections[0]);
			mean_time_data = mean_data(projection1_time);
			distance = distance_to_template(projection1_time,mean_time_data,polynomial_push_up);
			if ((distance)<7483147950.81){
				print("push-up push-up push-up push-up push-up push-up ");
				pushUpDetected = true;
			}
		///////////////////////////////////////////////////////////
	}


	public bool GetPushUpDetected() {
		return pushUpDetected;
	}


	public float mean_data(float[] data){
		float mean_aux = 0;
		for (int i = 0; i < data.Length; i++)
		{
			mean_aux = data[i]+mean_aux;
		}
		mean_aux = mean_aux/data.Length;
		return mean_aux;
	}
	public float distance_to_template(float[] data, float mean_time_data, float[] polynomial){//TODO: order to change, , int order_distance
		float distance = 0;
		float distance_aux = 0;
		float evaluate_poly = 0;
		for (int j = 0; j < data.Length; j++){

			evaluate_poly = polynomial[0] + polynomial[1]*j + polynomial[2]*j*j + polynomial[3]*j*j*j;
			distance_aux=evaluate_poly-(data[j]-mean_time_data);
			//distance_aux = distance_aux*distance_aux*distance_aux*distance_aux;
			distance=distance+distance_aux*distance_aux*distance_aux*distance_aux;
		}
		return distance;
	}



	public float[] PCA_classify(float[] data, float[,] basis, float[] mean, float[] projection) {
		//int width = basis.GetLength(0);
		//int height = basis.GetLength(1);
		//1st step: substract mean
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = data[i]-mean[i];
		}

		//2nd step, dot product with each basis
		for (int j = 0; j < projection.Length; j++){
			projection[j]=0;
			for (int i = 0; i < data.Length-1; i++)
			{
				projection[j] = projection[j]+data[i]*basis[j,i];
			}
		}
		return projection;
	}




	public int which_cluster(float[] projection, float[,] centroids){
		//Console.WriteLine(centroids.GetLength(0));
		int cluster_choose;
		float distance = 0;
		for (int i = 0; i < projection.Length-1; i++)
		{
			distance = distance+(projection[i]-centroids[0,i])*(projection[i]-centroids[0,i]);
		}
		cluster_choose = 0;
		float distance_aux = 0;
		for (int j = 1; j <centroids.GetLength(0); j++)
		{
			distance_aux = 0;
			for (int i = 0; i < projection.GetLength(0); i++)
			{
				distance_aux = distance_aux+(projection[i]-centroids[j,i])*(projection[i]-centroids[j,i]);
			}
			if (distance_aux<distance){
				distance = distance_aux;
				cluster_choose = j;
			}
		}
		return cluster_choose;
	}




	public void print_data(float[] data) {
		for (int i = 1; i < data.Length; i++)
		{
			Console.WriteLine(data[i]);
		}
	}

	public float[] FIFO_update(float[] data, float new_data){
		for (int i = 0; i < data.Length-1; i++)
		{
			data[i] = data[i+1];
		}
		data[data.Length-1] = new_data;
		return data;
	}

}
