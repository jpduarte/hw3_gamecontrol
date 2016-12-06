using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Linq;  


public class PressureMatController : MonoBehaviour {

	public GameObject player;
	private PlayerController playerController;
	public int threshold_data = 15000;
	string line;
	float time;
	float mean_time_data;
	float mean_time_data0;
	float mean_time_data1;
	float mean_time_data2;
	float distance = 0;
	float distance_sum = 0;
	float distance0 = 0;
	float distance1 = 0;
	float distance2 = 0;

	float[] polynomial_squat0 =  new float[4]{ 1475f, -224f, 4.881f, -0.01105f };
	float[] polynomial_squat1 =  new float[4]{ -3.937f, 48.79f, -2.24f, 0.02271f };
	float[] polynomial_squat2 =  new float[4]{ -202.7f, 15.46f, 0.07076f, -0.006437f};
	float[] polynomial_squat3 =  new float[4]{ -213.1f, 10.42f, 0.4034f,-0.01084f };


	float[] polynomial_pushup0 =  new float[4]{ 1475f, -224f, 4.881f, -0.01105f };
	float[] polynomial_pushup1 =  new float[4]{ -3.937f, 48.79f, -2.24f, 0.02271f };
	float[] polynomial_pushup2 =  new float[4]{ -202.7f, 15.46f, 0.07076f, -0.006437f};
	float[] polynomial_pushup3 =  new float[4]{ -213.1f, 10.42f, 0.4034f,-0.01084f };

						

	float squat_threshold = 6250000000.0f;
	float pushup_threshold =   6250000000.0f;

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
	bool squatDetected = false;

	float[] squat_weights = new float[4] {0.0f,0f,0f,1f};
	float[] pushUp_weights = new float[4] {0f,0f,0f,1f};


	Queue<float> matDataTimeQueue = new Queue<float>(60);
	float[] add_data_time = Enumerable.Repeat(0.0f,60).ToArray();
	Queue<float> projectionQueue0 = new Queue<float>(60);
	float[] projection0_time = Enumerable.Repeat(0.0f,60).ToArray();
	Queue<float> projectionQueue1 = new Queue<float>(60);
	float[] projection1_time = Enumerable.Repeat(0.0f,60).ToArray();
	Queue<float> projectionQueue2 = new Queue<float>(60);
	float[] projection2_time = Enumerable.Repeat(0.0f,60).ToArray();

	float add_data_smooth = 0; 
	float[] projection_smooth = new float[3];

	float[] add_data_filter = new float[5];  
	float[] projection0_filter = new float[5];
	float[] projection1_filter = new float[5];
	float[] projection2_filter = new float[5];

	int filter_count = 0;
	int filter_order = 5;

	int tagNow = 0;
	int tagLast = 0;

	DateTime matTimeStampNew = DateTime.Now;
	DateTime matTimeStampLast = DateTime.Now;

	System.IO.StreamReader file1; 


	int lineCount;
	//int lineNumber = 0;

	// Use this for initialization
	void Start () {

		playerController = player.GetComponent<PlayerController> ();
		LoadTrainingSet ();


	}
	
	// Update is called once per frame
	void Update () {


		if (playerController.GetExercise()) {
			//print ("MAT GO!");

			//matTimeStampNew = playerController.GetMatTimeStamp ();
			tagNow = playerController.GetTagNum();
			mat_data = playerController.GetMatData();
			PressureMatDetection ();
			tagLast = tagNow;
			//matTimeStampLast = matTimeStampNew;
		}


	}


