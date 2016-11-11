// This example code is in the public domain.

import processing.serial.*;
import processing.opengl.*;


int bgcolor;                 // Background color
int fgcolor;                 // Fill color
Serial myPort;                       // The serial port
int[] serialInArray = new int[225];    // Where we'll put what we receive
int[] pastInArray = new int [225];
float[][] colorTarget   = new float[3][255];
float[][] currentColor   = new float[3][255];
PVector[][] vertices = new PVector[15][15];
float[] verticesTZ = new float[15];
float w = 40;
float ease = 0.75; 
float z_factor_data = 40;

int serialCount = 0;                 // A count of how many bytes we receive
int xpos, ypos;                  // Starting position of the ball
boolean firstContact = false;        // Whether we've heard from the microcontroller
int tiempoant;
int render=0;
int dif=0;

//
float pitch=0, roll=0; 
int feetsensor=0;  
float timemili;
  
PrintWriter output;

void setup() {
  size(960, 600, OPENGL);  // Stage size
  noStroke();      // No border on the next thing draw
  
  // Print a list of the serial ports, for debugging purposes:
  //println(Serial.list());

  myPort = new Serial(this, Serial.list()[0], 9600);
  myPort.bufferUntil('\n');
  
  for (int j = 0; j < 8; j++) {
        for (int i = 0; i < 12; i++) {
            vertices[i][j] = new PVector( i*w, j*w, 0);
        }
    }  
  output = createWriter("5squats.txt"); 
  
}

void draw() {
    //println
    //
    rotateX(3.15*6.0/5.0);
    rotateY(3.15);
    //rotateZ(3.15);

    translate(-width/1.3, -height/1.3);
    //rotateX(PI/10);
    background(0);

     for (int j=0; j<7; j++) {
       beginShape(QUAD_STRIP);
         for (int i=0; i<12; i++) {
          stroke(255);
          float colorfill = (vertices[i][j].z)*255.0/4095.0;//((vertices[i][j].z+vertices[i][j+1].z+ vertices[i+1][j].z+vertices[i+1][j+1].z)*255.0*40/4095.0)/4.0;
          fill(colorfill, 0, 0);
          vertex( vertices[i][j].x, vertices[i][j].y, -vertices[i][j].z/z_factor_data);
          vertex( vertices[i][j+1].x, vertices[i][j+1].y, -vertices[i][j+1].z/z_factor_data);       
         }
         endShape(CLOSE);
      }    


}

void serialEvent(Serial myPort) {
  // read the serial buffer:
  String myString = myPort.readStringUntil('\n');
  // if you got any bytes other than the linefeed: 
  if (myString != null) {
    myString = trim(myString);
    String items[] = split(myString, ','); 
    pitch = float(items[0]);
    roll = float(items[1]);
    feetsensor = int(items[2]);
    int index=3;
    //loop row first
    timemili = millis();
    output.print(timemili);
    output.print(",");
    for (int rowcount = 0;rowcount<8;rowcount++) {
       //then loop columns
       for (int columncount = 0;columncount<12;columncount++) {
        //serialInArray[index-3]=int(items[index]); 
        vertices[columncount][rowcount].z=int(items[index]);
        //vertices[rowcount][columncount].z=int(items[index])/40; 
        output.print(vertices[columncount][rowcount].z);
        output.print(",");
        index=index+1;
        } 
        
    }
    output.print("\n");
    
  }
}

void keyPressed() {
  output.flush();  // Writes the remaining data to the file
  output.close();  // Finishes the file
  exit();  // Stops the program
}