	void LoadTrainingSet () {
		/////////////////////////////////////////////////enter mean vector information
		path = "/Users/j_kay/Documents/unity_games/EndlessRunner/Assets/TrainingData/mean3steps_josh_v1.txt";
		System.IO.StreamReader file0 = new System.IO.StreamReader (path);
		j = 0;
		while ((line = file0.ReadLine ()) != null) {
			mean_vector [j] = float.Parse (line);
			j = j + 1;
		}


		/////////////////////////////////////////////////enter basis vector information
		path = "/Users/j_kay/Documents/unity_games/EndlessRunner/Assets/TrainingData/basis3steps_josh_v1.txt";
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
		path = "/Users/j_kay/Documents/unity_games/EndlessRunner/Assets/TrainingData/cluster3steps_josh_v1.txt";
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


	void PressureMatDetection() {
			
		//line = file1.ReadLine (lineNumber);
		//path = "/Users/j_kay/Documents/IDD/git_final/hw3_gamecontrol/matplot/matdraw/2016_11_26_test.txt";
		//line = GetLine(path, lineNumber);
		//Console.WriteLine(line);
		//string[] data = line.Split(',');
		//print ("Line" + line);
		//print ("Data" + data);
		//Console.WriteLine(data.Length);
		//time = (mat_data[0])/1000;

		if (tagNow == tagLast) {
			return;
		}
	

		//Find Sum 
		add_data = 0;
		add_data = mat_data.Sum ();
		print ("Add Data " + add_data);
		//print ("Threshold " + threshold_data);

		if (filter_count > filter_order-1)
			filter_count = 0;


		//Classification:
		//state= -1 (no one on board), 4: right feet, 1: left feet, 0: both foots, 2: right hand, 5: left hand, 3: both hands
		if (add_data > threshold_data) {//check threshold data
			
			//filter add data
			add_data_filter [filter_count] = add_data;
			add_data_smooth = add_data_filter.Sum ()/ filter_order;

			projections = PCA_classify(mat_data,basis,mean_vector,projections);

			//filter projections
			projection0_filter [filter_count] = projections [0];
			projection1_filter [filter_count] = projections [1];
			projection2_filter [filter_count] = projections [2];

			projection_smooth[0] = projection0_filter.Sum ()/ filter_order;
			projection_smooth[1] = projection1_filter.Sum ()/ filter_order;
			projection_smooth[2] = projection2_filter.Sum ()/ filter_order;

			//print ("Smooth Add Data" + add_data_smooth);
		//print ("Projections [0] " + projections[0] + "Projections [1] " + projections[1] + "Projections [2] " + projections[2]);
			//state = which_cluster(projections,centroids);
			state = which_cluster(projection_smooth,centroids);



			//Get Mean Times
			matDataTimeQueue.Enqueue (add_data_smooth);
			add_data_time = matDataTimeQueue.ToArray ();
			mean_time_data = mean_data (add_data_time);


			projectionQueue0.Enqueue (projection_smooth [0]);
			projection0_time = projectionQueue0.ToArray ();
			//projection0_time = FIFO_update(projection0_time,projection_smooth[0]);
			mean_time_data0 = mean_data (projection0_time);

			projectionQueue1.Enqueue (projection_smooth [1]);
			projection1_time = projectionQueue1.ToArray ();
			//projection1_time = FIFO_update(projection1_time,projections[1]);
			mean_time_data1 = mean_data (projection1_time);


			projectionQueue2.Enqueue (projection_smooth [2]);
			projection2_time = projectionQueue2.ToArray ();
			//projection2_time = FIFO_update(projection2_time,projections[1]);
			mean_time_data2 = mean_data (projection2_time);


			//Check Squats
			distance_sum = distance_to_template (add_data_time, mean_time_data, polynomial_squat0);

			distance0 = distance_to_template (projection0_time, mean_time_data0, polynomial_squat1);

			distance1 = distance_to_template (projection1_time, mean_time_data1, polynomial_squat2);

			distance2 = distance_to_template (projection2_time, mean_time_data2, polynomial_squat3);

			distance = distance_sum * squat_weights [0] + distance0 * squat_weights [1] + distance1 * squat_weights [2] + distance2 * squat_weights [3];

			//print ("Distance: " + distance);
			if (((distance) < squat_threshold) && (matDataTimeQueue.Count == 60)) {
				print ("Squat");
				squatDetected = true;
			} else {
				squatDetected = false;
			}


			//Check PushUp
			distance_sum = distance_to_template (add_data_time, mean_time_data, polynomial_pushup0);

			distance0 = distance_to_template (projection0_time, mean_time_data0, polynomial_pushup1);

			distance1 = distance_to_template (projection1_time, mean_time_data1, polynomial_pushup2);

			distance2 = distance_to_template (projection2_time, mean_time_data2, polynomial_pushup3);

			distance = distance_sum * pushUp_weights [0] + distance0 * pushUp_weights [1] + distance1 * pushUp_weights [2] + distance2 * pushUp_weights [3];

			if (((distance) < pushup_threshold) && (matDataTimeQueue.Count == 60)) {
				print ("PushUp");
				pushUpDetected = true;
			} else {
				pushUpDetected = false;
			}

		} else {
			state=-1;
			pushUpDetected = false;
			squatDetected = false;
		}

		filter_count++;
		//Console.WriteLine(time);
		print("State: " + state);
		///////////////////////////////////////////////////////////
	}


	public bool GetPushUpDetected() {
		return pushUpDetected;
	}

	public bool GetSquatDetected() {
		return squatDetected;
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
